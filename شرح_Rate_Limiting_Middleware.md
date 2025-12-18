# شرح Rate Limiting Middleware بالتفصيل 🔒

## 📋 ما هو Rate Limiting؟

**Rate Limiting** هو آلية للتحكم في عدد الطلبات (Requests) التي يمكن لعميل معين (Client) إرسالها في فترة زمنية محددة.

---

## 🎯 الهدف من Rate Limiting

### 1. **حماية من DDoS Attacks** 🛡️
**المشكلة:**
- هجوم DDoS يرسل آلاف الطلبات في ثوانٍ
- يؤدي إلى استنزاف موارد الخادم
- قد يؤدي إلى تعطيل الخدمة

**الحل:**
- تحديد حد أقصى للطلبات (مثلاً: 100 طلب/دقيقة)
- رفض الطلبات التي تتجاوز الحد
- حماية الخادم من الاستنزاف

### 2. **حماية من Brute Force Attacks** 🔐
**المشكلة:**
- محاولات متكررة لتخمين كلمة المرور
- محاولات متكررة للدخول بحسابات غير موجودة
- استغلال endpoints حساسة

**الحل:**
- حدود صارمة للـ endpoints الحساسة:
  - Login: 5 محاولات/دقيقة
  - Change Password: 3 محاولات/5 دقائق
- منع المهاجم من المحاولات المتكررة

---

## 🔧 كيف يعمل Rate Limiting Middleware؟

### **الخطوات:**

```
1. Request يصل → Middleware
2. التحقق من Enabled
3. التحقق من Bypass Paths
4. تحديد Client (User ID أو IP)
5. التحقق من Whitelist/Blacklist
6. تحديد Rate Limit (Global أو Endpoint-specific)
7. التحقق من عدد الطلبات
8. إما السماح أو رفض الطلب
```

---

## 📊 مثال عملي

### **سيناريو 1: طلب عادي (ضمن الحد)**

```
العميل: user_123
Endpoint: /api/FormBuilder
الطلبات السابقة في الدقيقة: 50
الحد الأقصى: 100 requests/minute

✅ النتيجة: السماح بالطلب
Response Headers:
  X-RateLimit-Limit: 100
  X-RateLimit-Remaining: 50
  X-RateLimit-Used: 50
```

### **سيناريو 2: تجاوز الحد**

```
العميل: user_123
Endpoint: /api/Account/login
الطلبات السابقة في الدقيقة: 5
الحد الأقصى: 5 requests/minute

❌ النتيجة: رفض الطلب (429 Too Many Requests)
Response:
  {
    "error": "Too many requests",
    "message": "Rate limit exceeded. Maximum 5 requests per 1 minute(s).",
    "retryAfter": 45,
    "limit": 5,
    "windowMinutes": 1
  }
```

---

## ⚙️ التكوين (Configuration)

### **1. Global Rate Limit**
```json
{
  "RateLimiting": {
    "GlobalLimit": {
      "MaxRequests": 100,        // الحد الأقصى
      "TimeWindowMinutes": 1     // النافذة الزمنية (دقيقة)
    }
  }
}
```

**المعنى:** جميع الـ endpoints: 100 طلب في الدقيقة

---

### **2. Endpoint-Specific Limits**
```json
{
  "RateLimiting": {
    "EndpointLimits": {
      "/api/Account/login": {
        "MaxRequests": 5,         // 5 محاولات فقط
        "TimeWindowMinutes": 1    // في الدقيقة
      },
      "/api/Account/change-password": {
        "MaxRequests": 3,         // 3 محاولات فقط
        "TimeWindowMinutes": 5    // في 5 دقائق
      }
    }
  }
}
```

**المعنى:**
- Login: 5 محاولات/دقيقة (حماية من Brute Force)
- Change Password: 3 محاولات/5 دقائق (حماية إضافية)

---

### **3. IP Whitelist**
```json
{
  "RateLimiting": {
    "Whitelist": [ "192.168.1.100", "10.0.0.50" ]
  }
}
```

**المعنى:** هذه الـ IPs **لا تخضع** لـ Rate Limiting (تخطي كامل)

**الاستخدام:**
- خوادم داخلية
- Monitoring Tools
- Admin IPs

---

### **4. IP Blacklist**
```json
{
  "RateLimiting": {
    "Blacklist": [ "192.168.1.200" ]
  }
}
```

**المعنى:** هذه الـ IPs **ممنوعة تماماً** (403 Forbidden)

**الاستخدام:**
- IPs معروفة بالهجمات
- IPs مخالفة

---

### **5. Bypass Paths**
```json
{
  "RateLimiting": {
    "BypassPaths": [ "/swagger", "/health", "/api/Account/current-user" ]
  }
}
```

**المعنى:** هذه المسارات **لا تخضع** لـ Rate Limiting

**الاستخدام:**
- Swagger UI
- Health Checks
- Endpoints غير حساسة

---

## 🔍 كيف يتم تحديد العميل (Client Identification)?

### **الترتيب:**

1. **User ID** (إذا كان authenticated)
   ```
   Client ID = "user_123"
   ```
   - أفضل للتتبع
   - دقة أعلى
   - يعمل حتى مع تغيير IP

2. **X-Forwarded-For Header** (للـ proxies)
   ```
   X-Forwarded-For: 192.168.1.100, 10.0.0.1
   Client ID = "192.168.1.100" (أول IP)
   ```
   - مهم عند وجود Load Balancer
   - مهم عند وجود Reverse Proxy

3. **RemoteIpAddress** (fallback)
   ```
   Client ID = "192.168.1.100"
   ```
   - IP مباشر من الاتصال

---

## 📈 Response Headers

### **Headers المضافة في كل Response:**

```
X-RateLimit-Limit: 100          ← الحد الأقصى
X-RateLimit-Remaining: 50       ← الطلبات المتبقية
X-RateLimit-Reset: Mon, 01 Jan 2024 12:01:00 GMT  ← وقت إعادة التعيين
X-RateLimit-Used: 50            ← الطلبات المستخدمة
```

### **مثال Response:**

```http
HTTP/1.1 200 OK
X-RateLimit-Limit: 5
X-RateLimit-Remaining: 2
X-RateLimit-Reset: Mon, 01 Jan 2024 12:01:00 GMT
X-RateLimit-Used: 3

{
  "token": "...",
  "refreshToken": "..."
}
```

---

## 🚫 ماذا يحدث عند تجاوز الحد؟

### **Response:**
```http
HTTP/1.1 429 Too Many Requests
Content-Type: application/json

{
  "error": "Too many requests",
  "message": "Rate limit exceeded. Maximum 5 requests per 1 minute(s).",
  "retryAfter": 45,        ← الوقت المتبقي بالثواني
  "limit": 5,              ← الحد الأقصى
  "windowMinutes": 1        ← النافذة الزمنية
}
```

### **Logging:**
```
Rate limit exceeded - Client: user_123, IP: 192.168.1.100, Path: /api/Account/login, Requests: 6/5, Window: 1min
```

---

## 💾 كيف يتم التخزين؟

### **في الذاكرة (Memory):**
```csharp
ConcurrentDictionary<string, RateLimitInfo> _requestCounts

Key: "user_123:/api/account/login"
Value: {
  FirstRequest: 2024-01-01 12:00:00,
  RequestCount: 3,
  MaxRequests: 5,
  TimeWindow: 00:01:00
}
```

### **التنظيف التلقائي:**
- كل 5 دقائق يتم تنظيف entries أقدم من 10 دقائق
- تقليل استخدام الذاكرة
- منع Memory Leaks

---

## 🎯 أمثلة عملية

### **مثال 1: Login Endpoint**

```
العميل: IP 192.168.1.100
Endpoint: POST /api/Account/login
الحد: 5 requests/minute

الطلبات:
12:00:00 - Request #1 ✅
12:00:15 - Request #2 ✅
12:00:30 - Request #3 ✅
12:00:45 - Request #4 ✅
12:00:50 - Request #5 ✅
12:00:55 - Request #6 ❌ (429 Too Many Requests)

النتيجة: رفض الطلب #6
```

---

### **مثال 2: Change Password**

```
العميل: user_123
Endpoint: POST /api/Account/change-password
الحد: 3 requests/5 minutes

الطلبات:
12:00:00 - Request #1 ✅
12:02:00 - Request #2 ✅
12:04:00 - Request #3 ✅
12:05:00 - Request #4 ❌ (429 Too Many Requests)

النتيجة: رفض الطلب #4 (3 محاولات في 5 دقائق)
```

---

### **مثال 3: IP في Whitelist**

```
العميل: IP 192.168.1.100 (في Whitelist)
Endpoint: أي endpoint
الحد: لا ينطبق

الطلبات:
12:00:00 - Request #1 ✅ (تخطي Rate Limiting)
12:00:01 - Request #2 ✅ (تخطي Rate Limiting)
12:00:02 - Request #3 ✅ (تخطي Rate Limiting)
... (لا حدود)

النتيجة: جميع الطلبات مسموحة
```

---

### **مثال 4: IP في Blacklist**

```
العميل: IP 192.168.1.200 (في Blacklist)
Endpoint: أي endpoint

الطلبات:
12:00:00 - Request #1 ❌ (403 Forbidden - Access denied)

النتيجة: منع كامل للوصول
```

---

## 🔄 كيف يتم إعادة التعيين (Reset)?

### **Sliding Window:**

```
النافذة الزمنية: 1 دقيقة
الحد الأقصى: 5 requests

الطلبات:
12:00:00 - Request #1 (FirstRequest = 12:00:00)
12:00:15 - Request #2
12:00:30 - Request #3
12:00:45 - Request #4
12:00:50 - Request #5

12:01:00 - Request #6 ✅ (النافذة انتهت، إعادة تعيين)
         (FirstRequest = 12:01:00, RequestCount = 1)
```

**المعنى:** النافذة تنزلق مع الوقت

---

## 📊 Monitoring & Logging

### **معلومات مسجلة:**

1. **عند تجاوز الحد:**
   ```
   Rate limit exceeded - Client: user_123, IP: 192.168.1.100, 
   Path: /api/Account/login, Requests: 6/5, Window: 1min
   ```

2. **عند التنظيف:**
   ```
   Cleaned up 15 old rate limit entries
   ```

3. **عند Blacklist:**
   ```
   Blocked request from blacklisted IP: 192.168.1.200
   ```

---

## 🎛️ التحكم والتكوين

### **تفعيل/تعطيل:**
```json
{
  "RateLimiting": {
    "Enabled": true    // false لتعطيل Rate Limiting
  }
}
```

### **تغيير الحدود:**
```json
{
  "RateLimiting": {
    "GlobalLimit": {
      "MaxRequests": 200,        // زيادة الحد
      "TimeWindowMinutes": 2     // نافذة أكبر
    }
  }
}
```

---

## 🔐 الأمان

### **حماية من DDoS:**
- ✅ Global Limit (100 requests/minute)
- ✅ IP-based tracking
- ✅ Automatic cleanup

### **حماية من Brute Force:**
- ✅ Login: 5 attempts/minute
- ✅ Change Password: 3 attempts/5 minutes
- ✅ User ID tracking

### **حماية إضافية:**
- ✅ IP Blacklist (منع كامل)
- ✅ IP Whitelist (تخطي)
- ✅ Bypass Paths

---

## 📝 ملاحظات مهمة

1. **Memory-based:**
   - حالياً يستخدم Memory (ConcurrentDictionary)
   - في Production: استخدم Redis للـ Distributed Rate Limiting

2. **Thread-Safe:**
   - يستخدم ConcurrentDictionary
   - آمن للاستخدام في Multi-threaded environment

3. **Performance:**
   - Overhead قليل جداً
   - Lookup سريع
   - Cleanup تلقائي

---

## 🚀 Production Recommendations

1. **استخدام Redis:**
   ```csharp
   // Future enhancement
   // استخدام Redis للـ Distributed Rate Limiting
   // يعمل مع Multiple Servers
   ```

2. **Monitoring:**
   - مراقبة Rate Limit violations
   - تحديث Blacklist بناءً على Logs
   - Alerting عند هجمات

3. **Dynamic Limits:**
   - Rate Limits ديناميكية بناءً على Load
   - Rate Limits مختلفة لكل Role

---

**تاريخ الشرح:** $(Get-Date -Format "yyyy-MM-dd HH:mm")
**الحالة:** ✅ شرح شامل ومفصل
