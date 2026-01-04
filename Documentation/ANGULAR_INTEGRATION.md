# دليل تكامل نظام الحساب مع Angular 📘

## 📋 المتطلبات الأساسية

### 1. Service للاتصال بـ Backend API

أنشئ ملف `calculation.service.ts`:

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

export interface CalculateExpressionRequest {
  expressionText: string;
  fieldValues: { [key: string]: any };
}

@Injectable({
  providedIn: 'root'
})
export class CalculationService {
  private apiUrl = '/api/Formulas';

  constructor(private http: HttpClient) {}

  calculateSafe(request: CalculateExpressionRequest): Observable<number> {
    return this.http.post<number | any>(
      `${this.apiUrl}/calculate-safe`,
      request
    ).pipe(
      map(response => {
        if (typeof response === 'number') {
          return response;
        }
        if (response?.success && typeof response.data === 'number') {
          return response.data;
        }
        throw new Error(response?.message || 'Calculation failed');
      }),
      catchError(error => {
        throw new Error(error.error?.message || 'حدث خطأ أثناء الحساب');
      })
    );
  }
}
```

### 2. Module Configuration

في `app.module.ts`:

```typescript
import { HttpClientModule } from '@angular/common/http';

@NgModule({
  imports: [
    HttpClientModule, // مهم جداً!
  ],
  providers: [
    CalculationService,
  ],
})
export class AppModule { }
```

### 3. استخدام في Component

```typescript
import { Component } from '@angular/core';
import { CalculationService } from './services/calculation.service';

@Component({
  selector: 'app-calculator',
  template: `
    <div>
      <input [(ngModel)]="expression" placeholder="MAX([N1], [N2], [N3])">
      <button (click)="calculate()">احسب</button>
      <div *ngIf="result !== null">النتيجة: {{ result }}</div>
    </div>
  `
})
export class CalculatorComponent {
  expression = 'MAX([N1], [N2], [N3])';
  result: number | null = null;

  constructor(private calculationService: CalculationService) {}

  calculate(): void {
    this.calculationService.calculateSafe({
      expressionText: this.expression,
      fieldValues: { N1: 1, N2: 23, N3: 3 }
    }).subscribe({
      next: (result) => this.result = result,
      error: (err) => console.error(err)
    });
  }
}
```

## 🎯 أمثلة استخدام

### MAX
```typescript
this.calculationService.calculateSafe({
  expressionText: 'MAX([N1], [N2], [N3])',
  fieldValues: { N1: 1, N2: 23, N3: 3 }
}).subscribe(result => console.log(result)); // 23
```

### MIN
```typescript
this.calculationService.calculateSafe({
  expressionText: 'MIN([N1], [N2], [N3])',
  fieldValues: { N1: 1, N2: 23, N3: 3 }
}).subscribe(result => console.log(result)); // 1
```

### SUM
```typescript
this.calculationService.calculateSafe({
  expressionText: 'SUM([A], [B], [C])',
  fieldValues: { A: 10, B: 20, C: 30 }
}).subscribe(result => console.log(result)); // 60
```

## ✅ Checklist

- [ ] إنشاء CalculationService
- [ ] إضافة HttpClientModule
- [ ] اختبار جميع الدوال
- [ ] إضافة Error Handling

