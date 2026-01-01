# Calculation Fields API Endpoints - Complete Reference

This document provides all API endpoints for testing Calculation Fields functionality with complete URL examples and JSON request/response samples.

## Base URL
```
http://localhost:5203/api
```

## Authentication
All endpoints require JWT Bearer token (except where noted). Include in header:
```
Authorization: Bearer {your_token}
```

---

## 1. Calculate Expression Endpoints

### 1.1 Calculate Expression (Simple)
**POST** `/api/Formulas/calculate-expression`

Calculate a simple expression with field values.

**Request JSON:**
```json
{
  "expressionText": "[RENT] * [MONTHS]",
  "fieldValues": {
    "RENT": 1000,
    "MONTHS": 12
  }
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": 12000,
  "statusCode": 200
}
```

**Example - Rent Calculation (from spec):**
```json
{
  "expressionText": "([RENT] * [MONTHS]) - [DISCOUNT]",
  "fieldValues": {
    "RENT": 1000,
    "MONTHS": 12,
    "DISCOUNT": 500
  }
}
```

**Response:**
```json
{
  "success": true,
  "data": 11500,
  "statusCode": 200
}
```

---

### 1.2 Calculate Expression (Safe)
**POST** `/api/Formulas/calculate-safe`

Safe calculation with error handling.

**Request JSON:**
```json
{
  "expressionText": "[RENT] + [MONTHS]",
  "fieldValues": {
    "RENT": 1000,
    "MONTHS": 12
  }
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": 1012,
  "statusCode": 200
}
```

**Example - Missing Field Values:**
```json
{
  "expressionText": "[RENT] + [MONTHS]",
  "fieldValues": {
    "RENT": 1000
  }
}
```

**Response:** (Missing fields default to 0)
```json
{
  "success": true,
  "data": 1000,
  "statusCode": 200
}
```

---

### 1.3 Calculate Expression (Advanced)
**POST** `/api/Formulas/calculate-advanced`

Advanced calculation with all operations support.

**Request JSON:**
```json
{
  "expressionText": "([price] * [quantity]) - ([discount] / 100 * [price] * [quantity]) + [shipping]",
  "fieldValues": {
    "price": 100,
    "quantity": 10,
    "discount": 10,
    "shipping": 50
  }
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": 950,
  "statusCode": 200
}
```

---

### 1.4 Calculate Formula by ID
**POST** `/api/Formulas/{formulaId}/calculate`

Calculate a saved formula by its ID.

**URL Example:**
```
POST /api/Formulas/5/calculate
```

**Request JSON:**
```json
{
  "RENT": 1000,
  "MONTHS": 12,
  "DISCOUNT": 500
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": 11500,
  "statusCode": 200
}
```

---

### 1.5 Batch Calculate Formulas
**POST** `/api/Formulas/form-builder/{formBuilderId}/batch-calculate`

Calculate all formulas for a form builder.

**URL Example:**
```
POST /api/Formulas/form-builder/1/batch-calculate
```

**Request JSON:**
```json
{
  "RENT": 1000,
  "MONTHS": 12,
  "DISCOUNT": 500,
  "TAX": 100
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "1": 11500,
    "2": 1200,
    "3": 500
  },
  "statusCode": 200
}
```

---

### 1.6 Preview Calculation
**POST** `/api/Formulas/preview-calculation`

Preview calculation result before saving.

**Request JSON:**
```json
{
  "expressionText": "[RENT] * [MONTHS]",
  "formBuilderId": 1,
  "fieldValues": {
    "RENT": 1000,
    "MONTHS": 12
  }
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "result": 12000,
    "expressionText": "[RENT] * [MONTHS]",
    "processedExpression": "1000 * 12"
  },
  "statusCode": 200
}
```

---

### 1.7 Test Formula with Sample Data
**GET** `/api/Formulas/{formulaId}/test-with-samples`

Test a formula with auto-generated sample data.

**URL Example:**
```
GET /api/Formulas/5/test-with-samples
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": 12000,
  "statusCode": 200
}
```

---

## 2. Validate Expression Endpoints

### 2.1 Validate Expression
**POST** `/api/Formulas/validate-expression`

Validate expression syntax and check if field codes exist.

**Request JSON:**
```json
{
  "expressionText": "[RENT] * [MONTHS] - [DISCOUNT]",
  "formBuilderId": 1
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "isValid": true,
    "validFieldCodes": ["RENT", "MONTHS", "DISCOUNT"],
    "invalidFieldCodes": [],
    "fieldDetails": [
      {
        "fieldId": 1,
        "fieldCode": "RENT",
        "fieldName": "Monthly Rent",
        "fieldType": "Decimal",
        "tabName": "Basic Info",
        "formBuilderId": 1,
        "formBuilderName": "Rental Form",
        "isActive": true
      },
      {
        "fieldId": 2,
        "fieldCode": "MONTHS",
        "fieldName": "Number of Months",
        "fieldType": "Integer",
        "tabName": "Basic Info",
        "formBuilderId": 1,
        "formBuilderName": "Rental Form",
        "isActive": true
      },
      {
        "fieldId": 3,
        "fieldCode": "DISCOUNT",
        "fieldName": "Discount Amount",
        "fieldType": "Decimal",
        "tabName": "Basic Info",
        "formBuilderId": 1,
        "formBuilderName": "Rental Form",
        "isActive": true
      }
    ]
  },
  "statusCode": 200
}
```

**Example - Invalid Field Codes:**
```json
{
  "expressionText": "[RENT] * [INVALID_FIELD]",
  "formBuilderId": 1
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "isValid": false,
    "validFieldCodes": ["RENT"],
    "invalidFieldCodes": ["INVALID_FIELD"],
    "fieldDetails": []
  },
  "statusCode": 200
}
```

---

### 2.2 Validate Expression with Details
**POST** `/api/Formulas/validate-expression-with-details`

Validate expression with detailed field information.

**Request JSON:**
```json
{
  "expressionText": "[RENT] * [MONTHS]",
  "formBuilderId": 1
}
```

**Response:** Same as validate-expression but with more detailed field information.

---

## 3. Formula CRUD Endpoints

### 3.1 Create Formula
**POST** `/api/Formulas`

Create a new formula.

**Request JSON:**
```json
{
  "formBuilderId": 1,
  "name": "Total Rent Calculation",
  "code": "TOTAL_RENT_CALC",
  "expressionText": "([RENT] * [MONTHS]) - [DISCOUNT]",
  "resultFieldId": 10,
  "isActive": true
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "data": {
    "id": 5,
    "formBuilderId": 1,
    "name": "Total Rent Calculation",
    "code": "TOTAL_RENT_CALC",
    "expressionText": "([RENT] * [MONTHS]) - [DISCOUNT]",
    "resultFieldId": 10,
    "isActive": true,
    "createdDate": "2024-01-15T10:30:00Z"
  },
  "statusCode": 201
}
```

---

### 3.2 Get Formula by ID
**GET** `/api/Formulas/{id}`

**URL Example:**
```
GET /api/Formulas/5
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 5,
    "formBuilderId": 1,
    "formBuilderName": "Rental Form",
    "name": "Total Rent Calculation",
    "code": "TOTAL_RENT_CALC",
    "expressionText": "([RENT] * [MONTHS]) - [DISCOUNT]",
    "resultFieldId": 10,
    "resultFieldName": "Total Rent",
    "resultFieldCode": "TOTAL_RENT",
    "isActive": true,
    "createdDate": "2024-01-15T10:30:00Z",
    "updatedDate": null,
    "variableCount": 3,
    "variables": [
      {
        "id": 1,
        "formulaId": 5,
        "variableName": "RENT",
        "sourceFieldId": 1,
        "fieldCode": "RENT",
        "fieldName": "Monthly Rent"
      },
      {
        "id": 2,
        "formulaId": 5,
        "variableName": "MONTHS",
        "sourceFieldId": 2,
        "fieldCode": "MONTHS",
        "fieldName": "Number of Months"
      },
      {
        "id": 3,
        "formulaId": 5,
        "variableName": "DISCOUNT",
        "sourceFieldId": 3,
        "fieldCode": "DISCOUNT",
        "fieldName": "Discount Amount"
      }
    ]
  },
  "statusCode": 200
}
```

---

### 3.3 Get Formulas by Form Builder
**GET** `/api/Formulas/form-builder/{formBuilderId}`

**URL Example:**
```
GET /api/Formulas/form-builder/1
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 5,
      "formBuilderId": 1,
      "name": "Total Rent Calculation",
      "code": "TOTAL_RENT_CALC",
      "expressionText": "([RENT] * [MONTHS]) - [DISCOUNT]",
      "resultFieldId": 10,
      "isActive": true
    }
  ],
  "statusCode": 200
}
```

---

### 3.4 Update Formula
**PUT** `/api/Formulas/{id}`

**URL Example:**
```
PUT /api/Formulas/5
```

**Request JSON:**
```json
{
  "name": "Updated Total Rent Calculation",
  "expressionText": "([RENT] * [MONTHS]) - [DISCOUNT] + [TAX]",
  "isActive": true
}
```

**Response (204 No Content)**

---

### 3.5 Delete Formula
**DELETE** `/api/Formulas/{id}`

**URL Example:**
```
DELETE /api/Formulas/5
```

**Response (204 No Content)**

---

## 4. Form Fields Endpoints (Calculated Fields)

### 4.1 Create Calculated Field
**POST** `/api/FormFields`

Create a calculated field.

**Request JSON:**
```json
{
  "fieldName": "Total Rent",
  "fieldCode": "TOTAL_RENT",
  "fieldTypeId": 15,
  "tabId": 1,
  "fieldOrder": 10,
  "expressionText": "([RENT] * [MONTHS]) - [DISCOUNT]",
  "calculationMode": "Expression",
  "recalculateOn": "OnFieldChange",
  "resultType": "Decimal",
  "isEditable": false,
  "isMandatory": false,
  "isVisible": true,
  "isActive": true
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "data": {
    "id": 10,
    "fieldName": "Total Rent",
    "fieldCode": "TOTAL_RENT",
    "fieldTypeId": 15,
    "fieldTypeName": "Calculated",
    "tabId": 1,
    "fieldOrder": 10,
    "expressionText": "([RENT] * [MONTHS]) - [DISCOUNT]",
    "calculationMode": "Expression",
    "recalculateOn": "OnFieldChange",
    "resultType": "Decimal",
    "isEditable": false,
    "isMandatory": false,
    "isVisible": true,
    "isActive": true
  },
  "statusCode": 201
}
```

**Important:** Calculated fields must have:
- `isEditable: false` (always non-editable)
- `isMandatory: false` (always non-mandatory)
- `isVisible: true` (can be hidden via visibility rules)
- `expressionText` is required

---

### 4.2 Update Calculated Field
**PUT** `/api/FormFields/{id}`

**URL Example:**
```
PUT /api/FormFields/10
```

**Request JSON:**
```json
{
  "fieldName": "Total Rent (Updated)",
  "fieldCode": "TOTAL_RENT",
  "fieldTypeId": 15,
  "tabId": 1,
  "fieldOrder": 10,
  "expressionText": "([RENT] * [MONTHS]) - [DISCOUNT] + [TAX]",
  "calculationMode": "Expression",
  "recalculateOn": "OnFieldChange",
  "resultType": "Decimal",
  "isEditable": false,
  "isMandatory": false,
  "isVisible": true,
  "isActive": true
}
```

**Response (204 No Content)**

---

### 4.3 Get Calculated Field by ID
**GET** `/api/FormFields/{id}`

**URL Example:**
```
GET /api/FormFields/10
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 10,
    "fieldName": "Total Rent",
    "fieldCode": "TOTAL_RENT",
    "fieldTypeId": 15,
    "fieldTypeName": "Calculated",
    "tabId": 1,
    "expressionText": "([RENT] * [MONTHS]) - [DISCOUNT]",
    "calculationMode": "Expression",
    "recalculateOn": "OnFieldChange",
    "resultType": "Decimal",
    "isEditable": false,
    "isMandatory": false,
    "isVisible": true,
    "isActive": true
  },
  "statusCode": 200
}
```

---

### 4.4 Get Fields by Form ID
**GET** `/api/FormFields/form/{formBuilderId}`

Get all fields (including calculated) for a form.

**URL Example:**
```
GET /api/FormFields/form/1
```

**Response:** Array of all fields including calculated fields.

---

## 5. Utility Endpoints

### 5.1 Get Referenced Field Codes
**GET** `/api/Formulas/{id}/referenced-field-codes`

Get all field codes referenced in a formula.

**URL Example:**
```
GET /api/Formulas/5/referenced-field-codes
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": ["RENT", "MONTHS", "DISCOUNT"],
  "statusCode": 200
}
```

---

### 5.2 Get Field Codes for Form
**GET** `/api/Formulas/form-builder/{formBuilderId}/field-codes`

Get all available field codes for a form builder.

**URL Example:**
```
GET /api/Formulas/form-builder/1/field-codes
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": ["RENT", "MONTHS", "DISCOUNT", "TAX", "TOTAL_RENT"],
  "statusCode": 200
}
```

---

### 5.3 Get Formula Statistics
**GET** `/api/Formulas/form-builder/{formBuilderId}/statistics`

Get statistics about formulas for a form builder.

**URL Example:**
```
GET /api/Formulas/form-builder/1/statistics
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "totalFormulas": 5,
    "activeFormulas": 4,
    "inactiveFormulas": 1,
    "formulasWithResultField": 3,
    "formulasWithoutResultField": 2,
    "totalVariables": 12,
    "averageVariablesPerFormula": 2.4
  },
  "statusCode": 200
}
```

---

## 6. Error Responses

### 400 Bad Request
```json
{
  "success": false,
  "errorMessage": "Expression text is required",
  "statusCode": 400
}
```

### 401 Unauthorized
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

### 404 Not Found
```json
{
  "success": false,
  "errorMessage": "Formula not found",
  "statusCode": 404
}
```

### 500 Internal Server Error
```json
{
  "success": false,
  "errorMessage": "Error calculating expression: Invalid syntax",
  "statusCode": 500
}
```

---

## 7. Complete Test Scenarios

### Scenario 1: Rent Calculation (From Spec)
```bash
# Step 1: Validate expression
POST /api/Formulas/validate-expression
{
  "expressionText": "([RENT] * [MONTHS]) - [DISCOUNT]",
  "formBuilderId": 1
}

# Step 2: Preview calculation
POST /api/Formulas/preview-calculation
{
  "expressionText": "([RENT] * [MONTHS]) - [DISCOUNT]",
  "formBuilderId": 1,
  "fieldValues": {
    "RENT": 1000,
    "MONTHS": 12,
    "DISCOUNT": 500
  }
}

# Step 3: Create calculated field
POST /api/FormFields
{
  "fieldName": "Total Rent",
  "fieldCode": "TOTAL_RENT",
  "fieldTypeId": 15,
  "tabId": 1,
  "expressionText": "([RENT] * [MONTHS]) - [DISCOUNT]",
  "calculationMode": "Expression",
  "recalculateOn": "OnFieldChange",
  "resultType": "Decimal",
  "isEditable": false,
  "isMandatory": false,
  "isVisible": true
}

# Step 4: Calculate with values
POST /api/Formulas/calculate-expression
{
  "expressionText": "([RENT] * [MONTHS]) - [DISCOUNT]",
  "fieldValues": {
    "RENT": 1000,
    "MONTHS": 12,
    "DISCOUNT": 500
  }
}
```

---

## 8. cURL Examples

### Calculate Expression
```bash
curl -X POST "http://localhost:5203/api/Formulas/calculate-expression" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "expressionText": "[RENT] * [MONTHS]",
    "fieldValues": {
      "RENT": 1000,
      "MONTHS": 12
    }
  }'
```

### Validate Expression
```bash
curl -X POST "http://localhost:5203/api/Formulas/validate-expression" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "expressionText": "[RENT] * [MONTHS]",
    "formBuilderId": 1
  }'
```

### Create Calculated Field
```bash
curl -X POST "http://localhost:5203/api/FormFields" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "fieldName": "Total Rent",
    "fieldCode": "TOTAL_RENT",
    "fieldTypeId": 15,
    "tabId": 1,
    "expressionText": "([RENT] * [MONTHS]) - [DISCOUNT]",
    "calculationMode": "Expression",
    "recalculateOn": "OnFieldChange",
    "resultType": "Decimal",
    "isEditable": false,
    "isMandatory": false,
    "isVisible": true
  }'
```

---

## Notes

1. **Field Codes Format**: Field codes in expressions must be wrapped in square brackets: `[FIELD_CODE]`
2. **Result Types**: Can be `Decimal`, `Integer`, or `Text`
3. **Recalculate On**: Can be `OnFieldChange`, `OnLoad`, or `OnSubmitOnly`
4. **Calculation Mode**: Currently supports `Expression` (Formula mode is for future use)
5. **All Calculated Fields**: Must be non-editable (`isEditable: false`) and non-mandatory (`isMandatory: false`)
6. **Authentication**: Most endpoints require `Administration` role

