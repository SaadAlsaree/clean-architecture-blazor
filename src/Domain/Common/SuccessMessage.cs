using System.ComponentModel.DataAnnotations;

namespace Domain.Common;

/// <summary>
/// Enum for application success messages with Arabic descriptions
/// </summary>
public enum SuccessMessage
{
    #region Database Operations (20001-20099)
    [Display(Name = "تم جلب البيانات بنجاح")]
    SuccessOnGet = 20001,

    [Display(Name = "تم إدخال البيانات بنجاح")]
    SuccessOnCreate = 20002,

    [Display(Name = "تم تحديث البيانات بنجاح")]
    SuccessOnUpdate = 20003,

    [Display(Name = "تم حذف البيانات بنجاح")]
    SuccessOnDelete = 20004,
    #endregion

    #region Authentication & Authorization (20100-20199)
    [Display(Name = "تم تسجيل الدخول بنجاح")]
    LoginSuccess = 20101,

    [Display(Name = "تم تسجيل الخروج بنجاح")]
    LogoutSuccess = 20102,

    [Display(Name = "تم إنشاء الحساب بنجاح")]
    AccountCreated = 20103,

    [Display(Name = "تم تحديث كلمة المرور بنجاح")]
    PasswordUpdated = 20104,
    #endregion

    #region File Operations (20200-20299)
    [Display(Name = "تم رفع الملف بنجاح")]
    FileUploaded = 20201,

    [Display(Name = "تم حذف الملف بنجاح")]
    FileDeleted = 20202,

    [Display(Name = "تم معالجة الملف بنجاح")]
    FileProcessed = 20203,
    #endregion

    #region General Operations (20300-20399)
    [Display(Name = "تم تنفيذ العملية بنجاح")]
    OperationSuccess = 20301,

    [Display(Name = "تم حفظ البيانات بنجاح")]
    DataSaved = 20302,

    [Display(Name = "تم إرسال البيانات بنجاح")]
    DataSent = 20303,
    #endregion
}
