# UpdateHandler - دليل الاستخدام

## نظرة عامة

`UpdateHandler<TEntity, TCommand, TResponse>` هو كلاس أساسي مجرد يوفر وظائف مشتركة لتحديث الكيانات مع إرجاع أنواع مختلفة من الاستجابات.

## الميزات الرئيسية

### ✅ الميزات المضافة/المحسنة:

-  **مرونة في أنواع الاستجابة**: يدعم إرجاع `bool`, `int`, `Entity`, `DTO` أو أي نوع آخر
-  **معالجة الأخطاء المحسنة**: معالجة شاملة للأخطاء مع رسائل واضحة
-  **التحقق من صحة البيانات**: إمكانية إضافة تحقق مخصص للكيانات
-  **إدارة الحالة**: تحديث حالة الكيانات المحدثة
-  **الأمان**: استخدام آمن للأنواع
-  **المرونة**: إمكانية تخصيص السلوك عبر الوراثة

## أنواع الاستجابة المدعومة

### 1. إرجاع نجاح/فشل فقط (bool)

```csharp
public class UpdateUserHandler : UpdateHandler<User, UpdateUserCommand, bool>
{
    protected override bool MapToResponse(User entity) => true;
}
```

### 2. إرجاع معرف الكيان (int)

```csharp
public class UpdateProductHandler : UpdateHandler<Product, UpdateProductCommand, int>
{
    protected override int MapToResponse(Product entity) => entity.Id;
}
```

### 3. إرجاع الكيان كاملاً

```csharp
public class UpdateOrderHandler : UpdateHandler<Order, UpdateOrderCommand, Order>
{
    protected override Order MapToResponse(Order entity) => entity;
}
```

### 4. إرجاع DTO مخصص

```csharp
public class UpdateCategoryHandler : UpdateHandler<Category, UpdateCategoryCommand, CategoryDto>
{
    protected override CategoryDto MapToResponse(Category entity)
        => new() { Id = entity.Id, Name = entity.Name };
}
```

## كيفية الاستخدام

### 1. إنشاء Command

```csharp
public record UpdateUserCommand(int Id, string Name, string Email) : IRequest<Response<bool>>;
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
public class UpdateUserHandler : UpdateHandler<User, UpdateUserCommand, bool>
{
    public UpdateUserHandler(IWriteRepository<User> repository, IReadRepository<User> readRepository)
        : base(repository, readRepository) { }

    protected override Expression<Func<User, bool>>? FindPredicate(UpdateUserCommand request)
        => user => user.Id == request.Id;

    protected override User MapToEntity(UpdateUserCommand request, User existingEntity)
    {
        existingEntity.Name = request.Name;
        existingEntity.Email = request.Email;
        return existingEntity;
    }

    protected override bool MapToResponse(User entity) => true;

    public async Task<Response<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken = default)
        => await HandleBase(request, cancellationToken);
}
```

## الطرق القابلة للتخصيص

### `FindPredicate`

يحدد كيفية العثور على الكيان المراد تحديثه.

### `MapToEntity`

يحول الـ Command إلى Entity مع الحفاظ على الكيان الموجود.

### `MapToResponse`

يحول الكيان المُحدث إلى نوع الاستجابة المطلوب.

### `UpdateStatus`

يحدد الحالة الجديدة للكيان المحدث.

### `ValidateEntity`

يضيف تحقق مخصص للكيان قبل التحديث.

## معالجة الأخطاء

الكلاس يتعامل مع الأخطاء التالية:

-  **InvalidInputData**: بيانات الإدخال غير صحيحة
-  **NotFound**: الكيان غير موجود
-  **FailOnUpdate**: فشل في التحديث
-  **SystemError**: خطأ في النظام

## مثال كامل

راجع ملف `Examples.cs` للحصول على أمثلة شاملة للاستخدام.

# UpdateRangeHandler - دليل الاستخدام

## نظرة عامة

`UpdateRangeHandler<TEntity, TCommand, TResponse>` هو كلاس أساسي مجرد للعمليات المجمعة لتحديث عدة كيانات.

## الميزات الرئيسية

### ✅ الميزات المتقدمة:

-  **أداء محسن**: استخدام `UpdateRangeAsync` للعمليات المجمعة
-  **مرونة في التكوين**: خيارات متقدمة للعمليات المجمعة
-  **معالجة الأخطاء المتقدمة**: استراتيجيات مختلفة للتعامل مع الأخطاء
-  **إدارة النزاعات**: استراتيجيات مختلفة لحل النزاعات
-  **التحقق المخصص**: إمكانية إضافة تحقق مخصص للبيانات

## أنواع الاستجابة المدعومة

### 1. إرجاع تفاصيل العملية المجمعة (BulkResult)

```csharp
public class UpdateUsersRangeHandler : UpdateRangeHandler<User, UpdateUsersRangeCommand, BulkResult>
{
    protected override BulkResult MapToResponse(IEnumerable<User> entities) => new BulkResult();
}
```

### 2. إرجاع ملخص العملية (BulkSummaryDto)

```csharp
public class UpdateProductsRangeHandler : UpdateRangeHandler<Product, UpdateProductsRangeCommand, BulkSummaryDto>
{
    protected override BulkSummaryDto MapToResponse(IEnumerable<Product> entities)
        => new BulkSummaryDto
        {
            TotalRecords = entities.Count(),
            // ... other properties
        };
}
```

### 3. إرجاع نجاح/فشل فقط (bool)

```csharp
public class UpdateOrdersRangeHandler : UpdateRangeHandler<Order, UpdateOrdersRangeCommand, bool>
{
    protected override bool MapToResponse(IEnumerable<Order> entities) => entities.Any();
}
```

## كيفية الاستخدام

### 1. إنشاء Command

```csharp
public record UpdateUsersRangeCommand(List<UserUpdateData> Users) : IRequest<Response<bool>>;
```

### 2. إنشاء Handler

```csharp
public class UpdateUsersRangeHandler : UpdateRangeHandler<User, UpdateUsersRangeCommand, bool>
{
    public UpdateUsersRangeHandler(IWriteRepository<User> repository, IReadRepository<User> readRepository)
        : base(repository, readRepository) { }

    protected override Expression<Func<User, bool>>? FindPredicate(UpdateUsersRangeCommand request)
        => user => request.Users.Select(u => u.Id).Contains(user.Id);

    protected override IEnumerable<User> MapToEntities(UpdateUsersRangeCommand request, IEnumerable<User> existingEntities)
    {
        var updateMap = request.Users.ToDictionary(u => u.Id, u => u);
        return existingEntities.Select(entity =>
        {
            if (updateMap.TryGetValue(entity.Id, out var updateData))
            {
                entity.Name = updateData.Name;
                entity.Email = updateData.Email;
            }
            return entity;
        });
    }

    protected override bool MapToResponse(IEnumerable<User> entities) => entities.Any();

    public async Task<Response<bool>> Handle(UpdateUsersRangeCommand request, CancellationToken cancellationToken = default)
        => await HandleBase(request, cancellationToken);
}
```

## الطرق القابلة للتخصيص

### `FindPredicate`

يحدد كيفية العثور على الكيانات المراد تحديثها.

### `MapToEntities`

يحول الـ Command إلى مجموعة من الكيانات المحدثة.

### `MapToResponse`

يحول الكيانات المُحدثة إلى نوع الاستجابة المطلوب.

### `UpdateStatus`

يحدد الحالة الجديدة للكيانات المحدثة.

### `ValidateEntities`

يضيف تحقق مخصص للكيانات قبل التحديث.

## معالجة الأخطاء المتقدمة

### BulkResult

```csharp
public class BulkResult
{
    public int TotalRecords { get; set; }
    public int SuccessfulUpdates { get; set; }
    public int SkippedRecords { get; set; }
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

## مثال كامل

راجع ملف `UpdateRangeExamples.cs` للحصول على أمثلة شاملة للاستخدام.

# UpdateBulkHandler - دليل الاستخدام

## نظرة عامة

`UpdateBulkHandler<TEntity, TCommand, TResponse>` هو كلاس أساسي مجرد للعمليات المجمعة عالية الأداء باستخدام `IBulkRepository`.

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
public class UpdateUsersBulkHandler : UpdateBulkHandler<User, UpdateUsersBulkCommand, BulkResult>
{
    protected override BulkResult MapToResponse(BulkResult bulkResult) => bulkResult;
}
```

### 2. إرجاع ملخص العملية (BulkSummaryDto)

```csharp
public class UpdateProductsBulkHandler : UpdateBulkHandler<Product, UpdateProductsBulkCommand, BulkSummaryDto>
{
    protected override BulkSummaryDto MapToResponse(BulkResult bulkResult)
        => new BulkSummaryDto
        {
            TotalRecords = bulkResult.TotalRecords,
            SuccessfulUpdates = bulkResult.SuccessfulUpdates,
            // ... other properties
        };
}
```

### 3. إرجاع نجاح/فشل فقط (bool)

```csharp
public class UpdateOrdersBulkHandler : UpdateBulkHandler<Order, UpdateOrdersBulkCommand, bool>
{
    protected override bool MapToResponse(BulkResult bulkResult) => bulkResult.IsSuccess;
}
```

## خيارات العملية المجمعة

### BulkUpdateOptions

```csharp
protected override BulkUpdateOptions<TEntity> GetBulkOptions()
{
    return new BulkUpdateOptions<TEntity>
    {
        BatchSize = 1000,                    // حجم الدفعة
        UseTransaction = true,               // استخدام المعاملات
        TimeoutInSeconds = 300,              // مهلة زمنية
        ConflictStrategy = ConflictResolutionStrategy.Update,  // استراتيجية النزاع
        ValidateEntities = true,             // التحقق من الكيانات
        ErrorHandling = BulkErrorHandling.ContinueOnError,   // معالجة الأخطاء
        ReturnDetailedResults = true         // إرجاع النتائج التفصيلية
    };
}
```

## استراتيجيات حل النزاعات

### ConflictResolutionStrategy

-  **Update**: تحديث السجلات الموجودة
-  **Skip**: تخطي السجلات المكررة
-  **Replace**: استبدال السجلات الموجودة
-  **ThrowError**: إلقاء خطأ عند النزاع

### BulkErrorHandling

-  **ThrowOnError**: إلقاء استثناء عند أول خطأ
-  **ContinueOnError**: متابعة المعالجة وجمع الأخطاء
-  **SkipErrors**: تخطي السجلات الخاطئة بصمت

## كيفية الاستخدام

### 1. إنشاء Command

```csharp
public record UpdateUsersBulkCommand(List<UserUpdateData> Users) : IRequest<Response<BulkResult>>;
```

### 2. إنشاء Handler

```csharp
public class UpdateUsersBulkHandler : UpdateBulkHandler<User, UpdateUsersBulkCommand, BulkResult>
{
    public UpdateUsersBulkHandler(IBulkRepository<User> bulkRepository, IReadRepository<User> readRepository)
        : base(bulkRepository, readRepository) { }

    protected override Expression<Func<User, bool>>? FindPredicate(UpdateUsersBulkCommand request)
        => user => request.Users.Select(u => u.Id).Contains(user.Id);

    protected override IEnumerable<User> MapToEntities(UpdateUsersBulkCommand request, IEnumerable<User> existingEntities)
    {
        var updateMap = request.Users.ToDictionary(u => u.Id, u => u);
        return existingEntities.Select(entity =>
        {
            if (updateMap.TryGetValue(entity.Id, out var updateData))
            {
                entity.Name = updateData.Name;
                entity.Email = updateData.Email;
            }
            return entity;
        });
    }

    protected override Expression<Func<User, object>>? GetKeySelector()
        => user => user.Id;

    protected override BulkResult MapToResponse(BulkResult bulkResult) => bulkResult;

    public async Task<Response<BulkResult>> Handle(UpdateUsersBulkCommand request, CancellationToken cancellationToken = default)
        => await HandleBase(request, cancellationToken);
}
```

## الطرق القابلة للتخصيص

### `FindPredicate`

يحدد كيفية العثور على الكيانات المراد تحديثها.

### `MapToEntities`

يحول الـ Command إلى مجموعة من الكيانات المحدثة.

### `GetKeySelector`

يحدد المفتاح الفريد للكيانات (لحل النزاعات).

### `MapToResponse`

يحول نتيجة العملية المجمعة إلى نوع الاستجابة المطلوب.

### `GetBulkOptions`

يحدد خيارات العملية المجمعة.

### `UpdateStatus`

يحدد الحالة الجديدة للكيانات المحدثة.

### `ValidateEntities`

يضيف تحقق مخصص للكيانات قبل التحديث.

## معالجة الأخطاء المتقدمة

### BulkResult

```csharp
public class BulkResult
{
    public int TotalRecords { get; set; }
    public int SuccessfulUpdates { get; set; }
    public int SkippedRecords { get; set; }
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

راجع ملف `UpdateBulkExamples.cs` للحصول على أمثلة شاملة للاستخدام.

## الفرق بين UpdateHandler و UpdateRangeHandler و UpdateBulkHandler

| الميزة               | UpdateHandler | UpdateRangeHandler | UpdateBulkHandler         |
| -------------------- | ------------- | ------------------ | ------------------------- |
| **الاستخدام**        | كيان واحد     | عدة كيانات         | عمليات مجمعة عالية الأداء |
| **الأداء**           | عادي          | متوسط              | عالي جداً                 |
| **الخيارات**         | أساسية        | متوسطة             | متقدمة                    |
| **التقارير**         | بسيطة         | بسيطة              | تفصيلية                   |
| **الاستخدام الأمثل** | تحديث فردي    | تحديث متعدد        | تحديث ضخم                 |
