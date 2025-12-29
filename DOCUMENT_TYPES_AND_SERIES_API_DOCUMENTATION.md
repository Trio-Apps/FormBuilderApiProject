# Document Types & Document Series API Documentation
## توثيق API لأنواع المستندات وسلسلة المستندات

---

## 📋 Table of Contents / جدول المحتويات
1. [Document Types API](#document-types-api)
2. [Document Series API](#document-series-api)
3. [Document Settings API (Combined)](#document-settings-api-combined)
4. [DTOs Structure](#dtos-structure)
5. [Angular Implementation Guide](#angular-implementation-guide)

---

## 🔷 Document Types API

### Base URL
```
/api/DocumentTypes
```

### Endpoints

#### 1. Get All Document Types
```http
GET /api/DocumentTypes
```
**Response:** `ServiceResult<IEnumerable<DocumentTypeDto>>`
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "name": "Lease Contract",
      "code": "LC",
      "formBuilderId": 5,
      "menuCaption": "Lease Contracts",
      "menuOrder": 1,
      "parentMenuId": null,
      "isActive": true,
      "formBuilderName": "Lease Form",
      "parentMenuName": null
    }
  ],
  "statusCode": 200
}
```

#### 2. Get Document Type by ID
```http
GET /api/DocumentTypes/{id}
```
**Response:** `ServiceResult<DocumentTypeDto>`

#### 3. Get Document Type by Code
```http
GET /api/DocumentTypes/code/{code}
```
**Example:** `GET /api/DocumentTypes/code/LC`

#### 4. Get Active Document Types
```http
GET /api/DocumentTypes/active
```
**Response:** `ServiceResult<IEnumerable<DocumentTypeDto>>`

#### 5. Get by Parent Menu ID
```http
GET /api/DocumentTypes/parent-menu/{parentMenuId}
```
**Get Root Items:**
```http
GET /api/DocumentTypes/parent-menu/null
```

#### 6. Create Document Type
```http
POST /api/DocumentTypes
Content-Type: application/json
```
**Request Body:**
```json
{
  "name": "Lease Contract",
  "code": "LC",
  "formBuilderId": 5,
  "menuCaption": "Lease Contracts",
  "menuOrder": 1,
  "parentMenuId": null,
  "isActive": true
}
```
**Response:** `201 Created` with location header

#### 7. Update Document Type
```http
PUT /api/DocumentTypes/{id}
Content-Type: application/json
```
**Request Body:**
```json
{
  "name": "Updated Name",
  "code": "LC",
  "menuCaption": "Updated Caption",
  "menuOrder": 2,
  "parentMenuId": null,
  "isActive": true
}
```
**Response:** `204 No Content` on success

#### 8. Delete Document Type
```http
DELETE /api/DocumentTypes/{id}
```
**Response:** `204 No Content` on success

#### 9. Toggle Active Status (via Service, not exposed in controller)
```http
PUT /api/DocumentTypes/{id}
```
**Request Body:**
```json
{
  "isActive": false
}
```

---

## 🔷 Document Series API

### Base URL
```
/api/DocumentSeries
```

**Note:** Requires `[Authorize(Roles = "Administration")]`

### Endpoints

#### 1. Get All Document Series
```http
GET /api/DocumentSeries
```
**Response:** `ApiResponse`
```json
{
  "statusCode": 200,
  "message": "Success",
  "data": [
    {
      "id": 1,
      "documentTypeId": 1,
      "documentTypeName": "Lease Contract",
      "projectId": 1,
      "projectName": "Project A",
      "seriesCode": "LC-AND1-2025",
      "nextNumber": 1,
      "isDefault": true,
      "isActive": true
    }
  ]
}
```

#### 2. Get Document Series by ID
```http
GET /api/DocumentSeries/{id}
```

#### 3. Get by Series Code
```http
GET /api/DocumentSeries/code/{seriesCode}
```
**Example:** `GET /api/DocumentSeries/code/LC-AND1-2025`

#### 4. Get by Document Type ID
```http
GET /api/DocumentSeries/document-type/{documentTypeId}
```

#### 5. Get by Project ID
```http
GET /api/DocumentSeries/project/{projectId}
```

#### 6. Get Active Series
```http
GET /api/DocumentSeries/active
```

#### 7. Get Default Series
```http
GET /api/DocumentSeries/default?documentTypeId={id}&projectId={id}
```

#### 8. Create Document Series
```http
POST /api/DocumentSeries
Content-Type: application/json
```
**Request Body:**
```json
{
  "documentTypeId": 1,
  "projectId": 1,
  "seriesCode": "LC-AND1-2025",
  "nextNumber": 1,
  "isDefault": true,
  "isActive": true
}
```
**Response:** `ApiResponse` with status code 200

#### 9. Update Document Series
```http
PUT /api/DocumentSeries/{id}
Content-Type: application/json
```
**Request Body:**
```json
{
  "seriesCode": "LC-AND1-2025-UPDATED",
  "nextNumber": 5,
  "isDefault": false,
  "isActive": true
}
```

#### 10. Delete Document Series
```http
DELETE /api/DocumentSeries/{id}
```

#### 11. Toggle Active Status
```http
PATCH /api/DocumentSeries/{id}/toggle-active
Content-Type: application/json
```
**Request Body:**
```json
true
```
or
```json
false
```

#### 12. Set as Default Series
```http
PATCH /api/DocumentSeries/{id}/set-default
```

#### 13. Get Next Number
```http
GET /api/DocumentSeries/{id}/next-number
```
**Response:**
```json
{
  "statusCode": 200,
  "message": "Next number retrieved successfully",
  "data": {
    "seriesId": 1,
    "seriesCode": "LC-AND1-2025",
    "nextNumber": 2,
    "fullNumber": "LC-AND1-2025-000002"
  }
}
```

#### 14. Check if Series Exists
```http
GET /api/DocumentSeries/{id}/exists
```

---

## 🔷 Document Settings API (Combined)

### Base URL
```
/api/FormBuilderDocumentSettings
```

**Note:** Requires `[Authorize(Roles = "Administration")]`

This API combines Document Type and Document Series management for a specific Form Builder.

### Endpoints

#### 1. Get Document Settings
```http
GET /api/FormBuilderDocumentSettings/form/{formBuilderId}
```
**Response:** `ServiceResult<DocumentSettingsDto>`
```json
{
  "success": true,
  "data": {
    "formBuilderId": 1,
    "formBuilderName": "Lease Form",
    "documentTypeId": 5,
    "documentName": "Lease Contract",
    "documentCode": "LC",
    "menuCaption": "Lease Contracts",
    "menuOrder": 1,
    "parentMenuId": null,
    "isActive": true,
    "documentSeries": [
      {
        "id": 1,
        "documentTypeId": 5,
        "documentTypeName": "Lease Contract",
        "projectId": 1,
        "projectName": "Project A",
        "seriesCode": "LC-AND1-2025",
        "nextNumber": 1,
        "isDefault": true,
        "isActive": true
      }
    ]
  },
  "statusCode": 200
}
```

#### 2. Save Document Settings
```http
POST /api/FormBuilderDocumentSettings
Content-Type: application/json
```
**Request Body:**
```json
{
  "formBuilderId": 1,
  "documentName": "Lease Contract",
  "documentCode": "LC",
  "menuCaption": "Lease Contracts",
  "menuOrder": 1,
  "parentMenuId": null,
  "isActive": true,
  "documentSeries": [
    {
      "id": 1,
      "projectId": 1,
      "seriesCode": "LC-AND1-2025",
      "nextNumber": 1,
      "isDefault": true,
      "isActive": true
    },
    {
      "projectId": 2,
      "seriesCode": "LC-AND2-2025",
      "nextNumber": 1,
      "isDefault": false,
      "isActive": true
    }
  ]
}
```
**Note:** 
- If `id` is provided in `documentSeries`, it will update the existing series
- If `id` is not provided, it will create a new series
- Document Type will be created or updated based on whether it exists for this FormBuilder

#### 3. Delete Document Settings
```http
DELETE /api/FormBuilderDocumentSettings/form/{formBuilderId}
```
**Response:** `204 No Content` on success

This deletes the Document Type and all associated Document Series.

---

## 📦 DTOs Structure

### DocumentTypeDto
```typescript
interface DocumentTypeDto {
  id: number;
  name: string;
  code: string;
  formBuilderId?: number;
  menuCaption: string;
  menuOrder: number;
  parentMenuId?: number;
  isActive: boolean;
  formBuilderName?: string;
  parentMenuName?: string;
}
```

### CreateDocumentTypeDto
```typescript
interface CreateDocumentTypeDto {
  name: string;
  code: string;
  formBuilderId?: number;
  menuCaption: string;
  menuOrder?: number; // default: 0
  parentMenuId?: number;
  isActive?: boolean; // default: true
}
```

### UpdateDocumentTypeDto
```typescript
interface UpdateDocumentTypeDto {
  name?: string;
  code?: string;
  formBuilderId?: number;
  menuCaption?: string;
  menuOrder?: number;
  parentMenuId?: number;
  isActive?: boolean;
}
```

### DocumentSeriesDto
```typescript
interface DocumentSeriesDto {
  id: number;
  documentTypeId: number;
  documentTypeName?: string;
  projectId: number;
  projectName?: string;
  seriesCode: string;
  nextNumber: number;
  isDefault: boolean;
  isActive: boolean;
}
```

### CreateDocumentSeriesDto
```typescript
interface CreateDocumentSeriesDto {
  documentTypeId: number; // Required
  projectId: number; // Required
  seriesCode: string; // Required, max 50 chars
  nextNumber?: number; // default: 1
  isDefault?: boolean; // default: false
  isActive?: boolean; // default: true
}
```

### UpdateDocumentSeriesDto
```typescript
interface UpdateDocumentSeriesDto {
  documentTypeId?: number;
  projectId?: number;
  seriesCode?: string; // max 50 chars
  nextNumber?: number;
  isDefault?: boolean;
  isActive?: boolean;
}
```

### DocumentSeriesNumberDto
```typescript
interface DocumentSeriesNumberDto {
  seriesId: number;
  seriesCode: string;
  nextNumber: number;
  fullNumber: string; // e.g., "LC-AND1-2025-000002"
}
```

### ServiceResult<T>
```typescript
interface ServiceResult<T> {
  success: boolean;
  data?: T;
  message?: string;
  errorMessage?: string;
  statusCode: number;
}
```

### ApiResponse
```typescript
interface ApiResponse {
  statusCode: number;
  message: string;
  data?: any;
  errors?: any;
}
```

### DocumentSettingsDto
```typescript
interface DocumentSettingsDto {
  formBuilderId: number;
  formBuilderName: string;
  documentTypeId?: number;
  documentName: string;
  documentCode: string;
  menuCaption: string;
  menuOrder: number;
  parentMenuId?: number;
  isActive: boolean;
  documentSeries: DocumentSeriesDto[];
}
```

### SaveDocumentSettingsDto
```typescript
interface SaveDocumentSettingsDto {
  formBuilderId: number; // Required
  documentName: string; // Required, max 200 chars
  documentCode: string; // Required, max 100 chars
  menuCaption: string; // Required, max 200 chars
  menuOrder?: number; // default: 0
  parentMenuId?: number;
  isActive?: boolean; // default: true
  documentSeries: SaveDocumentSeriesDto[];
}

interface SaveDocumentSeriesDto {
  id?: number; // If provided, update existing; otherwise create new
  projectId: number; // Required
  seriesCode: string; // Required, max 50 chars
  nextNumber?: number; // default: 1
  isDefault?: boolean; // default: false
  isActive?: boolean; // default: true
}
```

---

## 🅰️ Angular Implementation Guide

### 1. Create Service Files

#### document-type.service.ts
```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface DocumentTypeDto {
  id: number;
  name: string;
  code: string;
  formBuilderId?: number;
  menuCaption: string;
  menuOrder: number;
  parentMenuId?: number;
  isActive: boolean;
  formBuilderName?: string;
  parentMenuName?: string;
}

export interface CreateDocumentTypeDto {
  name: string;
  code: string;
  formBuilderId?: number;
  menuCaption: string;
  menuOrder?: number;
  parentMenuId?: number;
  isActive?: boolean;
}

export interface UpdateDocumentTypeDto {
  name?: string;
  code?: string;
  formBuilderId?: number;
  menuCaption?: string;
  menuOrder?: number;
  parentMenuId?: number;
  isActive?: boolean;
}

export interface ServiceResult<T> {
  success: boolean;
  data?: T;
  message?: string;
  errorMessage?: string;
  statusCode: number;
}

@Injectable({
  providedIn: 'root'
})
export class DocumentTypeService {
  private apiUrl = `${environment.apiUrl}/api/DocumentTypes`;

  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token') || localStorage.getItem('accessToken');
    let headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });
    
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }
    
    return headers;
  }

  getAll(): Observable<ServiceResult<DocumentTypeDto[]>> {
    return this.http.get<ServiceResult<DocumentTypeDto[]>>(this.apiUrl, {
      headers: this.getHeaders()
    });
  }

  getById(id: number): Observable<ServiceResult<DocumentTypeDto>> {
    return this.http.get<ServiceResult<DocumentTypeDto>>(`${this.apiUrl}/${id}`, {
      headers: this.getHeaders()
    });
  }

  getByCode(code: string): Observable<ServiceResult<DocumentTypeDto>> {
    return this.http.get<ServiceResult<DocumentTypeDto>>(`${this.apiUrl}/code/${code}`, {
      headers: this.getHeaders()
    });
  }

  getActive(): Observable<ServiceResult<DocumentTypeDto[]>> {
    return this.http.get<ServiceResult<DocumentTypeDto[]>>(`${this.apiUrl}/active`, {
      headers: this.getHeaders()
    });
  }

  getByParentMenuId(parentMenuId: number | null): Observable<ServiceResult<DocumentTypeDto[]>> {
    const url = parentMenuId === null 
      ? `${this.apiUrl}/parent-menu/null`
      : `${this.apiUrl}/parent-menu/${parentMenuId}`;
    return this.http.get<ServiceResult<DocumentTypeDto[]>>(url, {
      headers: this.getHeaders()
    });
  }

  create(dto: CreateDocumentTypeDto): Observable<DocumentTypeDto> {
    return this.http.post<DocumentTypeDto>(this.apiUrl, dto, {
      headers: this.getHeaders()
    });
  }

  update(id: number, dto: UpdateDocumentTypeDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto, {
      headers: this.getHeaders()
    });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, {
      headers: this.getHeaders()
    });
  }
}
```

#### document-series.service.ts
```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface DocumentSeriesDto {
  id: number;
  documentTypeId: number;
  documentTypeName?: string;
  projectId: number;
  projectName?: string;
  seriesCode: string;
  nextNumber: number;
  isDefault: boolean;
  isActive: boolean;
}

export interface CreateDocumentSeriesDto {
  documentTypeId: number;
  projectId: number;
  seriesCode: string;
  nextNumber?: number;
  isDefault?: boolean;
  isActive?: boolean;
}

export interface UpdateDocumentSeriesDto {
  documentTypeId?: number;
  projectId?: number;
  seriesCode?: string;
  nextNumber?: number;
  isDefault?: boolean;
  isActive?: boolean;
}

export interface DocumentSeriesNumberDto {
  seriesId: number;
  seriesCode: string;
  nextNumber: number;
  fullNumber: string;
}

export interface ApiResponse {
  statusCode: number;
  message: string;
  data?: any;
  errors?: any;
}

@Injectable({
  providedIn: 'root'
})
export class DocumentSeriesService {
  private apiUrl = `${environment.apiUrl}/api/DocumentSeries`;

  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token') || localStorage.getItem('accessToken');
    let headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });
    
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }
    
    return headers;
  }

  getAll(): Observable<ApiResponse> {
    return this.http.get<ApiResponse>(this.apiUrl, {
      headers: this.getHeaders()
    });
  }

  getById(id: number): Observable<ApiResponse> {
    return this.http.get<ApiResponse>(`${this.apiUrl}/${id}`, {
      headers: this.getHeaders()
    });
  }

  getBySeriesCode(seriesCode: string): Observable<ApiResponse> {
    return this.http.get<ApiResponse>(`${this.apiUrl}/code/${seriesCode}`, {
      headers: this.getHeaders()
    });
  }

  getByDocumentTypeId(documentTypeId: number): Observable<ApiResponse> {
    return this.http.get<ApiResponse>(`${this.apiUrl}/document-type/${documentTypeId}`, {
      headers: this.getHeaders()
    });
  }

  getByProjectId(projectId: number): Observable<ApiResponse> {
    return this.http.get<ApiResponse>(`${this.apiUrl}/project/${projectId}`, {
      headers: this.getHeaders()
    });
  }

  getActive(): Observable<ApiResponse> {
    return this.http.get<ApiResponse>(`${this.apiUrl}/active`, {
      headers: this.getHeaders()
    });
  }

  getDefaultSeries(documentTypeId: number, projectId: number): Observable<ApiResponse> {
    const params = new HttpParams()
      .set('documentTypeId', documentTypeId.toString())
      .set('projectId', projectId.toString());
    
    return this.http.get<ApiResponse>(`${this.apiUrl}/default`, {
      headers: this.getHeaders(),
      params
    });
  }

  create(dto: CreateDocumentSeriesDto): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(this.apiUrl, dto, {
      headers: this.getHeaders()
    });
  }

  update(id: number, dto: UpdateDocumentSeriesDto): Observable<ApiResponse> {
    return this.http.put<ApiResponse>(`${this.apiUrl}/${id}`, dto, {
      headers: this.getHeaders()
    });
  }

  delete(id: number): Observable<ApiResponse> {
    return this.http.delete<ApiResponse>(`${this.apiUrl}/${id}`, {
      headers: this.getHeaders()
    });
  }

  toggleActive(id: number, isActive: boolean): Observable<ApiResponse> {
    return this.http.patch<ApiResponse>(`${this.apiUrl}/${id}/toggle-active`, isActive, {
      headers: this.getHeaders()
    });
  }

  setAsDefault(id: number): Observable<ApiResponse> {
    return this.http.patch<ApiResponse>(`${this.apiUrl}/${id}/set-default`, null, {
      headers: this.getHeaders()
    });
  }

  getNextNumber(id: number): Observable<ApiResponse> {
    return this.http.get<ApiResponse>(`${this.apiUrl}/${id}/next-number`, {
      headers: this.getHeaders()
    });
  }

  exists(id: number): Observable<ApiResponse> {
    return this.http.get<ApiResponse>(`${this.apiUrl}/${id}/exists`, {
      headers: this.getHeaders()
    });
  }
}
```

### 2. Example Component Usage

#### document-types.component.ts
```typescript
import { Component, OnInit } from '@angular/core';
import { DocumentTypeService, DocumentTypeDto, CreateDocumentTypeDto, UpdateDocumentTypeDto } from './document-type.service';

@Component({
  selector: 'app-document-types',
  templateUrl: './document-types.component.html',
  styleUrls: ['./document-types.component.css']
})
export class DocumentTypesComponent implements OnInit {
  documentTypes: DocumentTypeDto[] = [];
  loading = false;
  error: string | null = null;
  showModal = false;
  editingId: number | null = null;
  
  // Form fields
  form: CreateDocumentTypeDto = {
    name: '',
    code: '',
    menuCaption: '',
    menuOrder: 0,
    isActive: true
  };

  constructor(private documentTypeService: DocumentTypeService) {}

  ngOnInit(): void {
    this.loadDocumentTypes();
  }

  loadDocumentTypes(): void {
    this.loading = true;
    this.error = null;
    
    this.documentTypeService.getAll().subscribe({
      next: (result) => {
        if (result.success && result.data) {
          this.documentTypes = result.data;
        }
        this.loading = false;
      },
      error: (err) => {
        this.error = err.error?.errorMessage || 'Failed to load document types';
        this.loading = false;
      }
    });
  }

  openCreateModal(): void {
    this.editingId = null;
    this.form = {
      name: '',
      code: '',
      menuCaption: '',
      menuOrder: 0,
      isActive: true
    };
    this.showModal = true;
  }

  openEditModal(docType: DocumentTypeDto): void {
    this.editingId = docType.id;
    this.form = {
      name: docType.name,
      code: docType.code,
      formBuilderId: docType.formBuilderId,
      menuCaption: docType.menuCaption,
      menuOrder: docType.menuOrder,
      parentMenuId: docType.parentMenuId,
      isActive: docType.isActive
    };
    this.showModal = true;
  }

  save(): void {
    if (!this.form.name || !this.form.code || !this.form.menuCaption) {
      this.error = 'Name, Code, and Menu Caption are required';
      return;
    }

    this.loading = true;
    this.error = null;

    if (this.editingId) {
      const updateDto: UpdateDocumentTypeDto = this.form;
      this.documentTypeService.update(this.editingId, updateDto).subscribe({
        next: () => {
          this.loadDocumentTypes();
          this.showModal = false;
          this.loading = false;
        },
        error: (err) => {
          this.error = err.error?.errorMessage || 'Failed to update document type';
          this.loading = false;
        }
      });
    } else {
      this.documentTypeService.create(this.form).subscribe({
        next: () => {
          this.loadDocumentTypes();
          this.showModal = false;
          this.loading = false;
        },
        error: (err) => {
          this.error = err.error?.errorMessage || 'Failed to create document type';
          this.loading = false;
        }
      });
    }
  }

  delete(id: number): void {
    if (!confirm('Are you sure you want to delete this document type?')) {
      return;
    }

    this.loading = true;
    this.documentTypeService.delete(id).subscribe({
      next: () => {
        this.loadDocumentTypes();
        this.loading = false;
      },
      error: (err) => {
        this.error = err.error?.errorMessage || 'Failed to delete document type';
        this.loading = false;
      }
    });
  }
}
```

### 3. Error Handling

```typescript
// Handle ServiceResult response
this.documentTypeService.getAll().subscribe({
  next: (result: ServiceResult<DocumentTypeDto[]>) => {
    if (result.success && result.data) {
      // Success
      this.documentTypes = result.data;
    } else {
      // Error from service
      this.error = result.errorMessage || result.message || 'Unknown error';
    }
  },
  error: (httpError) => {
    // HTTP error
    if (httpError.error?.errorMessage) {
      this.error = httpError.error.errorMessage;
    } else if (httpError.error?.message) {
      this.error = httpError.error.message;
    } else {
      this.error = 'An error occurred';
    }
  }
});
```

### 4. Document Settings Service Example

#### document-settings.service.ts
```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface DocumentSettingsDto {
  formBuilderId: number;
  formBuilderName: string;
  documentTypeId?: number;
  documentName: string;
  documentCode: string;
  menuCaption: string;
  menuOrder: number;
  parentMenuId?: number;
  isActive: boolean;
  documentSeries: DocumentSeriesDto[];
}

export interface SaveDocumentSettingsDto {
  formBuilderId: number;
  documentName: string;
  documentCode: string;
  menuCaption: string;
  menuOrder?: number;
  parentMenuId?: number;
  isActive?: boolean;
  documentSeries: SaveDocumentSeriesDto[];
}

export interface SaveDocumentSeriesDto {
  id?: number;
  projectId: number;
  seriesCode: string;
  nextNumber?: number;
  isDefault?: boolean;
  isActive?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class DocumentSettingsService {
  private apiUrl = `${environment.apiUrl}/api/FormBuilderDocumentSettings`;

  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token') || localStorage.getItem('accessToken');
    let headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });
    
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }
    
    return headers;
  }

  getDocumentSettings(formBuilderId: number): Observable<ServiceResult<DocumentSettingsDto>> {
    return this.http.get<ServiceResult<DocumentSettingsDto>>(
      `${this.apiUrl}/form/${formBuilderId}`,
      { headers: this.getHeaders() }
    );
  }

  saveDocumentSettings(dto: SaveDocumentSettingsDto): Observable<ServiceResult<DocumentSettingsDto>> {
    return this.http.post<ServiceResult<DocumentSettingsDto>>(
      this.apiUrl,
      dto,
      { headers: this.getHeaders() }
    );
  }

  deleteDocumentSettings(formBuilderId: number): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/form/${formBuilderId}`,
      { headers: this.getHeaders() }
    );
  }
}
```

### 5. Important Notes

1. **Authentication**: 
   - DocumentSeries endpoints require `Administration` role
   - DocumentSettings endpoints require `Administration` role
   - DocumentTypes endpoints are currently not protected (commented out)

2. **Response Types**: 
   - DocumentTypes returns `ServiceResult<T>`
   - DocumentSeries returns `ApiResponse`
   - DocumentSettings returns `ServiceResult<DocumentSettingsDto>`

3. **Default Series**: Only one series can be default per document type + project combination

4. **Next Number**: Automatically formatted as `{SeriesCode}-{NextNumber:D6}`

5. **Parent Menu**: Use `null` or `0` for root menu items

6. **Document Settings**: 
   - Combines Document Type and Series management
   - Automatically creates/updates Document Type based on FormBuilderId
   - Manages multiple Document Series in one request
   - Series with `id` are updated, without `id` are created

---

## 📝 Summary

### Document Types Endpoints: 9
- GET: 5 endpoints
- POST: 1 endpoint
- PUT: 1 endpoint
- DELETE: 1 endpoint
- Special: 1 (parent-menu)

### Document Series Endpoints: 14
- GET: 8 endpoints
- POST: 1 endpoint
- PUT: 1 endpoint
- DELETE: 1 endpoint
- PATCH: 2 endpoints
- Special: 1 (next-number)

### Document Settings Endpoints: 3
- GET: 1 endpoint (get by formBuilderId)
- POST: 1 endpoint (save/create/update)
- DELETE: 1 endpoint (delete by formBuilderId)

### Key Features
✅ Full CRUD operations
✅ Active/Inactive status management
✅ Default series management
✅ Next number generation
✅ Filtering by document type, project, code
✅ Menu hierarchy support

