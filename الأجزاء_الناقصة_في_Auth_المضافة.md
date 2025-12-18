# الأجزاء الناقصة في موديل Auth - تم إضافتها ✅

## 📋 ملخص الأجزاء المضافة

تم إضافة جميع الأجزاء الناقصة في موديل Authentication لتحسين الوظائف والأمان.

---

## ✅ الأجزاء المضافة

### 1. **Endpoint لـ RevokeAllUserTokensAsync** ✅
**الوصف:** إضافة endpoint لإلغاء جميع tokens الخاصة بمستخدم معين (للمسؤولين فقط)

**الملف:** `frombuilderApiProject/Controllers/Auth/AccountController.cs`

```csharp
[HttpPost("revoke-all-tokens/{userId}")]
[Authorize(Roles = "Administration")]
public async Task<IActionResult> RevokeAllUserTokens(int userId, CancellationToken cancellationToken)
```

**الاستخدام:**
- `POST /api/Account/revoke-all-tokens/{userId}`
- يتطلب صلاحيات Administration
- يلغي جميع refresh tokens للمستخدم المحدد

---

### 2. **معلومات إضافية في LoginResponseDto** ✅
**الوصف:** إضافة معلومات المستخدم في response الـ Login

**الملف:** `FormBuilder.Core/DTOS/account/LoginRequestDto.cs`

**الحقول المضافة:**
- `UserId` - معرف المستخدم
- `Username` - اسم المستخدم
- `Email` - البريد الإلكتروني
- `Name` - الاسم الكامل

**قبل:**
```csharp
public class LoginResponseDto
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public string? Role { get; set; }
    // ...
}
```

**بعد:**
```csharp
public class LoginResponseDto
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public string? Role { get; set; }
    public int? UserId { get; set; }        // ✅ جديد
    public string? Username { get; set; }   // ✅ جديد
    public string? Email { get; set; }      // ✅ جديد
    public string? Name { get; set; }      // ✅ جديد
    // ...
}
```

---

### 3. **GetCurrentUser Endpoint** ✅
**الوصف:** endpoint للحصول على معلومات المستخدم الحالي من الـ token

**الملف:** `frombuilderApiProject/Controllers/Auth/AccountController.cs`

```csharp
[HttpGet("current-user")]
[Authorize]
public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
```

**الاستخدام:**
- `GET /api/Account/current-user`
- يتطلب authentication
- يعيد معلومات المستخدم من الـ JWT token

**Response:**
```json
{
  "id": 1,
  "username": "admin",
  "name": "Administrator",
  "email": "admin@example.com",
  "phone": "1234567890",
  "role": "Administration",
  "isActive": true
}
```

---

### 4. **ChangePassword Endpoint** ✅
**الوصف:** endpoint لتغيير كلمة مرور المستخدم

**الملف:** `frombuilderApiProject/Controllers/Auth/AccountController.cs`

```csharp
[HttpPost("change-password")]
[Authorize]
public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request, CancellationToken cancellationToken)
```

**الاستخدام:**
- `POST /api/Account/change-password`
- يتطلب authentication
- يغير كلمة المرور ويلغي جميع الجلسات الحالية

**Request Body:**
```json
{
  "currentPassword": "oldPassword123",
  "newPassword": "newPassword456",
  "confirmPassword": "newPassword456"
}
```

**الأمان:**
- بعد تغيير كلمة المرور، يتم إلغاء جميع refresh tokens تلقائياً
- يتطلب كلمة المرور الحالية للتحقق

---

### 5. **UserInfoDto** ✅
**الوصف:** DTO جديد لمعلومات المستخدم

**الملف:** `FormBuilder.Core/DTOS/account/LoginRequestDto.cs`

```csharp
public class UserInfoDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Role { get; set; }
    public bool IsActive { get; set; }
}
```

---

### 6. **ChangePasswordRequestDto** ✅
**الوصف:** DTO لطلب تغيير كلمة المرور مع validation

**الملف:** `FormBuilder.Core/DTOS/account/LoginRequestDto.cs`

```csharp
public class ChangePasswordRequestDto
{
    [Required(ErrorMessage = "Current password is required.")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "New password is required.")]
    [MinLength(6, ErrorMessage = "New password must be at least 6 characters long.")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm password is required.")]
    [Compare("NewPassword", ErrorMessage = "New password and confirm password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
```

**Validation:**
- CurrentPassword: مطلوب
- NewPassword: مطلوب، على الأقل 6 أحرف
- ConfirmPassword: يجب أن يطابق NewPassword

---

### 7. **GetCurrentUserAsync في AuthService** ✅
**الوصف:** method في AuthService للحصول على معلومات المستخدم

**الملف:** `FormBuilder.Services/Services/account/AuthService.cs`

```csharp
public async Task<UserInfoDto?> GetCurrentUserAsync(int userId, CancellationToken cancellationToken)
{
    var user = await _identityContext.TblUsers
        .Include(u => u.TblUserGroupUsers)
            .ThenInclude(ugu => ugu.IdUserGroupNavigation)
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive, cancellationToken);
    
    // ... إرجاع UserInfoDto
}
```

**الأداء:**
- يستخدم `Include` لتجنب N+1 Query
- يستخدم `AsNoTracking` للقراءة فقط

---

### 8. **ChangePasswordAsync في AuthService** ✅
**الوصف:** method في AuthService لتغيير كلمة المرور

**الملف:** `FormBuilder.Services/Services/account/AuthService.cs`

```csharp
public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken cancellationToken)
{
    // التحقق من كلمة المرور الحالية
    // تحديث كلمة المرور
    // إلغاء جميع tokens للمستخدم
}
```

**الأمان:**
- التحقق من كلمة المرور الحالية
- تشفير كلمة المرور الجديدة بـ SHA512
- إلغاء جميع refresh tokens بعد تغيير كلمة المرور

---

## 📁 الملفات المعدلة

1. ✅ `FormBuilder.Core/IServices/Auth/IAuthService.cs`
   - إضافة `GetCurrentUserAsync`
   - إضافة `ChangePasswordAsync`

2. ✅ `FormBuilder.Core/DTOS/account/LoginRequestDto.cs`
   - تحديث `LoginResponseDto` (إضافة UserId, Username, Email, Name)
   - إضافة `UserInfoDto`
   - إضافة `ChangePasswordRequestDto`

3. ✅ `FormBuilder.Services/Services/account/AuthService.cs`
   - تحديث `LoginAsync` (إضافة معلومات المستخدم في response)
   - إضافة `GetCurrentUserAsync`
   - إضافة `ChangePasswordAsync`

4. ✅ `frombuilderApiProject/Controllers/Auth/AccountController.cs`
   - إضافة `RevokeAllUserTokens` endpoint
   - إضافة `GetCurrentUser` endpoint
   - إضافة `ChangePassword` endpoint

---

## 🔐 Endpoints الجديدة

| Method | Endpoint | Auth Required | Role Required | الوصف |
|--------|----------|---------------|---------------|-------|
| POST | `/api/Account/revoke-all-tokens/{userId}` | ✅ | Administration | إلغاء جميع tokens لمستخدم |
| GET | `/api/Account/current-user` | ✅ | - | الحصول على معلومات المستخدم الحالي |
| POST | `/api/Account/change-password` | ✅ | - | تغيير كلمة المرور |

---

## ✅ التحقق من الأخطاء

- ✅ لا توجد أخطاء في الكود
- ✅ لا توجد تحذيرات
- ✅ جميع التغييرات متوافقة مع الكود الحالي
- ✅ Validation يعمل بشكل صحيح

---

## 🎯 الميزات المضافة

1. **إدارة Tokens:**
   - إلغاء جميع tokens لمستخدم (للمسؤولين)
   - إلغاء تلقائي للـ tokens بعد تغيير كلمة المرور

2. **معلومات المستخدم:**
   - معلومات المستخدم في Login response
   - endpoint للحصول على معلومات المستخدم الحالي

3. **تغيير كلمة المرور:**
   - endpoint آمن لتغيير كلمة المرور
   - validation شامل
   - إلغاء تلقائي للجلسات بعد التغيير

---

## 📝 ملاحظات

- جميع الأجزاء المضافة متوافقة مع الكود الحالي
- لا توجد breaking changes
- الكود جاهز للاستخدام الفوري
- تم استخدام أفضل الممارسات في الأمان والأداء

---

**تاريخ الإضافة:** $(Get-Date -Format "yyyy-MM-dd HH:mm")
**الحالة:** ✅ مكتمل وجاهز للاستخدام
