// ====================================================================================================
// Test.aspx.cs - صفحه تست سیستم RBAC
// سازگار با .NET Framework 4.8.1 / C# 5.0
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RBACSystem.Models.RBAC;
using RBACSystem.DataAccess.RBAC;
using RBACSystem.BusinessLogic.RBAC;
using RBACSystem.Utilities;


public partial class Pages_Test : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ShowWelcomeMessage();
        }
    }

    private void ShowWelcomeMessage()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<div class='result-box'>");
        sb.Append("<span class='info'>✅ صفحه تست آماده است!</span><br />");
        sb.Append("<span class='info'>📌 لطفاً ابتدا 'تست اتصال به دیتابیس' را اجرا کنید.</span><br />");
        sb.Append("<span class='info'>📌 سپس می‌توانید تست‌های دیگر را اجرا کنید.</span><br />");
        sb.Append("<span class='info'>💡 توصیه: از دکمه 'اجرای تمام تست‌ها' استفاده کنید.</span>");
        sb.Append("</div>");

        litConnectionResult.Text = sb.ToString();
    }

    // ====================================================================================================
    // 1. تست اتصال به دیتابیس
    // ====================================================================================================

    protected void btnTestConnection_Click(object sender, EventArgs e)
    {
        StringBuilder result = new StringBuilder();
        result.Append("<div class='result-box'>");

        try
        {
            result.Append("<span class='info'>⏳ در حال تست اتصال به دیتابیس...</span><br />");

            string connectionString = BankLink.GetConnectionString();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                result.Append("<span class='success'>✅ اتصال به دیتابیس موفق بود!</span><br />");
                result.Append("<span class='info'>📊 Server: " + conn.DataSource + "</span><br />");
                result.Append("<span class='info'>📊 Database: " + conn.Database + "</span><br />");
                result.Append("<span class='info'>📊 نسخه SQL Server: " + conn.ServerVersion + "</span><br />");
                conn.Close();
            }
        }
        catch (Exception ex)
        {
            result.Append("<span class='error'>❌ خطا در اتصال به دیتابیس!</span><br />");
            result.Append("<span class='error'>پیام خطا: " + ex.Message + "</span><br />");
        }

        result.Append("</div>");
        litConnectionResult.Text = result.ToString();
    }

    // ====================================================================================================
    // 2. تست عملیات کاربران
    // ====================================================================================================

    protected void btnTestCreateUser_Click(object sender, EventArgs e)
    {
        StringBuilder result = new StringBuilder();
        result.Append("<div class='result-box'>");

        try
        {
            result.Append("<span class='info'>⏳ در حال ایجاد کاربر تستی...</span><br />");

            var userManager = new UserManager();

            // ایجاد کاربر تستی
            string testUsername = "test_" + DateTime.Now.Ticks;
            var user = new User
            {
                Username = testUsername,
                Email = testUsername + "@test.com",
                FullName = "کاربر تستی",
                IsActive = true
            };

            int newUserId = userManager.CreateUser(user, "123456", 1);

            if (newUserId > 0)
            {
                result.Append("<span class='success'>✅ کاربر با موفقیت ایجاد شد!</span><br />");
                result.Append("<span class='info'>👤 شناسه: " + newUserId + "</span><br />");
                result.Append("<span class='info'>👤 نام کاربری: " + user.Username + "</span><br />");
                result.Append("<span class='info'>👤 ایمیل: " + user.Email + "</span><br />");
                result.Append("<span class='info'>🔑 رمز عبور: 123456</span><br />");
            }
            else
            {
                result.Append("<span class='error'>❌ خطا در ایجاد کاربر!</span><br />");
            }
        }
        catch (Exception ex)
        {
            result.Append("<span class='error'>❌ خطا: " + ex.Message + "</span><br />");
        }

        result.Append("</div>");
        litUserResult.Text = result.ToString();
    }

    protected void btnTestGetUsers_Click(object sender, EventArgs e)
    {
        StringBuilder result = new StringBuilder();
        result.Append("<div class='result-box'>");

        try
        {
            result.Append("<span class='info'>⏳ در حال دریافت کاربران...</span><br /><br />");

            var userManager = new UserManager();
            List<User> users = userManager.GetAllUsers(true);

            result.Append("<span class='success'>✅ تعداد کاربران: " + users.Count + "</span><br /><br />");

            foreach (var user in users)
            {
                string status = user.IsActive ? "✅ فعال" : "❌ غیرفعال";
                result.Append("👤 " + user.FullName + " (" + user.Username + ") - " + status + "<br />");
                result.Append("   📧 " + user.Email + "<br />");
                result.Append("   🆔 ID: " + user.UserId + "<br />");
                result.Append("   📅 تاریخ ایجاد: " + user.CreatedDate.ToString("yyyy/MM/dd HH:mm") + "<br /><br />");
            }
        }
        catch (Exception ex)
        {
            result.Append("<span class='error'>❌ خطا: " + ex.Message + "</span><br />");
        }

        result.Append("</div>");
        litUserResult.Text = result.ToString();
    }

    protected void btnTestLogin_Click(object sender, EventArgs e)
    {
        StringBuilder result = new StringBuilder();
        result.Append("<div class='result-box'>");

        try
        {
            result.Append("<span class='info'>⏳ در حال تست Login...</span><br />");

            var userManager = new UserManager();

            // تست Login با کاربر system
            User user = userManager.Login("system", "123456");

            if (user != null)
            {
                result.Append("<span class='success'>✅ Login موفق بود!</span><br />");
                result.Append("<span class='info'>👤 کاربر: " + user.FullName + "</span><br />");
                result.Append("<span class='info'>📧 ایمیل: " + user.Email + "</span><br />");
                result.Append("<span class='info'>🆔 شناسه: " + user.UserId + "</span><br />");

                // ذخیره در Session
                SecurityHelper.SetCurrentUser(user);
                result.Append("<span class='success'>✅ اطلاعات در Session ذخیره شد!</span><br />");
            }
            else
            {
                result.Append("<span class='error'>❌ Login ناموفق بود!</span><br />");
            }
        }
        catch (Exception ex)
        {
            result.Append("<span class='error'>❌ خطا: " + ex.Message + "</span><br />");
        }

        result.Append("</div>");
        litUserResult.Text = result.ToString();
    }

    // ====================================================================================================
    // 3. تست عملیات نقش‌ها
    // ====================================================================================================

    protected void btnTestGetRoles_Click(object sender, EventArgs e)
    {
        StringBuilder result = new StringBuilder();
        result.Append("<div class='result-box'>");

        try
        {
            result.Append("<span class='info'>⏳ در حال دریافت نقش‌ها...</span><br /><br />");

            var roleManager = new RoleManager();
            List<Role> roles = roleManager.GetAllRoles(true);

            result.Append("<span class='success'>✅ تعداد نقش‌ها: " + roles.Count + "</span><br /><br />");

            foreach (var role in roles)
            {
                string status = role.IsActive ? "✅ فعال" : "❌ غیرفعال";
                result.Append("🎭 " + role.RoleName + " - " + status + "<br />");
                result.Append("   📝 " + (role.Description ?? "بدون توضیحات") + "<br />");
                result.Append("   🔢 اولویت: " + role.Priority + "<br />");
                result.Append("   👥 تعداد کاربران: " + role.UserCount + "<br /><br />");
            }
        }
        catch (Exception ex)
        {
            result.Append("<span class='error'>❌ خطا: " + ex.Message + "</span><br />");
        }

        result.Append("</div>");
        litRoleResult.Text = result.ToString();
    }

    protected void btnTestAssignRole_Click(object sender, EventArgs e)
    {
        StringBuilder result = new StringBuilder();
        result.Append("<div class='result-box'>");

        try
        {
            result.Append("<span class='info'>⏳ در حال تخصیص نقش...</span><br />");

            var roleManager = new RoleManager();

            // فرض کنیم کاربر با ID=2 وجود داره
            // و می‌خوایم نقش User (RoleId=4) رو بهش بدیم
            bool assigned = roleManager.AssignRoleToUser(2, 4, 1);

            if (assigned)
            {
                result.Append("<span class='success'>✅ نقش با موفقیت تخصیص یافت!</span><br />");
            }
            else
            {
                result.Append("<span class='warning'>⚠️ نقش قبلاً تخصیص داده شده بود!</span><br />");
            }
        }
        catch (Exception ex)
        {
            result.Append("<span class='error'>❌ خطا: " + ex.Message + "</span><br />");
            result.Append("<span class='info'>💡 توجه: ممکن است کاربر یا نقش وجود نداشته باشد.</span><br />");
        }

        result.Append("</div>");
        litRoleResult.Text = result.ToString();
    }

    protected void btnTestGetUserRoles_Click(object sender, EventArgs e)
    {
        StringBuilder result = new StringBuilder();
        result.Append("<div class='result-box'>");

        try
        {
            result.Append("<span class='info'>⏳ در حال دریافت نقش‌های کاربر...</span><br /><br />");

            var roleManager = new RoleManager();

            // دریافت نقش‌های کاربر با ID=1
            List<Role> roles = roleManager.GetUserRoles(2);

            result.Append("<span class='success'>✅ تعداد نقش‌ها: " + roles.Count + "</span><br /><br />");

            foreach (var role in roles)
            {
                result.Append("🎭 " + role.RoleName + "<br />");
                result.Append("   📝 " + (role.Description ?? "بدون توضیحات") + "<br />");
                result.Append("   🔢 اولویت: " + role.Priority + "<br /><br />");
            }
        }
        catch (Exception ex)
        {
            result.Append("<span class='error'>❌ خطا: " + ex.Message + "</span><br />");
        }

        result.Append("</div>");
        litRoleResult.Text = result.ToString();
    }

    // ====================================================================================================
    // 4. تست عملیات دسترسی‌ها
    // ====================================================================================================

    protected void btnTestGetPermissions_Click(object sender, EventArgs e)
    {
        StringBuilder result = new StringBuilder();
        result.Append("<div class='result-box'>");

        try
        {
            result.Append("<span class='info'>⏳ در حال دریافت دسترسی‌ها...</span><br /><br />");

            var authManager = new AuthorizationManager();
            List<Permission> permissions = authManager.GetAllPermissions(false);

            result.Append("<span class='success'>✅ تعداد دسترسی‌ها: " + permissions.Count + "</span><br /><br />");

            string currentCategory = "";
            foreach (var perm in permissions)
            {
                if (currentCategory != perm.Category)
                {
                    currentCategory = perm.Category;
                    result.Append("<br /><strong>📁 " + currentCategory + ":</strong><br />");
                }
                result.Append("   🔐 " + perm.PermissionName + " - " + perm.DisplayName + "<br />");
            }
        }
        catch (Exception ex)
        {
            result.Append("<span class='error'>❌ خطا: " + ex.Message + "</span><br />");
        }

        result.Append("</div>");
        litPermissionResult.Text = result.ToString();
    }

    protected void btnTestUserPermissions_Click(object sender, EventArgs e)
    {
        StringBuilder result = new StringBuilder();
        result.Append("<div class='result-box'>");

        try
        {
            result.Append("<span class='info'>⏳ در حال دریافت دسترسی‌های کاربر...</span><br /><br />");

            var authManager = new AuthorizationManager();

            // دریافت دسترسی‌های کاربر با ID=5 (System/SuperAdmin)
            List<string> permissions = authManager.GetUserPermissions(5);

            result.Append("<span class='success'>✅ تعداد دسترسی‌های کاربر: " + permissions.Count + "</span><br /><br />");

            foreach (var perm in permissions)
            {
                result.Append("🔐 " + perm + "<br />");
            }
        }
        catch (Exception ex)
        {
            result.Append("<span class='error'>❌ خطا: " + ex.Message + "</span><br />");
        }

        result.Append("</div>");
        litPermissionResult.Text = result.ToString();
    }

    protected void btnTestCheckPermission_Click(object sender, EventArgs e)
    {
        StringBuilder result = new StringBuilder();
        result.Append("<div class='result-box'>");

        try
        {
            result.Append("<span class='info'>⏳ در حال بررسی دسترسی...</span><br /><br />");

            var authManager = new AuthorizationManager();

            // بررسی دسترسی Users.View برای کاربر با ID=5
            bool hasPermission = authManager.UserHasPermission(5, "Users.View");

            if (hasPermission)
            {
                result.Append("<span class='success'>✅ کاربر دسترسی 'Users.View' را دارد!</span><br />");
            }
            else
            {
                result.Append("<span class='error'>❌ کاربر دسترسی 'Users.View' را ندارد!</span><br />");
            }

            // تست چند دسترسی
            result.Append("<br /><strong>بررسی سایر دسترسی‌ها:</strong><br />");

            string[] testPermissions = { "Users.Create", "Users.Edit", "Users.Delete", "Roles.View", "Settings.Edit" };

            foreach (string perm in testPermissions)
            {
                bool has = authManager.UserHasPermission(1, perm);
                string icon = has ? "✅" : "❌";
                result.Append(icon + " " + perm + "<br />");
            }
        }
        catch (Exception ex)
        {
            result.Append("<span class='error'>❌ خطا: " + ex.Message + "</span><br />");
        }

        result.Append("</div>");
        litPermissionResult.Text = result.ToString();
    }

    // ====================================================================================================
    // 5. تست Cache
    // ====================================================================================================

    protected void btnTestCache_Click(object sender, EventArgs e)
    {
        StringBuilder result = new StringBuilder();
        result.Append("<div class='result-box'>");

        try
        {
            result.Append("<span class='info'>⏳ در حال تست Cache...</span><br /><br />");

            var authManager = new AuthorizationManager();

            // تست 1: اولین بار (از DB)
            DateTime start1 = DateTime.Now;
            List<string> permissions1 = authManager.GetUserPermissions(1);
            TimeSpan time1 = DateTime.Now - start1;

            result.Append("<strong>🔄 بار اول (از Database):</strong><br />");
            result.Append("   ⏱️ زمان: " + time1.TotalMilliseconds + " ms<br />");
            result.Append("   📊 تعداد: " + permissions1.Count + " دسترسی<br /><br />");

            // تست 2: بار دوم (از Cache)
            DateTime start2 = DateTime.Now;
            List<string> permissions2 = authManager.GetUserPermissions(1);
            TimeSpan time2 = DateTime.Now - start2;

            result.Append("<strong>⚡ بار دوم (از Cache):</strong><br />");
            result.Append("   ⏱️ زمان: " + time2.TotalMilliseconds + " ms<br />");
            result.Append("   📊 تعداد: " + permissions2.Count + " دسترسی<br /><br />");

            // محاسبه بهبود
            double improvement = ((time1.TotalMilliseconds - time2.TotalMilliseconds) / time1.TotalMilliseconds) * 100;

            result.Append("<span class='success'>✅ بهبود سرعت: " + improvement.ToString("0.00") + "%</span><br />");

            if (time2.TotalMilliseconds < time1.TotalMilliseconds)
            {
                result.Append("<span class='success'>🚀 Cache به درستی کار می‌کند!</span><br />");
            }
        }
        catch (Exception ex)
        {
            result.Append("<span class='error'>❌ خطا: " + ex.Message + "</span><br />");
        }

        result.Append("</div>");
        litCacheResult.Text = result.ToString();
    }

    protected void btnClearCache_Click(object sender, EventArgs e)
    {
        StringBuilder result = new StringBuilder();
        result.Append("<div class='result-box'>");

        try
        {
            var authManager = new AuthorizationManager();
            authManager.ClearAllCache();

            result.Append("<span class='success'>✅ تمام Cache ها پاک شدند!</span><br />");
        }
        catch (Exception ex)
        {
            result.Append("<span class='error'>❌ خطا: " + ex.Message + "</span><br />");
        }

        result.Append("</div>");
        litCacheResult.Text = result.ToString();
    }

    // ====================================================================================================
    // 6. تست SecurityHelper
    // ====================================================================================================

    protected void btnTestHash_Click(object sender, EventArgs e)
    {
        StringBuilder result = new StringBuilder();
        result.Append("<div class='result-box'>");

        try
        {
            result.Append("<span class='info'>⏳ در حال تست Hash کردن...</span><br /><br />");

            string password1 = "123456";
            string password2 = "123456";
            string password3 = "654321";

            string hash1 = SecurityHelper.HashPassword(password1);
            string hash2 = SecurityHelper.HashPassword(password2);
            string hash3 = SecurityHelper.HashPassword(password3);

            result.Append("<strong>تست 1: رمز یکسان</strong><br />");
            result.Append("   رمز 1: " + password1 + "<br />");
            result.Append("   Hash 1: " + hash1 + "<br />");
            result.Append("   رمز 2: " + password2 + "<br />");
            result.Append("   Hash 2: " + hash2 + "<br />");

            if (hash1 == hash2)
            {
                result.Append("   <span class='success'>✅ Hash یکسان است (درست!)</span><br /><br />");
            }
            else
            {
                result.Append("   <span class='error'>❌ Hash متفاوت است (اشتباه!)</span><br /><br />");
            }

            result.Append("<strong>تست 2: رمز متفاوت</strong><br />");
            result.Append("   رمز 3: " + password3 + "<br />");
            result.Append("   Hash 3: " + hash3 + "<br />");

            if (hash1 != hash3)
            {
                result.Append("   <span class='success'>✅ Hash متفاوت است (درست!)</span><br />");
            }
            else
            {
                result.Append("   <span class='error'>❌ Hash یکسان است (اشتباه!)</span><br />");
            }
        }
        catch (Exception ex)
        {
            result.Append("<span class='error'>❌ خطا: " + ex.Message + "</span><br />");
        }

        result.Append("</div>");
        litSecurityResult.Text = result.ToString();
    }

    protected void btnTestSession_Click(object sender, EventArgs e)
    {
        StringBuilder result = new StringBuilder();
        result.Append("<div class='result-box'>");

        try
        {
            result.Append("<span class='info'>⏳ در حال تست Session...</span><br /><br />");

            // بررسی وضعیت فعلی
            bool isAuth = SecurityHelper.IsAuthenticated();
            result.Append("<strong>وضعیت فعلی:</strong><br />");
            result.Append("   " + (isAuth ? "✅ کاربر لاگین کرده" : "❌ کاربر لاگین نکرده") + "<br /><br />");

            if (isAuth)
            {
                int userId = SecurityHelper.GetCurrentUserId();
                string username = SecurityHelper.GetCurrentUsername();

                result.Append("<strong>اطلاعات Session:</strong><br />");
                result.Append("   🆔 شناسه: " + userId + "<br />");
                result.Append("   👤 نام کاربری: " + username + "<br />");
                result.Append("   📍 IP: " + SecurityHelper.GetUserIP() + "<br />");
                result.Append("   🌐 User Agent: " + SecurityHelper.GetUserAgent() + "<br />");
            }
            else
            {
                result.Append("<span class='info'>💡 برای تست، ابتدا دکمه 'تست Login' را بزنید.</span><br />");
            }
        }
        catch (Exception ex)
        {
            result.Append("<span class='error'>❌ خطا: " + ex.Message + "</span><br />");
        }

        result.Append("</div>");
        litSecurityResult.Text = result.ToString();
    }

    protected void btnTestValidation_Click(object sender, EventArgs e)
    {
        StringBuilder result = new StringBuilder();
        result.Append("<div class='result-box'>");

        try
        {
            result.Append("<span class='info'>⏳ در حال تست Validation...</span><br /><br />");

            // تست Email
            string[] testEmails = { "test@example.com", "invalid-email", "user@domain", "good.email@test.co.ir" };

            result.Append("<strong>تست اعتبار ایمیل:</strong><br />");
            foreach (string email in testEmails)
            {
                bool isValid = SecurityHelper.IsValidEmail(email);
                string icon = isValid ? "✅" : "❌";
                result.Append("   " + icon + " " + email + "<br />");
            }

            // تست Password Strength
            string[] testPasswords = { "123", "123456", "Pass123", "StrongP@ss123" };

            result.Append("<br /><strong>تست قدرت رمز عبور:</strong><br />");
            foreach (string pass in testPasswords)
            {
                string strength = SecurityHelper.CheckPasswordStrength(pass);
                result.Append("   🔑 " + pass + " → " + strength + "<br />");
            }
        }
        catch (Exception ex)
        {
            result.Append("<span class='error'>❌ خطا: " + ex.Message + "</span><br />");
        }

        result.Append("</div>");
        litSecurityResult.Text = result.ToString();
    }

    // ====================================================================================================
    // آمار سیستم
    // ====================================================================================================

    protected void btnGetStats_Click(object sender, EventArgs e)
    {
        StringBuilder result = new StringBuilder();

        try
        {
            var userRepo = new UserRepository();
            var roleRepo = new RoleRepository();
            var permissionRepo = new PermissionRepository();

            List<User> users = userRepo.GetAllUsers(true);
            List<Role> roles = roleRepo.GetAllRoles(true);
            List<Permission> permissions = permissionRepo.GetAllPermissions(true);

            int activeUsers = 0;
            foreach (var u in users)
            {
                if (u.IsActive) activeUsers++;
            }

            result.Append("<div class='stats-grid'>");
            result.Append("<div class='stat-card'>");
            result.Append("<h3>" + users.Count + "</h3>");
            result.Append("<p>کل کاربران</p>");
            result.Append("</div>");

            result.Append("<div class='stat-card'>");
            result.Append("<h3>" + activeUsers + "</h3>");
            result.Append("<p>کاربران فعال</p>");
            result.Append("</div>");

            result.Append("<div class='stat-card'>");
            result.Append("<h3>" + roles.Count + "</h3>");
            result.Append("<p>نقش‌ها</p>");
            result.Append("</div>");

            result.Append("<div class='stat-card'>");
            result.Append("<h3>" + permissions.Count + "</h3>");
            result.Append("<p>دسترسی‌ها</p>");
            result.Append("</div>");
            result.Append("</div>");
        }
        catch (Exception ex)
        {
            result.Append("<div class='result-box'>");
            result.Append("<span class='error'>❌ خطا: " + ex.Message + "</span>");
            result.Append("</div>");
        }

        litStats.Text = result.ToString();
    }

    // ====================================================================================================
    // اجرای تمام تست‌ها
    // ====================================================================================================

    protected void btnRunAllTests_Click(object sender, EventArgs e)
    {
        btnTestConnection_Click(sender, e);
        btnTestGetUsers_Click(sender, e);
        btnTestGetRoles_Click(sender, e);
        btnTestGetPermissions_Click(sender, e);
        btnTestUserPermissions_Click(sender, e);
        btnTestCache_Click(sender, e);
        btnTestHash_Click(sender, e);
        btnGetStats_Click(sender, e);
    }
}