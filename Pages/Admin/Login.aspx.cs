using System;
using System.Web;
using System.Web.UI;
using RBACSystem.BusinessLogic.RBAC;
using RBACSystem.Models.RBAC;
using RBACSystem.Utilities;

// ═══════════════════════════════════════════════════════════════════
// Login.aspx.cs - کد پشت صفحه ورود
// بدون namespace - مستقیم در پوشه Root
// ═══════════════════════════════════════════════════════════════════

public partial class Pages_Admin_Login : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // اگر قبلاً لاگین کرده، به صفحه اصلی برو
            if (SecurityHelper.IsAuthenticated())
            {
                string returnUrl = Request.QueryString["ReturnUrl"];

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    Response.Redirect(returnUrl, false);
                }
                else
                {
                    Response.Redirect("~/Pages/AdminTest.aspx", false);
                }

                Context.ApplicationInstance.CompleteRequest();
            }
        }
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        try
        {
            // ─────────────────────────────────────────────────────
            // 1. Validation ورودی‌ها
            // ─────────────────────────────────────────────────────

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username))
            {
                ShowError("نام کاربری را وارد کنید");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("رمز عبور را وارد کنید");
                return;
            }

            // ─────────────────────────────────────────────────────
            // 2. احراز هویت
            // ─────────────────────────────────────────────────────

            var userManager = new UserManager();
            User user = userManager.Login(username, password);

            if (user == null)
            {
                ShowError("نام کاربری یا رمز عبور اشتباه است");
                return;
            }

            // ─────────────────────────────────────────────────────
            // 3. ذخیره در Session
            // ─────────────────────────────────────────────────────

            SecurityHelper.SetCurrentUser(user);

            // ─────────────────────────────────────────────────────
            // 4. Remember Me
            // ─────────────────────────────────────────────────────

            if (chkRememberMe.Checked)
            {
                // ذخیره در Cookie (برای 30 روز)
                HttpCookie cookie = new HttpCookie("RememberMe");
                cookie.Values["Username"] = username;
                cookie.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Add(cookie);
            }

            // ─────────────────────────────────────────────────────
            // 5. Redirect به صفحه مقصد
            // ─────────────────────────────────────────────────────

            string returnUrl = Request.QueryString["ReturnUrl"];

            if (!string.IsNullOrEmpty(returnUrl))
            {
                Response.Redirect(returnUrl, false);
                Context.ApplicationInstance.CompleteRequest();
            }
            else
            {
                // بر اساس نقش کاربر، به صفحه مناسب برو
                string targetUrl = GetDefaultPageByRole(user);
                Response.Redirect(targetUrl, false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            ShowError(ex.Message);
        }
        catch (System.Threading.ThreadAbortException)
        {
            // این Exception طبیعی است و باید نادیده گرفته شود
        }
        catch (Exception ex)
        {
            ShowError("خطا در ورود به سیستم: " + ex.Message);

            // لاگ کردن خطا
            System.Diagnostics.Debug.WriteLine("[Login Error]: " + ex.ToString());
        }
    }

    // ═════════════════════════════════════════════════════════════════
    // Helper Methods
    // ═════════════════════════════════════════════════════════════════

    private string GetDefaultPageByRole(User user)
    {
        try
        {
            var rbacManager = new RBACManager();
            var roles = rbacManager.GetUserRoles(user.UserId);

            // بر اساس اولویت نقش، صفحه پیش‌فرض را تعیین کن
            foreach (var role in roles)
            {
                if (role.RoleName == "SuperAdmin" || role.RoleName == "Admin")
                    return "~/Pages/AdminTest.aspx";

                if (role.RoleName == "Manager")
                    return "~/Pages/AdminTest.aspx";
            }

            // کاربر عادی
            return "~/Pages/AdminTest.aspx";
        }
        catch
        {
            return "~/Pages/AdminTest.aspx";
        }
    }

    private void ShowError(string message)
    {
        pnlMessage.Visible = true;
        lblMessage.Text = message;
        lblMessage.CssClass = "alert alert-error";
    }

    private void ShowSuccess(string message)
    {
        pnlMessage.Visible = true;
        lblMessage.Text = message;
        lblMessage.CssClass = "alert alert-success";
    }
}