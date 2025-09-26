using System.ComponentModel.DataAnnotations;

namespace Domain.Common;

/// <summary>
/// Enum for application error messages with Arabic descriptions
/// </summary>
public enum ErrorsMessage
{
    #region Database Operations (10001-10099)
    [Display(Name = "حدث خطأ أثناء جلب البيانات , أعد المحاولة لاحقاً")]
    FailOnGet = 10001,

    [Display(Name = "حدث خطأ أثناء إدخال البيانات , أعد المحاولة لاحقاً")]
    FailOnCreate = 10002,

    [Display(Name = "حدث خطأ أثناء تحديث البيانات , أعد المحاولة لاحقاً")]
    FailOnUpdate = 10003,

    [Display(Name = "حدث خطأ أثناء حذف البيانات , أعد المحاولة لاحقاً")]
    FailOnDelete = 10004,

    [Display(Name = "لا توجد بيانات")]
    NotFoundData = 10005,
    #endregion

    #region Validation Errors (10100-10199)
    [Display(Name = "المدخل موجود سابقاً")]
    ExistOnCreate = 10101,

    [Display(Name = "يرجى التحقق من المدخلات , لا توجد بيانات")]
    NotExistOnCreate = 10102,

    [Display(Name = "يرجى التحقق من المدخلات , لا توجد بيانات")]
    NotExistOnUpdate = 10103,

    [Display(Name = "البيانات المدخلة غير صحيحة")]
    InvalidInputData = 10104,

    [Display(Name = "الحقل مطلوب")]
    RequiredField = 10105,

    [Display(Name = "تنسيق البيانات غير صحيح")]
    InvalidDataFormat = 10106,

    [Display(Name = "الطول المسموح تجاوز الحد الأقصى")]
    ExceededMaxLength = 10107,

    [Display(Name = "القيمة خارج النطاق المسموح")]
    OutOfRange = 10108,
    #endregion

    #region Business Logic Errors (10200-10299)
    [Display(Name = "الحالة غير معرفة")]
    ThisStatusNotFound = 10201,

    [Display(Name = "العملية غير مسموحة في الحالة الحالية")]
    InvalidOperationForCurrentStatus = 10202,

    [Display(Name = "لا يمكن تنفيذ العملية - قيود العمل")]
    BusinessRuleViolation = 10203,

    [Display(Name = "الصلاحيات غير كافية لتنفيذ العملية")]
    InsufficientPermissions = 10204,

    [Display(Name = "العملية مكررة - تم تنفيذها مسبقاً")]
    DuplicateOperation = 10205,
    #endregion

    #region Authentication & Authorization (10300-10399)
    [Display(Name = "فشل في التحقق من الهوية")]
    AuthenticationFailed = 10301,

    [Display(Name = "انتهت صلاحية الجلسة")]
    SessionExpired = 10302,

    [Display(Name = "الوصول مرفوض")]
    AccessDenied = 10303,

    [Display(Name = "المستخدم غير مفعل")]
    UserInactive = 10304,

    [Display(Name = "حساب المستخدم محظور")]
    UserBlocked = 10305,
    #endregion

    #region External Services (10400-10499)
    [Display(Name = "فشل في الاتصال بالخدمة الخارجية")]
    ExternalServiceFailure = 10401,

    [Display(Name = "انتهت مهلة الاتصال")]
    ServiceTimeout = 10402,

    [Display(Name = "الخدمة غير متاحة حالياً")]
    ServiceUnavailable = 10403,

    [Display(Name = "فشل في معالجة الاستجابة")]
    ResponseProcessingFailed = 10404,
    #endregion

    #region File Operations (10500-10599)
    [Display(Name = "فشل في رفع الملف")]
    FileUploadFailed = 10501,

    [Display(Name = "نوع الملف غير مدعوم")]
    UnsupportedFileType = 10502,

    [Display(Name = "حجم الملف يتجاوز الحد المسموح")]
    FileSizeExceeded = 10503,

    [Display(Name = "الملف غير موجود")]
    FileNotFound = 10504,

    [Display(Name = "فشل في معالجة الملف")]
    FileProcessingFailed = 10505,
    #endregion

    #region System Errors (10600-10699)
    [Display(Name = "خطأ في النظام")]
    SystemError = 10601,

    [Display(Name = "الذاكرة غير كافية")]
    InsufficientMemory = 10602,

    [Display(Name = "خطأ في قاعدة البيانات")]
    DatabaseError = 10603,

    [Display(Name = "خطأ في الشبكة")]
    NetworkError = 10604,

    [Display(Name = "الخادم مشغول حالياً")]
    ServerBusy = 10605,
    #endregion
}