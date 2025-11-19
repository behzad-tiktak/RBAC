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

public partial class Pages_AdminTest : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadInitialData();
        }
    }
    private void LoadInitialData()
    {
        LoadUsers();
        LoadRoles();
        LoadPermissions();
        LoadRolesForPermissionManagement();
        UpdateCurrentUserDisplay();
        UpdateStats();
    }

    private void LoadUsers()
    {
        try
        {
            var userManager = new UserManager();
            List<User> users = userManager.GetAllUsers(true);
            
            ddlUsers.Items.Clear();
            ddlUsers.Items.Add(new ListItem("-- انتخاب کاربر --", "0"));
            
            foreach (User user in users)
            {
                string status = user.IsActive ? "" : " (غیرفعال)";
                ddlUsers.Items.Add(new ListItem(
                    user.FullName + " - " + user.Username + status, 
                    user.UserId.ToString()
                ));
            }
        }
        catch (Exception ex)
        {
            ShowError("خطا در بارگذاری کاربران: " + ex.Message);
        }
    }

    private void LoadRoles()
    {
        try
        {
            var roleManager = new RoleManager();
            List<Role> roles = roleManager.GetAllRoles(false);
            
            cblRoles.Items.Clear();
            
            foreach (Role role in roles)
            {
                cblRoles.Items.Add(new ListItem(role.RoleName + " - " + role.Description, role.RoleId.ToString()));
                
            }
        }
        catch (Exception ex)
        {
            ShowError("خطا در بارگذاری نقش‌ها: " + ex.Message);
        }
    }

    private void LoadPermissions()
    {
        try
        {
            var authManager = new AuthorizationManager();
            List<Permission> permissions = authManager.GetAllPermissions(false);
            
            cblPermissions.Items.Clear();
            
            string currentCategory = "";
            foreach (Permission perm in permissions)
            {
                if (currentCategory != perm.Category)
                {
                    currentCategory = perm.Category;
                    // اضافه کردن Header (غیرفعال)
                    ListItem header = new ListItem("═══ " + currentCategory + " ═══", "");
                    header.Attributes.Add("disabled", "disabled");
                    header.Attributes.Add("style", "font-weight:bold; background:#f0f0f0;");
                    cblPermissions.Items.Add(header);
                }
                
                cblPermissions.Items.Add(new ListItem(
                    "   " + perm.DisplayName + " (" + perm.PermissionName + ")", 
                    perm.PermissionId.ToString()
                ));
            }
        }
        catch (Exception ex)
        {
            ShowError("خطا در بارگذاری دسترسی‌ها: " + ex.Message);
        }
    }

    private void LoadRolesForPermissionManagement()
    {
        try
        {
            var roleManager = new RoleManager();
            List<Role> roles = roleManager.GetAllRoles(false);
            
            ddlRolesForPermission.Items.Clear();
            ddlRolesForPermission.Items.Add(new ListItem("-- انتخاب نقش --", "0"));
            
            foreach (Role role in roles)
            {
                ddlRolesForPermission.Items.Add(new ListItem(
                    role.RoleName, 
                    role.RoleId.ToString()
                ));
            }
        }
        catch (Exception ex)
        {
            ShowError("خطا: " + ex.Message);
        }
    }

    // ====================================================================================================
    // آمار
    // ====================================================================================================

    protected void btnRefreshStats_Click(object sender, EventArgs e)
    {
        UpdateStats();
        upSidebar.Update();
    }

    private void UpdateStats()
    {
        try
        {
            UserRepository userRepo = new UserRepository();
            RoleRepository roleRepo = new RoleRepository();
            
            // ✅ استفاده از Cache برای نقش‌ها
            List<User> users = userRepo.GetAllUsers(true);
            List<Role> roles = roleRepo.GetAllRoles(true);
            
            litTotalUsers.Text = users.Count.ToString();
            litTotalRoles.Text = roles.Count.ToString();
        }
        catch
        {
            litTotalUsers.Text = "0";
            litTotalRoles.Text = "0";
        }
    }

    // ====================================================================================================
    // متدهای کمکی
    // ====================================================================================================

    private bool EnsureUserLoggedIn()
    {
        if (!SecurityHelper.IsAuthenticated())
        {
            ShowWarning("لطفاً ابتدا یک کاربر را از Sidebar انتخاب کنید و در Session بارگذاری کنید");
            return false;
        }
        return true;
    }

    private void ShowResult(Panel panel, Literal literal, string message)
    {
        literal.Text = message;
        panel.Visible = true;
        upContent.Update();
    }

    private void ShowSuccess(string message)
    {
        string html = "<div class='success-box'><h4>✅ " + message + "</h4></div>";
        ShowResult(pnlUserResult, litUserResult, html);
    }

    private void ShowError(string message)
    {
        string html = "<div class='error-box'><h4>❌ " + message + "</h4></div>";
        ShowResult(pnlUserResult, litUserResult, html);
    }

    private void ShowWarning(string message)
    {
        string html = "<div class='error-box'><h4>⚠️ " + message + "</h4></div>";
        ShowResult(pnlUserResult, litUserResult, html);
    }

    private void ShowInfo(string message)
    {
        string html = "<div class='info-box'><h4>ℹ️ " + message + "</h4></div>";
        ShowResult(pnlUserResult, litUserResult, html);
    }


// ====================================================================================================
// ✅ تمامی قابلیت‌ها:
// ====================================================================================================
// 
// 1. ✅ انتخاب کاربر و بارگذاری در Session
// 2. ✅ نمایش اطلاعات کامل کاربر
// 3. ✅ نمایش نقش‌های کاربر
// 4. ✅ نمایش دسترسی‌های کاربر (با Cache)
// 5. ✅ تست Login
// 6. ✅ تخصیص نقش به کاربر
// 7. ✅ حذف نقش از کاربر
// 8. ✅ تخصیص دسترسی به نقش
// 9. ✅ حذف دسترسی از نقش
// 10. ✅ تست Performance Cache
// 11. ✅ پاک کردن Cache
// 12. ✅ ایجاد کاربر جدید
// 13. ✅ بروزرسانی کاربر
// 14. ✅ حذف کاربر
// 15. ✅ UpdatePanel برای جلوگیری از Postback کامل صفحه
// 16. ✅ مدیریت Session
// 17. ✅ آمار سیستم
//
// ====================================================================================================
    // ====================================================================================================
    // مدیریت Session و کاربر فعلی
    // ====================================================================================================

    protected void ddlUsers_SelectedIndexChanged(object sender, EventArgs e)
    {
        upSidebar.Update();
    }

    protected void btnLoadUser_Click(object sender, EventArgs e)
    {
        try
        {
            int userId = Convert.ToInt32(ddlUsers.SelectedValue);
            
            if (userId == 0)
            {
                ShowWarning("لطفاً یک کاربر انتخاب کنید");
                return;
            }
            var userManager = new UserManager();
            User user = userManager.GetUserById(userId);
            
            if (user != null)
            {
                SecurityHelper.SetCurrentUser(user);
                UpdateCurrentUserDisplay();
                
                ShowSuccess("کاربر '" + user.FullName + "' در Session بارگذاری شد");
            }
            else
            {
                ShowError("کاربر یافت نشد");
            }
        }
        catch (Exception ex)
        {
            ShowError("خطا: " + ex.Message);
        }

        upSidebar.Update();
    }

    protected void btnLogout_Click(object sender, EventArgs e)
    {
        SecurityHelper.Logout();
        UpdateCurrentUserDisplay();
        ShowInfo("از Session خارج شدید");
        
        upSidebar.Update();
    }

    private void UpdateCurrentUserDisplay()
    {
        if (SecurityHelper.IsAuthenticated())
        {
            User currentUser = SecurityHelper.GetCurrentUser();
            
            StringBuilder sb = new StringBuilder();
            sb.Append("<p><strong>" + currentUser.FullName + "</strong></p>");
            sb.Append("<p>@" + currentUser.Username + "</p>");
            sb.Append("<p>🆔 " + currentUser.UserId + "</p>");
            sb.Append("<p>📧 " + currentUser.Email + "</p>");
            
            litCurrentUser.Text = sb.ToString();
        }
        else
        {
            litCurrentUser.Text = "<p>❌ هیچ کاربری در Session نیست</p><p>لطفاً یک کاربر انتخاب کنید</p>";
        }
    }

    // ====================================================================================================
    // نمایش اطلاعات کاربر
    // ====================================================================================================

    protected void btnShowUserInfo_Click(object sender, EventArgs e)
    {
        if (!EnsureUserLoggedIn()) return;

        User user = SecurityHelper.GetCurrentUser();
        
        StringBuilder sb = new StringBuilder();
        sb.Append("<div class='info-box'>");
        sb.Append("<h4>📋 اطلاعات کامل کاربر</h4>");
        sb.Append("<p><strong>شناسه:</strong> " + user.UserId + "</p>");
        sb.Append("<p><strong>نام کاربری:</strong> " + user.Username + "</p>");
        sb.Append("<p><strong>نام کامل:</strong> " + user.FullName + "</p>");
        sb.Append("<p><strong>ایمیل:</strong> " + user.Email + "</p>");
        sb.Append("<p><strong>وضعیت:</strong> " + (user.IsActive ? "✅ فعال" : "❌ غیرفعال") + "</p>");
        sb.Append("<p><strong>تاریخ ایجاد:</strong> " + user.CreatedDate.ToString("yyyy/MM/dd HH:mm") + "</p>");
        
        if (user.LastLoginDate.HasValue)
        {
            sb.Append("<p><strong>آخرین ورود:</strong> " + user.LastLoginDate.Value.ToString("yyyy/MM/dd HH:mm") + "</p>");
        }
        
        sb.Append("</div>");
        
        ShowResult(pnlUserResult, litUserResult, sb.ToString());
    }

    protected void btnShowUserRoles_Click(object sender, EventArgs e)
    {
        if (!EnsureUserLoggedIn()) return;

        int userId = SecurityHelper.GetCurrentUserId();
        
        try
        {
            var roleManager = new RoleManager();
            List<Role> roles = roleManager.GetUserRoles(userId);
            
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='info-box'>");
            sb.Append("<h4>🎭 نقش‌های کاربر (" + roles.Count + ")</h4>");
            
            if (roles.Count > 0)
            {
                foreach (Role role in roles)
                {
                    sb.Append("<span class='badge badge-info'>" + role.RoleName + "</span>");
                }
            }
            else
            {
                sb.Append("<p>❌ هیچ نقشی تخصیص داده نشده</p>");
            }
            
            sb.Append("</div>");
            
            ShowResult(pnlUserResult, litUserResult, sb.ToString());
        }
        catch (Exception ex)
        {
            ShowError("خطا: " + ex.Message);
        }
    }

    protected void btnShowUserPermissions_Click(object sender, EventArgs e)
    {
        if (!EnsureUserLoggedIn()) return;

        int userId = SecurityHelper.GetCurrentUserId();
        
        try
        {
            // ✅ استفاده از Cache
            var authManager = new AuthorizationManager();
            List<string> permissions = authManager.GetUserPermissions(userId);
            
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='success-box'>");
            sb.Append("<h4>🔐 دسترسی‌های کاربر (" + permissions.Count + ")</h4>");
            sb.Append("<small>⚡ از Cache خوانده شد</small><br /><br />");
            
            if (permissions.Count > 0)
            {
                string currentCategory = "";
                foreach (string perm in permissions)
                {
                    string category = perm.Split('.')[0];
                    
                    if (category != currentCategory)
                    {
                        if (currentCategory != "") sb.Append("<br />");
                        currentCategory = category;
                        sb.Append("<strong>" + category + ":</strong><br />");
                    }
                    
                    sb.Append("<span class='badge badge-success'>" + perm + "</span>");
                }
            }
            else
            {
                sb.Append("<p>❌ هیچ دسترسی وجود ندارد</p>");
            }
            
            sb.Append("</div>");
            
            ShowResult(pnlUserResult, litUserResult, sb.ToString());
        }
        catch (Exception ex)
        {
            ShowError("خطا: " + ex.Message);
        }
    }

    protected void btnTestLogin_Click(object sender, EventArgs e)
    {
        if (!EnsureUserLoggedIn()) return;

        User user = SecurityHelper.GetCurrentUser();
        
        try
        {
            // تست Login با رمز عبور پیش‌فرض
            var userManager = new UserManager();
            User loginResult = userManager.Login(user.Username, "123456");
            
            StringBuilder sb = new StringBuilder();
            
            if (loginResult != null)
            {
                sb.Append("<div class='success-box'>");
                sb.Append("<h4>✅ Login موفق بود!</h4>");
                sb.Append("<p>نام کاربری: " + user.Username + "</p>");
                sb.Append("<p>رمز عبور: 123456</p>");
                sb.Append("</div>");
            }
            else
            {
                sb.Append("<div class='error-box'>");
                sb.Append("<h4>❌ Login ناموفق!</h4>");
                sb.Append("<p>رمز عبور اشتباه است یا کاربر غیرفعال است</p>");
                sb.Append("</div>");
            }
            
            ShowResult(pnlUserResult, litUserResult, sb.ToString());
        }
        catch (Exception ex)
        {
            ShowError("خطا: " + ex.Message);
        }
    }

    // ====================================================================================================
    // مدیریت نقش‌ها
    // ====================================================================================================

    protected void btnAssignRoles_Click(object sender, EventArgs e)
    {
        if (!EnsureUserLoggedIn()) return;

        int userId = SecurityHelper.GetCurrentUserId();
        int assignedBy = 1; // کاربر system
        int successCount = 0;
        int failCount = 0;        
        
        try
        {
            var roleManager = new RoleManager();
            var authManager = new AuthorizationManager();
            foreach (ListItem item in cblRoles.Items)
            {
                if (item.Selected)
                {
                    int roleId = Convert.ToInt32(item.Value);
                    
                    try
                    {                        
                        bool assigned = roleManager.AssignRoleToUser(userId, roleId, assignedBy);
                        if (assigned) successCount++;
                        else failCount++;
                    }
                    catch
                    {
                        failCount++;
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='success-box'>");
            sb.Append("<h4>✅ تخصیص نقش‌ها انجام شد</h4>");
            sb.Append("<p>موفق: " + successCount + "</p>");
            sb.Append("<p>ناموفق: " + failCount + "</p>");
            sb.Append("</div>");
            
            ShowResult(pnlRoleResult, litRoleResult, sb.ToString());
            
            // پاک کردن Cache
            authManager.ClearUserPermissionsCache(userId);
        }
        catch (Exception ex)
        {
            ShowError("خطا: " + ex.Message);
        }
    }

    protected void btnRemoveRoles_Click(object sender, EventArgs e)
    {
        if (!EnsureUserLoggedIn()) return;

        int userId = SecurityHelper.GetCurrentUserId();
        int removedBy = 1;
        int successCount = 0;        

        try
        {
            var roleManager = new RoleManager();            
            var authManager = new AuthorizationManager();
            foreach (ListItem item in cblRoles.Items)
            {
                if (item.Selected)
                {
                    int roleId = Convert.ToInt32(item.Value);
                    
                    bool removed = roleManager.RemoveRoleFromUser(userId, roleId, removedBy);
                    if (removed) successCount++;
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='success-box'>");
            sb.Append("<h4>✅ حذف نقش‌ها انجام شد</h4>");
            sb.Append("<p>تعداد حذف شده: " + successCount + "</p>");
            sb.Append("</div>");
            
            ShowResult(pnlRoleResult, litRoleResult, sb.ToString());
            
            // پاک کردن Cache
            authManager.ClearUserPermissionsCache(userId);
        }
        catch (Exception ex)
        {
            ShowError("خطا: " + ex.Message);
        }
    }

    protected void btnLoadRoles_Click(object sender, EventArgs e)
    {
        if (!EnsureUserLoggedIn()) return;

        int userId = SecurityHelper.GetCurrentUserId();
        
        try
        {
            // دریافت نقش‌های فعلی کاربر
            var roleManager = new RoleManager();
            List<Role> userRoles = roleManager.GetUserRoles(userId);
            
            // پاک کردن انتخاب‌ها
            foreach (ListItem item in cblRoles.Items)
            {
                item.Selected = false;
            }
            
            // انتخاب نقش‌های فعلی
            foreach (Role role in userRoles)
            {
                ListItem item = cblRoles.Items.FindByValue(role.RoleId.ToString());
                if (item != null)
                {
                    item.Selected = true;
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='info-box'>");
            sb.Append("<h4>✅ نقش‌های کاربر بارگذاری شد</h4>");
            sb.Append("<p>تعداد: " + userRoles.Count + "</p>");
            sb.Append("</div>");
            
            ShowResult(pnlRoleResult, litRoleResult, sb.ToString());
        }
        catch (Exception ex)
        {
            ShowError("خطا: " + ex.Message);
        }
    }

    // ====================================================================================================
    // مدیریت دسترسی‌ها
    // ====================================================================================================

    protected void ddlRolesForPermission_SelectedIndexChanged(object sender, EventArgs e)
    {
        int roleId = Convert.ToInt32(ddlRolesForPermission.SelectedValue);
        
        if (roleId == 0) return;

        try
        {
            // دریافت دسترسی‌های نقش
            var authManager = new AuthorizationManager();
            List<Permission> rolePermissions = authManager.GetRolePermissions(roleId);
            
            // پاک کردن انتخاب‌ها
            foreach (ListItem item in cblPermissions.Items)
            {
                item.Selected = false;
            }
            
            // انتخاب دسترسی‌های فعلی
            foreach (Permission perm in rolePermissions)
            {
                ListItem item = cblPermissions.Items.FindByValue(perm.PermissionId.ToString());
                if (item != null)
                {
                    item.Selected = true;
                }
            }
        }
        catch (Exception ex)
        {
            ShowError("خطا: " + ex.Message);
        }

        upContent.Update();
    }

    protected void btnAssignPermissions_Click(object sender, EventArgs e)
    {
        int roleId = Convert.ToInt32(ddlRolesForPermission.SelectedValue);
        
        if (roleId == 0)
        {
            ShowWarning("لطفاً یک نقش انتخاب کنید");
            return;
        }

        int assignedBy = 1;
        int successCount = 0;
        int failCount = 0;

        try
        {
            var authManager = new AuthorizationManager();
            foreach (ListItem item in cblPermissions.Items)
            {
                if (item.Selected && !string.IsNullOrEmpty(item.Value))
                {
                    int permissionId = Convert.ToInt32(item.Value);
                    
                    try
                    {                        
                        bool assigned = authManager.AssignPermissionToRole(roleId, permissionId, assignedBy);
                        if (assigned) successCount++;
                        else failCount++;
                    }
                    catch
                    {
                        failCount++;
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='success-box'>");
            sb.Append("<h4>✅ تخصیص دسترسی‌ها انجام شد</h4>");
            sb.Append("<p>موفق: " + successCount + "</p>");
            sb.Append("<p>ناموفق: " + failCount + "</p>");
            sb.Append("</div>");
            
            ShowResult(pnlPermissionResult, litPermissionResult, sb.ToString());
            
            // پاک کردن Cache
            
            authManager.ClearAllCache();
        }
        catch (Exception ex)
        {
            ShowError("خطا: " + ex.Message);
        }
    }

    protected void btnRemovePermissions_Click(object sender, EventArgs e)
    {
        int roleId = Convert.ToInt32(ddlRolesForPermission.SelectedValue);
        
        if (roleId == 0)
        {
            ShowWarning("لطفاً یک نقش انتخاب کنید");
            return;
        }

        int successCount = 0;

        try
        {
            var authManager = new AuthorizationManager();
            foreach (ListItem item in cblPermissions.Items)
            {
                if (item.Selected && !string.IsNullOrEmpty(item.Value))
                {
                    int permissionId = Convert.ToInt32(item.Value);
                    
                    bool removed = authManager.RemovePermissionFromRole(roleId, permissionId);
                    if (removed) successCount++;
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='success-box'>");
            sb.Append("<h4>✅ حذف دسترسی‌ها انجام شد</h4>");
            sb.Append("<p>تعداد: " + successCount + "</p>");
            sb.Append("</div>");
            
            ShowResult(pnlPermissionResult, litPermissionResult, sb.ToString());
            
            // پاک کردن Cache
            authManager.ClearAllCache();
        }
        catch (Exception ex)
        {
            ShowError("خطا: " + ex.Message);
        }
    }

    protected void btnShowRolePermissions_Click(object sender, EventArgs e)
    {
        int roleId = Convert.ToInt32(ddlRolesForPermission.SelectedValue);
        
        if (roleId == 0)
        {
            ShowWarning("لطفاً یک نقش انتخاب کنید");
            return;
        }

        try
        {
            var authManager = new AuthorizationManager();
            List<Permission> permissions = authManager.GetRolePermissions(roleId);
            
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='info-box'>");
            sb.Append("<h4>🔐 دسترسی‌های نقش (" + permissions.Count + ")</h4>");
            
            if (permissions.Count > 0)
            {
                string currentCategory = "";
                foreach (Permission perm in permissions)
                {
                    if (currentCategory != perm.Category)
                    {
                        if (currentCategory != "") sb.Append("<br />");
                        currentCategory = perm.Category;
                        sb.Append("<strong>" + currentCategory + ":</strong><br />");
                    }
                    
                    sb.Append("<span class='badge badge-info'>" + perm.PermissionName + "</span>");
                }
            }
            else
            {
                sb.Append("<p>❌ هیچ دسترسی وجود ندارد</p>");
            }
            
            sb.Append("</div>");
            
            ShowResult(pnlPermissionResult, litPermissionResult, sb.ToString());
        }
        catch (Exception ex)
        {
            ShowError("خطا: " + ex.Message);
        }
    }

    // ====================================================================================================
    // مدیریت Cache
    // ====================================================================================================

    protected void btnTestCache_Click(object sender, EventArgs e)
    {
        if (!EnsureUserLoggedIn()) return;

        int userId = SecurityHelper.GetCurrentUserId();
        
        try
        {            
            var authManager = new AuthorizationManager();
            // پاک کردن Cache اول
            authManager.ClearUserPermissionsCache(userId);
            
            // تست 1: از DB
            DateTime start1 = DateTime.Now;
            List<string> permissions1 = authManager.GetUserPermissions(userId);
            TimeSpan time1 = DateTime.Now - start1;
            
            // تست 2: از Cache
            DateTime start2 = DateTime.Now;
            List<string> permissions2 = authManager.GetUserPermissions(userId);
            TimeSpan time2 = DateTime.Now - start2;
            
            double improvement = ((time1.TotalMilliseconds - time2.TotalMilliseconds) / time1.TotalMilliseconds) * 100;
            
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='success-box'>");
            sb.Append("<h4>⚡ نتایج تست Performance</h4>");
            sb.Append("<p><strong>بار اول (از Database):</strong> " + time1.TotalMilliseconds.ToString("0.00") + " ms</p>");
            sb.Append("<p><strong>بار دوم (از Cache):</strong> " + time2.TotalMilliseconds.ToString("0.00") + " ms</p>");
            sb.Append("<p><strong>بهبود سرعت:</strong> " + improvement.ToString("0.00") + "%</p>");
            sb.Append("<p><strong>تعداد دسترسی‌ها:</strong> " + permissions1.Count + "</p>");
            sb.Append("</div>");
            
            ShowResult(pnlCacheResult, litCacheResult, sb.ToString());
        }
        catch (Exception ex)
        {
            ShowError("خطا: " + ex.Message);
        }
    }

    protected void btnClearUserCache_Click(object sender, EventArgs e)
    {
        if (!EnsureUserLoggedIn()) return;        
        var authManager = new AuthorizationManager();
        int userId = SecurityHelper.GetCurrentUserId();
        authManager.ClearUserPermissionsCache(userId);
        
        ShowResult(pnlCacheResult, litCacheResult, 
            "<div class='success-box'><h4>✅ Cache کاربر پاک شد</h4></div>");
    }

    protected void btnClearAllCache_Click(object sender, EventArgs e)
    {
        var authManager = new AuthorizationManager();
        authManager.ClearAllCache();
        
        ShowResult(pnlCacheResult, litCacheResult, 
            "<div class='success-box'><h4>✅ تمام Cache ها پاک شدند</h4></div>");
    }

    // ====================================================================================================
    // عملیات CRUD
    // ====================================================================================================

    protected void btnCreateUser_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(txtNewUsername.Text))
            {
                ShowWarning("نام کاربری الزامی است");
                return;
            }

            var user = new User
            {
                Username = txtNewUsername.Text.Trim(),
                Email = txtNewEmail.Text.Trim(),
                FullName = txtNewFullName.Text.Trim(),
                IsActive = true
            };

            string password = string.IsNullOrWhiteSpace(txtNewPassword.Text) ? "123456" : txtNewPassword.Text;
            var userManager = new UserManager();
            int newUserId = userManager.CreateUser(user, password, 1);

            if (newUserId > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<div class='success-box'>");
                sb.Append("<h4>✅ کاربر با موفقیت ایجاد شد!</h4>");
                sb.Append("<p><strong>شناسه:</strong> " + newUserId + "</p>");
                sb.Append("<p><strong>نام کاربری:</strong> " + user.Username + "</p>");
                sb.Append("<p><strong>رمز عبور:</strong> " + password + "</p>");
                sb.Append("</div>");
                
                ShowResult(pnlCrudResult, litCrudResult, sb.ToString());
                
                // بارگذاری مجدد لیست
                LoadUsers();
                
                // پاک کردن فرم
                txtNewUsername.Text = "";
                txtNewEmail.Text = "";
                txtNewFullName.Text = "";
                txtNewPassword.Text = "";
            }
            else
            {
                ShowError("خطا در ایجاد کاربر");
            }
        }
        catch (Exception ex)
        {
            ShowError("خطا: " + ex.Message);
        }
    }

    protected void btnUpdateUser_Click(object sender, EventArgs e)
    {
        if (!EnsureUserLoggedIn()) return;

        try
        {
            User user = SecurityHelper.GetCurrentUser();
            
            // بروزرسانی اطلاعات
            if (!string.IsNullOrWhiteSpace(txtNewUsername.Text))
                user.Username = txtNewUsername.Text.Trim();
            
            if (!string.IsNullOrWhiteSpace(txtNewEmail.Text))
                user.Email = txtNewEmail.Text.Trim();
            
            if (!string.IsNullOrWhiteSpace(txtNewFullName.Text))
                user.FullName = txtNewFullName.Text.Trim();
            var userManager = new UserManager();
            bool updated = userManager.UpdateUser(user, 1);

            if (updated)
            {
                // بروزرسانی Session
                SecurityHelper.SetCurrentUser(user);
                UpdateCurrentUserDisplay();
                
                ShowResult(pnlCrudResult, litCrudResult, 
                    "<div class='success-box'><h4>✅ کاربر بروزرسانی شد</h4></div>");
                
                // بارگذاری مجدد
                LoadUsers();
            }
            else
            {
                ShowError("خطا در بروزرسانی");
            }
        }
        catch (Exception ex)
        {
            ShowError("خطا: " + ex.Message);
        }
    }

    protected void btnDeleteUser_Click(object sender, EventArgs e)
    {
        if (!EnsureUserLoggedIn()) return;

        int userId = SecurityHelper.GetCurrentUserId();

        if (userId == 1)
        {
            ShowWarning("نمی‌توانید کاربر System را حذف کنید!");
            return;
        }

        try
        {
            var userManager = new UserManager();
            bool deleted = userManager.DeleteUser(userId, 1);

            if (deleted)
            {
                ShowResult(pnlCrudResult, litCrudResult,
                    "<div class='success-box'><h4>✅ کاربر حذف شد</h4></div>");

                // خروج از Session
                SecurityHelper.Logout();
                UpdateCurrentUserDisplay();

                // بارگذاری مجدد
                LoadUsers();
            }
            else
            {
                ShowError("خطا در حذف کاربر");
            }
        }
        catch (Exception ex)
        {
            ShowError("خطا: " + ex.Message);
        }
    }



}