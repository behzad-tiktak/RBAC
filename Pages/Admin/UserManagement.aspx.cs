using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using RBACSystem.BusinessLogic.RBAC;
using RBACSystem.Models.RBAC;
using RBACSystem.Base;

public partial class Pages_Admin_UserManagement : BasePage
{
    private RBACManager rbacManager = new RBACManager();

    protected override string[] RequiredPermissions
    {
        get { return new string[] { "Users.Manage" }; }
        
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadUsers();
            LoadUsersDropDown();
            LoadRolesCheckBoxList();
        }
    }

    // ═════════════════════════════════════════════════════════════════
    // ایجاد/ویرایش کاربر
    // ═════════════════════════════════════════════════════════════════

    protected void btnSaveUser_Click(object sender, EventArgs e)
    {
        try
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                ShowMessage("نام کاربری را وارد کنید", "error");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                ShowMessage("نام کامل را وارد کنید", "error");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                ShowMessage("ایمیل را وارد کنید", "error");
                return;
            }

            int editUserId = Convert.ToInt32(hfEditUserId.Value);

            if (editUserId > 0)
            {
                // ویرایش
                var user = rbacManager.GetUserById(editUserId);
                if (user == null)
                {
                    ShowMessage("کاربر یافت نشد", "error");
                    return;
                }

                user.Username = txtUsername.Text.Trim();
                user.FullName = txtFullName.Text.Trim();
                user.Email = txtEmail.Text.Trim();
                user.IsActive = chkIsActive.Checked;

                bool updated = rbacManager.UpdateUser(user, CurrentUserId);

                if (updated)
                {
                    // اگر رمز عبور وارد شده باشد، آن را هم تغییر بده
                    if (!string.IsNullOrEmpty(txtPassword.Text))
                    {
                        if (txtPassword.Text != txtPasswordConfirm.Text)
                        {
                            ShowMessage("رمز عبور و تکرار آن مطابقت ندارند", "error");
                            return;
                        }

                        // تغییر رمز با متد ChangePassword نیاز به رمز قدیمی دارد
                        // پس مستقیماً Update می‌کنیم
                        var userManager = new UserManager();
                        // TODO: باید متد ResetPassword اضافه کنیم
                    }

                    ShowMessage("کاربر با موفقیت ویرایش شد", "success");
                    ClearForm();
                    LoadUsers();
                    LoadUsersDropDown();
                }
            }
            else
            {
                // ایجاد جدید
                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    ShowMessage("رمز عبور را وارد کنید", "error");
                    return;
                }

                if (txtPassword.Text != txtPasswordConfirm.Text)
                {
                    ShowMessage("رمز عبور و تکرار آن مطابقت ندارند", "error");
                    return;
                }

                var user = new User
                {
                    Username = txtUsername.Text.Trim(),
                    FullName = txtFullName.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    IsActive = chkIsActive.Checked
                };

                int newUserId = rbacManager.CreateUser(
                    user, 
                    txtPassword.Text, 
                    CurrentUserId
                );

                if (newUserId > 0)
                {
                    ShowMessage("کاربر با موفقیت ایجاد شد", "success");
                    ClearForm();
                    LoadUsers();
                    LoadUsersDropDown();
                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage("خطا: " + ex.Message, "error");
        }
    }

    protected void btnCancelEdit_Click(object sender, EventArgs e)
    {
        ClearForm();
    }

    // ═════════════════════════════════════════════════════════════════
    // تخصیص نقش‌ها
    // ═════════════════════════════════════════════════════════════════

    protected void ddlUsersForRoles_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadUserRoles();
    }

    protected void btnSaveRoles_Click(object sender, EventArgs e)
    {
        try
        {
            int userId = Convert.ToInt32(ddlUsersForRoles.SelectedValue);

            if (userId <= 0)
            {
                ShowMessage("کاربری انتخاب نشده است", "error");
                return;
            }

            // جمع‌آوری نقش‌های انتخاب شده
            var selectedRoleIds = new List<int>();
            foreach (ListItem item in cblUserRoles.Items)
            {
                if (item.Selected)
                {
                    selectedRoleIds.Add(Convert.ToInt32(item.Value));
                }
            }

            // دریافت نقش‌های فعلی
            var currentRoles = rbacManager.GetUserRoles(userId);
            var currentRoleIds = currentRoles.Select(r => r.RoleId).ToList();

            // نقش‌هایی که باید اضافه شوند
            var rolesToAdd = selectedRoleIds
                .Where(id => !currentRoleIds.Contains(id))
                .ToList();

            // نقش‌هایی که باید حذف شوند
            var rolesToRemove = currentRoleIds
                .Where(id => !selectedRoleIds.Contains(id))
                .ToList();

            int addedCount = 0;
            int removedCount = 0;

            // افزودن نقش‌های جدید
            foreach (int roleId in rolesToAdd)
            {
                if (rbacManager.AssignRoleToUser(userId, roleId, CurrentUserId))
                    addedCount++;
            }

            // حذف نقش‌های قدیمی
            foreach (int roleId in rolesToRemove)
            {
                var roleManager = new RoleManager();
                if (roleManager.RemoveRoleFromUser(userId, roleId, CurrentUserId))
                    removedCount++;
            }

            ShowMessage(
                string.Format("نقش‌ها ذخیره شد. {0} افزوده، {1} حذف شد", addedCount, removedCount), 
                "success"
            );
        }
        catch (Exception ex)
        {
            ShowMessage("خطا: " + ex.Message, "error");
        }
    }

    // ═════════════════════════════════════════════════════════════════
    // GridView Events
    // ═════════════════════════════════════════════════════════════════

    protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int userId = Convert.ToInt32(e.CommandArgument);

        if (e.CommandName == "EditUser")
        {
            LoadUserForEdit(userId);
        }
        else if (e.CommandName == "DeleteUser")
        {
            DeleteUser(userId);
        }
    }

    private void LoadUserForEdit(int userId)
    {
        try
        {
            var user = rbacManager.GetUserById(userId);
            if (user != null)
            {
                hfEditUserId.Value = user.UserId.ToString();
                txtUsername.Text = user.Username;
                txtFullName.Text = user.FullName;
                txtEmail.Text = user.Email;
                chkIsActive.Checked = user.IsActive;
                
                txtPassword.Text = string.Empty;
                txtPasswordConfirm.Text = string.Empty;

                btnSaveUser.Text = "💾 بروزرسانی کاربر";
                btnCancelEdit.Visible = true;

                ScriptManager.RegisterStartupScript(
                    this, 
                    GetType(), 
                    "ScrollToTop", 
                    "window.scrollTo(0,0);", 
                    true
                );
            }
        }
        catch (Exception ex)
        {
            ShowMessage("خطا: " + ex.Message, "error");
        }
    }

    private void DeleteUser(int userId)
    {
        try
        {
            if (userId == CurrentUserId)
            {
                ShowMessage("نمی‌توانید خودتان را حذف کنید!", "error");
                return;
            }

            bool deleted = rbacManager.DeleteUser(userId, CurrentUserId);

            if (deleted)
            {
                ShowMessage("کاربر با موفقیت حذف شد", "success");
                LoadUsers();
                LoadUsersDropDown();
            }
        }
        catch (Exception ex)
        {
            ShowMessage("خطا: " + ex.Message, "error");
        }
    }

    // ═════════════════════════════════════════════════════════════════
    // Load Data
    // ═════════════════════════════════════════════════════════════════

    private void LoadUsers()
    {
        try
        {
            var users = rbacManager.GetAllUsers(true);
            gvUsers.DataSource = users;
            gvUsers.DataBind();
        }
        catch (Exception ex)
        {
            ShowMessage("خطا در بارگذاری کاربران: " + ex.Message, "error");
        }
    }

    private void LoadUsersDropDown()
    {
        try
        {
            var users = rbacManager.GetAllUsers(false);
            ddlUsersForRoles.DataSource = users;
            ddlUsersForRoles.DataTextField = "FullName";
            ddlUsersForRoles.DataValueField = "UserId";
            ddlUsersForRoles.DataBind();
            ddlUsersForRoles.Items.Insert(0, new ListItem("-- انتخاب کاربر --", "0"));
        }
        catch { }
    }

    private void LoadRolesCheckBoxList()
    {
        try
        {
            var roles = rbacManager.GetAllRoles(false);
            cblUserRoles.DataSource = roles;
            cblUserRoles.DataTextField = "RoleName";
            cblUserRoles.DataValueField = "RoleId";
            cblUserRoles.DataBind();
        }
        catch { }
    }

    private void LoadUserRoles()
    {
        try
        {
            int userId = Convert.ToInt32(ddlUsersForRoles.SelectedValue);

            if (userId > 0)
            {
                var userRoles = rbacManager.GetUserRoles(userId);
                var userRoleIds = userRoles.Select(r => r.RoleId).ToList();

                foreach (ListItem item in cblUserRoles.Items)
                {
                    int roleId = Convert.ToInt32(item.Value);
                    item.Selected = userRoleIds.Contains(roleId);
                }
            }
            else
            {
                foreach (ListItem item in cblUserRoles.Items)
                {
                    item.Selected = false;
                }
            }
        }
        catch { }
    }

    // ═════════════════════════════════════════════════════════════════
    // Helper Methods
    // ═════════════════════════════════════════════════════════════════

    private void ClearForm()
    {
        hfEditUserId.Value = "0";
        txtUsername.Text = string.Empty;
        txtFullName.Text = string.Empty;
        txtEmail.Text = string.Empty;
        txtPassword.Text = string.Empty;
        txtPasswordConfirm.Text = string.Empty;
        chkIsActive.Checked = true;
        btnSaveUser.Text = "💾 ذخیره کاربر";
        btnCancelEdit.Visible = false;
    }

    private void ShowMessage(string message, string type)
    {
        pnlMessage.Visible = true;
        lblMessage.Text = message;
        lblMessage.CssClass = type == "success" ? "alert alert-success" : "alert alert-error";
    }
}