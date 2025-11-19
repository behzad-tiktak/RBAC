using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RBACSystem.BusinessLogic.RBAC;
using RBACSystem.Models.RBAC;
using RBACSystem.Utilities;

namespace RBACSystem.Base
{
    // ====================================================================================================
    // BasePage - کلاس پایه برای تمام صفحات
    // مسیر: App_Code/Base/BasePage.cs
    // بدون namespace برای سادگی در Web Forms
    // ====================================================================================================

    /// <summary>
    /// کلاس پایه برای تمام صفحات
    /// شامل: احراز هویت، بررسی دسترسی، نمایش پیام
    /// </summary>
    public class BasePage : Page
    {
        protected RBACManager RBACManager;
        protected User CurrentUser;
        protected int CurrentUserId;

        /// <summary>
        /// آیا صفحه نیاز به احراز هویت دارد؟
        /// </summary>
        protected virtual bool RequiresAuthentication
        {
            get { return true; }
        }

        /// <summary>
        /// دسترسی‌های مورد نیاز صفحه (null = بدون محدودیت)
        /// </summary>
        protected virtual string[] RequiredPermissions
        {
            get { return null; }
        }

        /// <summary>
        /// آیا کاربر باید همه دسترسی‌ها را داشته باشد؟ (پیش‌فرض: فقط یکی کافیه)
        /// </summary>
        protected virtual bool RequireAllPermissions
        {
            get { return false; }
        }

        /// <summary>
        /// URL صفحه لاگین
        /// </summary>
        protected virtual string LoginPageUrl
        {
            get { return "~/Pages/Admin/Login.aspx"; }
        }

        /// <summary>
        /// URL صفحه عدم دسترسی
        /// </summary>
        protected virtual string AccessDeniedPageUrl
        {
            get { return "~/Pages/Admin/AccessDenied.aspx"; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // ایجاد نمونه RBACManager
            RBACManager = new RBACManager();

            // بررسی احراز هویت
            if (RequiresAuthentication)
            {
                if (!SecurityHelper.IsAuthenticated())
                {
                    // Redirect به صفحه لاگین
                    string returnUrl = Request.Url.PathAndQuery;
                    string loginUrl = string.Format("{0}?ReturnUrl={1}",
                        ResolveUrl(LoginPageUrl),
                        Server.UrlEncode(returnUrl));

                    Response.Redirect(loginUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                // دریافت کاربر فعلی
                CurrentUser = SecurityHelper.GetCurrentUser();
                CurrentUserId = SecurityHelper.GetCurrentUserId();

                // بررسی دسترسی‌ها
                if (RequiredPermissions != null && RequiredPermissions.Length > 0)
                {
                    bool hasAccess = false;

                    if (RequireAllPermissions)
                    {
                        // باید همه دسترسی‌ها را داشته باشد
                        hasAccess = RBACManager.UserHasAllPermissions(CurrentUserId, RequiredPermissions);
                    }
                    else
                    {
                        // یکی از دسترسی‌ها کافی است
                        hasAccess = RBACManager.UserHasAnyPermission(CurrentUserId, RequiredPermissions);
                    }

                    if (!hasAccess)
                    {
                        Response.Redirect(ResolveUrl(AccessDeniedPageUrl), false);
                        Context.ApplicationInstance.CompleteRequest();
                        return;
                    }
                }
            }
        }

        #region نمایش پیام‌ها

        /// <summary>
        /// نمایش پیام موفقیت
        /// </summary>
        protected void ShowSuccessMessage(string message)
        {
            ShowMessage(message, "success");
        }

        /// <summary>
        /// نمایش پیام خطا
        /// </summary>
        protected void ShowErrorMessage(string message)
        {
            ShowMessage(message, "error");
        }

        /// <summary>
        /// نمایش پیام هشدار
        /// </summary>
        protected void ShowWarningMessage(string message)
        {
            ShowMessage(message, "warning");
        }

        /// <summary>
        /// نمایش پیام اطلاعاتی
        /// </summary>
        protected void ShowInfoMessage(string message)
        {
            ShowMessage(message, "info");
        }

        /// <summary>
        /// نمایش پیام عمومی
        /// </summary>
        protected virtual void ShowMessage(string message, string type)
        {
            // این متد باید در کلاس‌های فرزند Override شود
            // یا می‌توانید از JavaScript Alert استفاده کنید

            string script = string.Format("alert('{0}');", message.Replace("'", "\\'"));
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowMessage", script, true);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// بررسی اینکه آیا کاربر فعلی دسترسی خاصی دارد
        /// </summary>
        protected bool HasPermission(string permissionName)
        {
            if (CurrentUserId <= 0)
                return false;

            return RBACManager.UserHasPermission(CurrentUserId, permissionName);
        }

        /// <summary>
        /// بررسی اینکه آیا کاربر فعلی یکی از دسترسی‌ها را دارد
        /// </summary>
        protected bool HasAnyPermission(params string[] permissionNames)
        {
            if (CurrentUserId <= 0)
                return false;

            return RBACManager.UserHasAnyPermission(CurrentUserId, permissionNames);
        }

        /// <summary>
        /// بررسی اینکه آیا کاربر فعلی همه دسترسی‌ها را دارد
        /// </summary>
        protected bool HasAllPermissions(params string[] permissionNames)
        {
            if (CurrentUserId <= 0)
                return false;

            return RBACManager.UserHasAllPermissions(CurrentUserId, permissionNames);
        }

        /// <summary>
        /// دریافت اطلاعات کامل کاربر
        /// </summary>
        protected User GetCurrentUserFullInfo()
        {
            if (CurrentUserId <= 0)
                return null;

            return RBACManager.GetUserFullInfo(CurrentUserId);
        }

        /// <summary>
        /// لاگ کردن عملیات
        /// </summary>
        protected void LogActivity(string action, string details)
        {
            try
            {
                // این می‌تواند به AuditRepository متصل شود
                System.Diagnostics.Debug.WriteLine(
                    string.Format("[Activity] User: {0}, Action: {1}, Details: {2}",
                    CurrentUserId, action, details));
            }
            catch
            {
                // خطای لاگ نباید مانع عملیات اصلی شود
            }
        }

        #endregion

        #region BindControls

        /// <summary>
        /// Bind کردن لیست کاربران
        /// </summary>
        protected void BindUsers(DropDownList ddl, bool includeInactive = false)
        {
            RBACHelper.BindUsersToDropDown(ddl, includeInactive);
        }

        /// <summary>
        /// Bind کردن لیست نقش‌ها
        /// </summary>
        protected void BindRoles(DropDownList ddl, bool includeInactive = false)
        {
            RBACHelper.BindRolesToDropDown(ddl, includeInactive);
        }

        /// <summary>
        /// Bind کردن لیست گروه‌ها
        /// </summary>
        protected void BindGroups(DropDownList ddl, bool includeInactive = false)
        {
            RBACHelper.BindGroupsToDropDown(ddl, includeInactive);
        }

        #endregion

        #region Exception Handling

        /// <summary>
        /// مدیریت خطاها به صورت متمرکز
        /// </summary>
        protected void HandleException(Exception ex, string operationName = "")
        {
            // لاگ کردن خطا
            LogError(operationName, ex);

            // نمایش پیام کاربرپسند
            string userMessage = GetUserFriendlyErrorMessage(ex);
            ShowErrorMessage(userMessage);
        }

        /// <summary>
        /// لاگ کردن خطا
        /// </summary>
        private void LogError(string operation, Exception ex)
        {
            try
            {
                string message = string.Format(
                    "[ERROR] User: {0}, Page: {1}, Operation: {2}, Error: {3}",
                    CurrentUserId,
                    Request.Url.PathAndQuery,
                    operation,
                    ex.Message
                );

                System.Diagnostics.Debug.WriteLine(message);

                // TODO: می‌توانید به یک سیستم لاگ خارجی متصل کنید
            }
            catch
            {
                // خطای لاگ نباید مانع عملیات شود
            }
        }

        /// <summary>
        /// تبدیل خطای فنی به پیام کاربرپسند
        /// </summary>
        private string GetUserFriendlyErrorMessage(Exception ex)
        {
            // خطاهای شناخته شده
            if (ex is ArgumentNullException || ex is ArgumentException)
                return ex.Message;

            if (ex is InvalidOperationException)
                return ex.Message;

            if (ex is UnauthorizedAccessException)
                return "شما دسترسی به این عملیات را ندارید";

            // خطای عمومی
            return "خطایی در انجام عملیات رخ داد. لطفاً مجدداً تلاش کنید";
        }

        #endregion
    }

    // ════════════════════════════════════════════════════════════════════════════════════════════════
    // مثال استفاده:
    // ════════════════════════════════════════════════════════════════════════════════════════════════
    /*

    public partial class UserManagement : BasePage
    {
        // صفحه نیاز به دسترسی Users.Manage دارد
        protected override string[] RequiredPermissions
        {
            get { return new string[] { "Users.Manage" }; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // CurrentUser و CurrentUserId در دسترس هستند
                lblWelcome.Text = "خوش آمدید " + CurrentUser.FullName;
            
                // دسترسی به RBACManager
                BindUsers(ddlUsers);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // انجام عملیات
                // ...
            
                ShowSuccessMessage("عملیات با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                HandleException(ex, "SaveUser");
            }
        }
    }

    */

}