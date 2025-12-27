# ملخص التحديثات - Angular Frontend

## التغييرات في .NET API

### التغييرات الرئيسية:
1. **FormRuleDto** الآن يرجع `Actions` و `ElseActions` كـ **Arrays** مباشرة (ليس JSON strings)
2. **CreateFormRuleDto** و **UpdateFormRuleDto** يقبلان `Actions` و `ElseActions` كـ **Arrays**

### قبل التحديث:
```csharp
// كان يرجع JSON strings
public string? ActionsJson { get; set; }
public string? ElseActionsJson { get; set; }
```

### بعد التحديث:
```csharp
// الآن يرجع Lists مباشرة
public List<ActionDataDto>? Actions { get; set; }
public List<ActionDataDto>? ElseActions { get; set; }
```

---

## التحديثات في Angular

### 1. تحديث Types (`frontend/src/types/formRules.ts`)

#### قبل:
```typescript
export interface CreateFormRuleDto {
  actionsJson?: string
  elseActionsJson?: string
  ruleJson?: string
}

export interface FormRuleDto {
  actionsJson?: string
  elseActionsJson?: string
  ruleJson?: string
}
```

#### بعد:
```typescript
export interface CreateFormRuleDto {
  actions?: Action[]      // ✅ Array مباشرة
  elseActions?: Action[]  // ✅ Array مباشرة
  // ❌ تم إزالة actionsJson, elseActionsJson, ruleJson
}

export interface FormRuleDto {
  actions?: Action[]      // ✅ Array مباشرة
  elseActions?: Action[]  // ✅ Array مباشرة
  // ❌ تم إزالة actionsJson, elseActionsJson, ruleJson
}
```

---

### 2. تحديث API Service (`frontend/src/services/api.ts`)

#### `convertFormRuleToDto()` - تم التحديث:
```typescript
// قبل: كان يرسل ruleJson كـ JSON string
ruleJson: JSON.stringify({...})

// بعد: يرسل Actions و ElseActions كـ Arrays مباشرة
actions: formRule.actions,
elseActions: formRule.elseActions,
conditionField: formRule.condition.field,
conditionOperator: formRule.condition.operator,
conditionValue: formRule.condition.value?.toString(),
conditionValueType: formRule.condition.valueType
```

#### `convertDtoToFormRule()` - تم التحديث:
```typescript
// قبل: كان يتحقق من actionsJson و elseActionsJson و ruleJson
if (dto.actionsJson) { actions = JSON.parse(dto.actionsJson) }

// بعد: يستخدم Arrays مباشرة
let actions: Action[] = dto.actions || []
let elseActions: Action[] = dto.elseActions || []
```

---

### 3. تحديث FormRulesList Component (`frontend/src/components/admin/FormRulesList.tsx`)

#### `handleSaveRule()` - تم التحديث:
```typescript
// قبل: كان ينشئ DTO يدوياً مع ruleJson
const updateDto = {
  ruleJson: JSON.stringify({...})
}

// بعد: يستخدم convertFormRuleToDto()
const createDto = ApiService.convertFormRuleToDto(rule, formBuilderId)
const updateDto: UpdateFormRuleDto = { ...createDto, isActive: rule.isActive }
```

#### `handleToggleActive()` - تم التحديث:
```typescript
// قبل: كان ينشئ DTO يدوياً مع ruleJson
const updateDto = { ruleJson: JSON.stringify({...}) }

// بعد: يستخدم convertFormRuleToDto()
const createDto = ApiService.convertFormRuleToDto(rule, formBuilderId)
const updateDto: UpdateFormRuleDto = { ...createDto, isActive: !rule.isActive }
```

---

## الملفات المحدثة

1. ✅ `frontend/src/types/formRules.ts` - تحديث Interfaces
2. ✅ `frontend/src/services/api.ts` - تحديث دوال التحويل
3. ✅ `frontend/src/components/admin/FormRulesList.tsx` - تحديث استخدام DTOs

---

## ملاحظات مهمة

1. **لا حاجة لـ JSON.parse/stringify** - البيانات الآن Arrays مباشرة
2. **التوافق مع .NET API** - الكود الآن متوافق تماماً مع الـ API الجديد
3. **أبسط وأسرع** - لا حاجة لتحويل JSON في كل مرة

---

## اختبار التحديثات

بعد التحديث، تأكد من:
- ✅ إنشاء Rule جديد يعمل
- ✅ تحديث Rule موجود يعمل
- ✅ عرض Rules يعمل بشكل صحيح
- ✅ Actions و ElseActions تظهر بشكل صحيح

---

## مثال على الاستخدام الجديد

```typescript
// إنشاء Rule جديد
const rule: FormRule = {
  ruleName: "Test Rule",
  condition: {
    field: "age",
    operator: ">",
    value: "18",
    valueType: "constant"
  },
  actions: [
    { type: "SetVisible", fieldCode: "drivingLicense", value: true }
  ],
  elseActions: [
    { type: "SetVisible", fieldCode: "drivingLicense", value: false }
  ],
  isActive: true,
  executionOrder: 1
}

// تحويل إلى DTO (يتم تلقائياً)
const dto = ApiService.convertFormRuleToDto(rule, formBuilderId)
// dto.actions = [{ type: "SetVisible", ... }] ✅ Array مباشرة
// dto.elseActions = [{ type: "SetVisible", ... }] ✅ Array مباشرة
```


