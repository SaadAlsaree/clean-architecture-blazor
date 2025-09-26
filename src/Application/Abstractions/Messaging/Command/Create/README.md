# CreateHandler - دليل الاستخدام

## نظرة عامة

`CreateHandler<TEntity, TCommand, TResponse>` هو كلاس أساسي مجرد يوفر وظائف مشتركة لإنشاء الكيانات مع إرجاع أنواع مختلفة من الاستجابات.

## الميزات الرئيسية

### ✅ الميزات المضافة/المحسنة:

-  **مرونة في أنواع الاستجابة**: يدعم إرجاع `bool`, `int`, `Entity`, `DTO` أو أي نوع آخر
-  **معالجة الأخطاء المحسنة**: معالجة شاملة للأخطاء مع رسائل واضحة
-  **التحقق من صحة البيانات**: إمكانية إضافة تحقق مخصص للكيانات
-  **إدارة الحالة**: تعيين حالة افتراضية للكيانات الجديدة
-  **الأمان**: استخدام آمن للأنواع
-  **المرونة**: إمكانية تخصيص السلوك عبر الوراثة

## أنواع الاستجابة المدعومة

### 1. إرجاع نجاح/فشل فقط (bool)

```csharp
public class CreateUserHandler : CreateHandler<User, CreateUserCommand, bool>
{
    protected override bool MapToResponse(User entity) => true;
}
```

### 2. إرجاع معرف الكيان (int)

```csharp
public class CreateProductHandler : CreateHandler<Product, CreateProductCommand, int>
{
    protected override int MapToResponse(Product entity) => entity.Id;
}
```

### 3. إرجاع الكيان كاملاً

```csharp
public class CreateOrderHandler : CreateHandler<Order, CreateOrderCommand, Order>
{
    protected override Order MapToResponse(Order entity) => entity;
}
```

### 4. إرجاع DTO مخصص

```csharp
public class CreateCategoryHandler : CreateHandler<Category, CreateCategoryCommand, CategoryDto>
{
    protected override CategoryDto MapToResponse(Category entity)
        => new() { Id = entity.Id, Name = entity.Name };
}
```

## كيفية الاستخدام

### 1. إنشاء Command

```csharp
public record CreateUserCommand(string Name, string Email) : IRequest<Response<bool>>;
```

### 2. إنشاء Entity

```csharp
public class User : IStatusEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Status StatusId { get; set; }
}
```

### 3. إنشاء Handler

```csharp
public class CreateUserHandler : CreateHandler<User, CreateUserCommand, bool>
{
    public CreateUserHandler(IWriteRepository<User> repository, IReadRepository<User> readRepository)
        : base(repository, readRepository) { }

    protected override Expression<Func<User, bool>>? ExistencePredicate(CreateUserCommand request)
        => user => user.Email == request.Email;

    protected override User MapToEntity(CreateUserCommand request)
        => new() { Name = request.Name, Email = request.Email };

    protected override bool MapToResponse(User entity) => true;

    public async Task<Response<bool>> Handle(CreateUserCommand request, CancellationToken cancellationToken = default)
        => await HandleBase(request, cancellationToken);
}
```

## الطرق القابلة للتخصيص

### `ExistencePredicate`

يحدد كيفية التحقق من وجود الكيان مسبقاً.

### `MapToEntity`

يحول الـ Command إلى Entity.

### `MapToResponse`

يحول الكيان المُنشأ إلى نوع الاستجابة المطلوب.

### `SetInitialStatus`

يحدد الحالة الافتراضية للكيان الجديد.

### `ValidateEntity`

يضيف تحقق مخصص للكيان قبل الإنشاء.

## معالجة الأخطاء

الكلاس يتعامل مع الأخطاء التالية:

-  **InvalidInputData**: بيانات الإدخال غير صحيحة
-  **ExistOnCreate**: الكيان موجود مسبقاً
-  **FailOnCreate**: فشل في الإنشاء
-  **SystemError**: خطأ في النظام

## مثال كامل

راجع ملف `Examples.cs` للحصول على أمثلة شاملة للاستخدام.

# CreateBulkHandler - دليل الاستخدام

## نظرة عامة

`CreateBulkHandler<TEntity, TCommand, TResponse>` هو كلاس أساسي مجرد للعمليات المجمعة عالية الأداء باستخدام `IBulkRepository`.

## الميزات الرئيسية

### ✅ الميزات المتقدمة:

-  **أداء عالي**: استخدام `IBulkRepository` للعمليات المجمعة
-  **مرونة في التكوين**: خيارات متقدمة للعمليات المجمعة
-  **معالجة الأخطاء المتقدمة**: استراتيجيات مختلفة للتعامل مع الأخطاء
-  **تقارير التقدم**: دعم لتقارير التقدم في العمليات الطويلة
-  **إدارة النزاعات**: استراتيجيات مختلفة لحل النزاعات
-  **التحقق المخصص**: إمكانية إضافة تحقق مخصص للبيانات

## أنواع الاستجابة المدعومة

### 1. إرجاع تفاصيل العملية المجمعة (BulkResult)

```csharp
public class CreateUsersBulkHandler : CreateBulkHandler<User, CreateUsersBulkCommand, BulkResult>
{
    protected override BulkResult MapToResponse(BulkResult bulkResult) => bulkResult;
}
```

### 2. إرجاع ملخص العملية (BulkSummaryDto)

```csharp
public class CreateProductsBulkHandler : CreateBulkHandler<Product, CreateProductsBulkCommand, BulkSummaryDto>
{
    protected override BulkSummaryDto MapToResponse(BulkResult bulkResult)
        => new BulkSummaryDto
        {
            TotalRecords = bulkResult.TotalRecords,
            SuccessfulInserts = bulkResult.SuccessfulInserts,
            // ... other properties
        };
}
```

### 3. إرجاع نجاح/فشل فقط (bool)

```csharp
public class CreateOrdersBulkHandler : CreateBulkHandler<Order, CreateOrdersBulkCommand, bool>
{
    protected override bool MapToResponse(BulkResult bulkResult) => bulkResult.IsSuccess;
}
```

## خيارات العملية المجمعة

### BulkInsertOptions

```csharp
protected override BulkInsertOptions<TEntity> GetBulkOptions()
{
    return new BulkInsertOptions<TEntity>
    {
        BatchSize = 1000,                    // حجم الدفعة
        UseTransaction = true,               // استخدام المعاملات
        TimeoutInSeconds = 300,              // مهلة زمنية
        ConflictStrategy = ConflictResolutionStrategy.Skip,  // استراتيجية النزاع
        ValidateEntities = true,             // التحقق من الكيانات
        ErrorHandling = BulkErrorHandling.ContinueOnError,   // معالجة الأخطاء
        ReturnDetailedResults = true         // إرجاع النتائج التفصيلية
    };
}
```

## استراتيجيات حل النزاعات

### ConflictResolutionStrategy

-  **Skip**: تخطي السجلات المكررة
-  **Update**: تحديث السجلات الموجودة
-  **Replace**: استبدال السجلات الموجودة
-  **ThrowError**: إلقاء خطأ عند النزاع

### BulkErrorHandling

-  **ThrowOnError**: إلقاء استثناء عند أول خطأ
-  **ContinueOnError**: متابعة المعالجة وجمع الأخطاء
-  **SkipErrors**: تخطي السجلات الخاطئة بصمت

## كيفية الاستخدام

### 1. إنشاء Command

```csharp
public record CreateUsersBulkCommand(List<UserData> Users) : IRequest<Response<BulkResult>>;
```

### 2. إنشاء Handler

```csharp
public class CreateUsersBulkHandler : CreateBulkHandler<User, CreateUsersBulkCommand, BulkResult>
{
    public CreateUsersBulkHandler(IBulkRepository<User> bulkRepository, IReadRepository<User> readRepository)
        : base(bulkRepository, readRepository) { }

    protected override Expression<Func<User, bool>>? ExistencePredicate(CreateUsersBulkCommand request)
        => user => request.Users.Select(u => u.Email).Contains(user.Email);

    protected override IEnumerable<User> MapToEntities(CreateUsersBulkCommand request)
        => request.Users.Select(u => new User { Name = u.Name, Email = u.Email });

    protected override Expression<Func<User, object>>? GetKeySelector()
        => user => user.Email;

    protected override BulkResult MapToResponse(BulkResult bulkResult) => bulkResult;

    public async Task<Response<BulkResult>> Handle(CreateUsersBulkCommand request, CancellationToken cancellationToken = default)
        => await HandleBase(request, cancellationToken);
}
```

## الطرق القابلة للتخصيص

### `ExistencePredicate`

يحدد كيفية التحقق من وجود الكيانات مسبقاً.

### `MapToEntities`

يحول الـ Command إلى مجموعة من الكيانات.

### `GetKeySelector`

يحدد المفتاح الفريد للكيانات (لحل النزاعات).

### `MapToResponse`

يحول نتيجة العملية المجمعة إلى نوع الاستجابة المطلوب.

### `GetBulkOptions`

يحدد خيارات العملية المجمعة.

### `SetInitialStatus`

يحدد الحالة الافتراضية للكيانات الجديدة.

### `ValidateEntities`

يضيف تحقق مخصص للكيانات قبل الإنشاء.

## معالجة الأخطاء المتقدمة

### BulkResult

```csharp
public class BulkResult
{
    public int TotalRecords { get; set; }
    public int SuccessfulInserts { get; set; }
    public int SkippedRecords { get; set; }
    public int UpdatedRecords { get; set; }
    public int ErroredRecords { get; set; }
    public List<BulkError> Errors { get; set; }
    public TimeSpan Duration { get; set; }
    public bool IsSuccess { get; set; }
}
```

### BulkError

```csharp
public class BulkError
{
    public int RowIndex { get; set; }
    public string? ErrorMessage { get; set; }
    public Exception? Exception { get; set; }
    public object? FailedEntity { get; set; }
}
```

## تقارير التقدم

```csharp
public class BulkProgress
{
    public int TotalRecords { get; set; }
    public int ProcessedRecords { get; set; }
    public int SuccessfulRecords { get; set; }
    public int ErroredRecords { get; set; }
    public double ProgressPercentage { get; set; }
    public TimeSpan ElapsedTime { get; set; }
    public string? CurrentOperation { get; set; }
}
```

## مثال كامل

راجع ملف `CreateBulkExamples.cs` للحصول على أمثلة شاملة للاستخدام.

## الفرق بين CreateHandler و CreateRangeHandler و CreateBulkHandler

| الميزة               | CreateHandler | CreateRangeHandler | CreateBulkHandler         |
| -------------------- | ------------- | ------------------ | ------------------------- |
| **الاستخدام**        | كيان واحد     | عدة كيانات         | عمليات مجمعة عالية الأداء |
| **الأداء**           | عادي          | متوسط              | عالي جداً                 |
| **الخيارات**         | أساسية        | متوسطة             | متقدمة                    |
| **التقارير**         | بسيطة         | بسيطة              | تفصيلية                   |
| **الاستخدام الأمثل** | إنشاء فردي    | إنشاء متعدد        | استيراد ضخم               |
