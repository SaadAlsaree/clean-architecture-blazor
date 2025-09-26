# GetListWithPagedHandler - دليل الاستخدام

## نظرة عامة

`GetListWithPagedHandler<TEntity, TQuery, TViewModel>` هو كلاس أساسي مجرد يوفر وظائف مشتركة لاستعلامات القوائم مع التصفح (Pagination) مع إرجاع أنواع مختلفة من الاستجابات.

## الميزات الرئيسية

### ✅ الميزات المضافة/المحسنة:

-  **التصفح الذكي**: دعم كامل للتصفح مع معلومات تفصيلية
-  **الفلترة المتقدمة**: إمكانية إضافة فلاتر مخصصة للاستعلامات
-  **الترتيب المرن**: دعم ترتيب متقدم للنتائج
-  **الربط بالكيانات**: دعم تضمين الكيانات المرتبطة
-  **التحقق من صحة البيانات**: إمكانية إضافة تحقق مخصص للاستعلامات
-  **الأمان**: استخدام آمن للأنواع
-  **المرونة**: إمكانية تخصيص السلوك عبر الوراثة

## أنواع الاستجابة المدعومة

### 1. إرجاع قائمة بسيطة (List)

```csharp
public class GetUsersListHandler : GetListWithPagedHandler<User, GetUsersListQuery, UserDto>
{
    protected override UserDto MapToViewModel(User entity)
        => new() { Id = entity.Id, Name = entity.Name };
}
```

### 2. إرجاع قائمة مع تفاصيل إضافية

```csharp
public class GetProductsListHandler : GetListWithPagedHandler<Product, GetProductsListQuery, ProductDetailDto>
{
    protected override ProductDetailDto MapToViewModel(Product entity)
        => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Price = entity.Price,
            CategoryName = entity.Category?.Name
        };
}
```

### 3. إرجاع قائمة مع إحصائيات

```csharp
public class GetOrdersListHandler : GetListWithPagedHandler<Order, GetOrdersListQuery, OrderSummaryDto>
{
    protected override OrderSummaryDto MapToViewModel(Order entity)
        => new()
        {
            Id = entity.Id,
            OrderDate = entity.OrderDate,
            TotalAmount = entity.OrderItems.Sum(x => x.Price * x.Quantity),
            Status = entity.Status.ToString()
        };
}
```

## كيفية الاستخدام

### 1. إنشاء Query

```csharp
public record GetUsersListQuery(
    string? SearchTerm = null,
    int? StatusId = null,
    int Page = 1,
    byte PageSize = 10
) : IRequest<Response<PagedResult<UserDto>>>, IPaginationQuery;
```

### 2. إنشاء Entity

```csharp
public class User : IStatusEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Status StatusId { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### 3. إنشاء ViewModel

```csharp
public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
```

### 4. إنشاء Handler

```csharp
public class GetUsersListHandler : GetListWithPagedHandler<User, GetUsersListQuery, UserDto>
{
    public GetUsersListHandler(IReadRepository<User> readRepository)
        : base(readRepository) { }

    protected override Expression<Func<User, bool>>? FilterPredicate(GetUsersListQuery request)
    {
        return user =>
            (string.IsNullOrEmpty(request.SearchTerm) ||
             user.Name.Contains(request.SearchTerm) ||
             user.Email.Contains(request.SearchTerm)) &&
            (!request.StatusId.HasValue || user.StatusId == (Status)request.StatusId);
    }

    protected override Func<IQueryable<User>, IOrderedQueryable<User>>? OrderBy(GetUsersListQuery request)
        => users => users.OrderByDescending(u => u.CreatedAt);

    protected override UserDto MapToViewModel(User entity)
        => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email,
            Status = entity.StatusId.ToString(),
            CreatedAt = entity.CreatedAt
        };

    public async Task<Response<PagedResult<UserDto>>> Handle(GetUsersListQuery request, CancellationToken cancellationToken = default)
        => await HandleBase(request, cancellationToken);
}
```

## الطرق القابلة للتخصيص

### `FilterPredicate`

يحدد كيفية فلترة البيانات بناءً على معايير البحث.

```csharp
protected override Expression<Func<User, bool>>? FilterPredicate(GetUsersListQuery request)
{
    return user =>
        (string.IsNullOrEmpty(request.SearchTerm) ||
         user.Name.Contains(request.SearchTerm)) &&
        (!request.StatusId.HasValue || user.StatusId == (Status)request.StatusId);
}
```

### `OrderBy`

يحدد كيفية ترتيب النتائج.

```csharp
protected override Func<IQueryable<User>, IOrderedQueryable<User>>? OrderBy(GetUsersListQuery request)
{
    return users => users.OrderByDescending(u => u.CreatedAt);
}
```

### `MapToViewModel`

يحول الكيان إلى ViewModel.

```csharp
protected override UserDto MapToViewModel(User entity)
{
    return new UserDto
    {
        Id = entity.Id,
        Name = entity.Name,
        Email = entity.Email
    };
}
```

### `Include`

يحدد الكيانات المرتبطة التي يجب تضمينها.

```csharp
protected override Func<IQueryable<User>, IIncludableQueryable<User, object>>? Include(GetUsersListQuery request)
{
    return users => users.Include(u => u.Profile).Include(u => u.Roles);
}
```

### `ValidateQuery`

يضيف تحقق مخصص للاستعلام.

```csharp
protected override async Task<Response<PagedResult<UserDto>>> ValidateQuery(GetUsersListQuery request, CancellationToken cancellationToken)
{
    if (request.Page < 1)
        return ErrorsMessage.InvalidInputData.ToErrorMessage(default(PagedResult<UserDto>)!);

    if (request.PageSize > 100)
        return ErrorsMessage.ExceededMaxLength.ToErrorMessage(default(PagedResult<UserDto>)!);

    return Response<PagedResult<UserDto>>.Success(default(PagedResult<UserDto>)!);
}
```

## PagedResult Properties

### خصائص النتيجة المصفحة

```csharp
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; }        // العناصر
    public int TotalCount { get; set; }              // العدد الإجمالي
    public int Page { get; set; }                    // الصفحة الحالية
    public int PageSize { get; set; }                // حجم الصفحة
    public int TotalPages { get; set; }              // إجمالي الصفحات
    public bool HasNextPage { get; set; }            // هل توجد صفحة تالية؟
    public bool HasPreviousPage { get; set; }         // هل توجد صفحة سابقة؟
}
```

## معالجة الأخطاء

الكلاس يتعامل مع الأخطاء التالية:

-  **InvalidInputData**: بيانات الإدخال غير صحيحة
-  **FailOnGet**: فشل في جلب البيانات
-  **SystemError**: خطأ في النظام

## أمثلة متقدمة

### 1. استعلام مع فلترة متقدمة

```csharp
public class GetProductsListHandler : GetListWithPagedHandler<Product, GetProductsListQuery, ProductDto>
{
    protected override Expression<Func<Product, bool>>? FilterPredicate(GetProductsListQuery request)
    {
        return product =>
            (string.IsNullOrEmpty(request.Name) || product.Name.Contains(request.Name)) &&
            (!request.CategoryId.HasValue || product.CategoryId == request.CategoryId) &&
            (!request.MinPrice.HasValue || product.Price >= request.MinPrice) &&
            (!request.MaxPrice.HasValue || product.Price <= request.MaxPrice) &&
            (!request.IsActive.HasValue || product.IsActive == request.IsActive);
    }
}
```

### 2. استعلام مع ترتيب متعدد

```csharp
protected override Func<IQueryable<Product>, IOrderedQueryable<Product>>? OrderBy(GetProductsListQuery request)
{
    return products => products
        .OrderBy(p => p.Category.Name)
        .ThenByDescending(p => p.Price)
        .ThenBy(p => p.Name);
}
```

### 3. استعلام مع تضمين الكيانات المرتبطة

```csharp
protected override Func<IQueryable<Product>, IIncludableQueryable<Product, object>>? Include(GetProductsListQuery request)
{
    return products => products
        .Include(p => p.Category)
        .Include(p => p.Supplier)
        .Include(p => p.Reviews);
}
```

### 4. استعلام مع تحقق مخصص

```csharp
protected override async Task<Response<PagedResult<ProductDto>>> ValidateQuery(GetProductsListQuery request, CancellationToken cancellationToken)
{
    // التحقق من صحة نطاق السعر
    if (request.MinPrice.HasValue && request.MaxPrice.HasValue &&
        request.MinPrice > request.MaxPrice)
    {
        return ErrorsMessage.InvalidInputData.ToErrorMessage(default(PagedResult<ProductDto>)!);
    }

    // التحقق من حجم الصفحة
    if (request.PageSize > 50)
    {
        return ErrorsMessage.ExceededMaxLength.ToErrorMessage(default(PagedResult<ProductDto>)!);
    }

    return Response<PagedResult<ProductDto>>.Success(default(PagedResult<ProductDto>)!);
}
```

## مثال كامل

```csharp
// Query
public record GetUsersListQuery(
    string? SearchTerm = null,
    int? StatusId = null,
    string? SortBy = "CreatedAt",
    string? SortDirection = "desc",
    int Page = 1,
    byte PageSize = 10
) : IRequest<Response<PagedResult<UserDto>>>, IPaginationQuery;

// Handler
public class GetUsersListHandler : GetListWithPagedHandler<User, GetUsersListQuery, UserDto>
{
    public GetUsersListHandler(IReadRepository<User> readRepository)
        : base(readRepository) { }

    protected override Expression<Func<User, bool>>? FilterPredicate(GetUsersListQuery request)
    {
        return user =>
            (string.IsNullOrEmpty(request.SearchTerm) ||
             user.Name.Contains(request.SearchTerm) ||
             user.Email.Contains(request.SearchTerm)) &&
            (!request.StatusId.HasValue || user.StatusId == (Status)request.StatusId);
    }

    protected override Func<IQueryable<User>, IOrderedQueryable<User>>? OrderBy(GetUsersListQuery request)
    {
        return request.SortBy?.ToLower() switch
        {
            "name" => request.SortDirection == "asc"
                ? users => users.OrderBy(u => u.Name)
                : users => users.OrderByDescending(u => u.Name),
            "email" => request.SortDirection == "asc"
                ? users => users.OrderBy(u => u.Email)
                : users => users.OrderByDescending(u => u.Email),
            _ => users => users.OrderByDescending(u => u.CreatedAt)
        };
    }

    protected override UserDto MapToViewModel(User entity)
        => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email,
            Status = entity.StatusId.ToString(),
            CreatedAt = entity.CreatedAt
        };

    public async Task<Response<PagedResult<UserDto>>> Handle(GetUsersListQuery request, CancellationToken cancellationToken = default)
        => await HandleBase(request, cancellationToken);
}
```

## الاستخدام في Controller

```csharp
[HttpGet]
public async Task<IActionResult> GetUsers([FromQuery] GetUsersListQuery query)
{
    var result = await _mediator.Send(query);

    if (!result.Succeeded)
        return BadRequest(result);

    return Ok(result.Data);
}
```

## الفوائد الرئيسية

-  **أداء محسن**: استخدام التصفح لتقليل استهلاك الذاكرة
-  **مرونة عالية**: دعم فلاتر وترتيب مخصص
-  **سهولة الاستخدام**: واجهة بسيطة وواضحة
-  **قابلية التوسع**: إمكانية إضافة ميزات جديدة بسهولة
-  **معالجة الأخطاء**: معالجة شاملة للأخطاء
-  **التحقق من البيانات**: دعم التحقق المخصص
