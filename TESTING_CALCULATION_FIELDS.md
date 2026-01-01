# Testing Calculation Fields Endpoints

## Prerequisites

1. **Start the API**: Run the API project in Development mode
   ```bash
   dotnet run --project frombuilderApiProject
   ```
   The API will be available at: `http://localhost:5203`
   Swagger UI: `http://localhost:5203/swagger`

2. **Get Authentication Token**:
   - Use the login endpoint to get a JWT token
   - Update the `@token` variable in `frombuilderApiProject.http` file

## Testing Steps

### 1. Authentication
First, login to get a token:
```http
POST /api/Account/login
{
  "username": "your_username",
  "password": "your_password"
}
```
Copy the `token` from the response and update `@token` in the `.http` file.

### 2. Extract Dependent Fields
Test the endpoint that extracts field codes from expressions:

**Endpoint**: `POST /api/FormFields/extract-dependent-fields`

**Test Cases**:
- Simple expression: `[quantity] * [unitPrice] + [tax]`
- Complex expression: `([price] * [quantity]) - ([discount] / 100 * [price] * [quantity]) + [shipping]`
- With functions: `SUM([item1], [item2], [item3]) / [count]`

**Expected Response**:
```json
{
  "success": true,
  "data": ["quantity", "unitPrice", "tax"],
  "statusCode": 200
}
```

### 3. Create Calculated Field
**Endpoint**: `POST /api/FormFields`

**Required Fields**:
- `fieldName`: Display name
- `fieldCode`: Unique code (used in expressions)
- `fieldTypeId`: ID of "Calculated" field type (check database for correct ID)
- `tabId`: ID of the tab
- `fieldOrder`: Display order
- `expressionText`: Calculation expression (e.g., `[quantity] * [unitPrice]`)
- `calculationMode`: "Expression" or "Formula"
- `recalculateOn`: "OnFieldChange", "OnLoad", or "OnSubmitOnly"
- `resultType`: "Decimal", "Integer", or "Text"
- `isEditable`: Must be `false` for Calculated fields
- `isMandatory`: Must be `false` for Calculated fields

**Example Request**:
```json
{
  "fieldName": "Total Price",
  "fieldCode": "totalPrice",
  "fieldTypeId": 15,
  "tabId": 1,
  "fieldOrder": 10,
  "calculationMode": "Expression",
  "expressionText": "[quantity] * [unitPrice]",
  "recalculateOn": "OnFieldChange",
  "resultType": "Decimal",
  "isEditable": false,
  "isMandatory": false,
  "isVisible": true,
  "isActive": true
}
```

### 4. Update Calculated Field
**Endpoint**: `PUT /api/FormFields/{id}`

Update the expression or other properties of an existing Calculated field.

### 5. Get Calculated Fields
**Endpoints**:
- `GET /api/FormFields` - Get all fields
- `GET /api/FormFields/{id}` - Get field by ID
- `GET /api/FormFields/code/{fieldCode}` - Get field by code
- `GET /api/FormFields/form/{formBuilderId}` - Get fields by form ID
- `GET /api/FormFields/tab/{tabId}` - Get fields by tab ID

### 6. Validation Tests

Test that validation rules are enforced:

1. **Missing Expression**: Should fail with validation error
2. **Editable = true**: Should fail (Calculated fields must be non-editable)
3. **Mandatory = true**: Should fail (Calculated fields must be non-mandatory)
4. **Invalid Expression**: Should fail if expression references non-existent fields

## Using the .http File

The file `frombuilderApiProject.http` contains all test requests. To use it:

1. **In Visual Studio Code**: Install the "REST Client" extension
2. **In Visual Studio**: Use the built-in HTTP file support
3. **In Rider**: Built-in support for `.http` files

Click the "Send Request" link above each request to execute it.

## Important Notes

1. **Field Type ID**: Make sure to use the correct `fieldTypeId` for "Calculated" field type. Check the database:
   ```sql
   SELECT Id, TypeName FROM FIELD_TYPES WHERE TypeName = 'Calculated'
   ```

2. **Tab ID**: Ensure the `tabId` exists in your database and belongs to a valid form.

3. **Field Codes**: When creating Calculated fields, make sure the field codes referenced in `expressionText` exist in the same form.

4. **Dependent Fields**: Use the `extract-dependent-fields` endpoint to identify which fields are referenced in an expression before creating the Calculated field.

## Troubleshooting

- **401 Unauthorized**: Make sure you have a valid token and it's set in the `@token` variable
- **400 Bad Request**: Check that all required fields are provided and validation rules are met
- **404 Not Found**: Verify that IDs (tabId, fieldTypeId, etc.) exist in the database
- **500 Internal Server Error**: Check the API logs for detailed error messages

