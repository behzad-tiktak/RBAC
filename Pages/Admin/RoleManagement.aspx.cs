using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using RBACSystem.BusinessLogic.RBAC;
using RBACSystem.Models.RBAC;
using RBACSystem.Utilities;
using RBACSystem.Base;

// ═══════════════════════════════════════════════════════════════════
// RoleManagement.aspx.cs - مدیریت نقش‌ها
// بدون namespace - فایل در پوشه Admin
// ═══════════════════════════════════════════════════════════════════

public partial class Pages_Admin_RoleManagement : BasePage
{
    private RBACManager rbacManager = new RBACManager();

    // صفحه نیاز به دسترسی Roles.Manage دارد
    protected override string[] RequiredPermissions
    {
        get { return new string[] { "Roles.Manage" }; }
        
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // بررسی دسترسی
            if (!SecurityHelper.IsAuthenticated())
            {
                Response.Redirect("~/Pages/Admin/Login.aspx");
                return;
            }

            int currentUserId = SecurityHelper.GetCurrentUserId();

            // بررسی دسترسی Admin
            if (!rbacManager.UserHasPermission(currentUserId, "Roles.Manage"))            
            {
                ShowMessage("شما دسترسی به این صفحه را ندارید!", "error");
                return;
            }

            // بارگذاری اولیه
            LoadRoles();
            LoadPermissions();
        }
    }

    #region ذخیره نقش

    protected void btnSaveRole_Click(object sender, EventArgs e)
    {
        try
        {
            int currentUserId = SecurityHelper.GetCurrentUserId();

            // Validation
            if (string.IsNullOrWhiteSpace(txtRoleName.Text))
            {
                ShowMessage("نام نقش را وارد کنید", "error");
                return;
            }

            int priority;
            if (!int.TryParse(txtPriority.Text, out priority))
            {
                ShowMessage("اولویت باید عدد باشد", "error");
                return;
            }

            int editRoleId = Convert.ToInt32(hfEditRoleId.Value);

            if (editRoleId > 0)
            {
                // ویرایش
                Role role = rbacManager.GetAllRoles(true)
                    .FirstOrDefault(r => r.RoleId == editRoleId);

                if (role == null)
                {
                    ShowMessage("نقش یافت نشد", "error");
                    return;
                }

                role.RoleName = txtRoleName.Text.Trim();
                role.Description = txtDescription.Text.Trim();
                role.Priority = priority;
                role.IsActive = chkIsActive.Checked;

                bool updated = rbacManager.UpdateRole(role, currentUserId);

                if (updated)
                {
                    ShowMessage("نقش با موفقیت ویرایش شد", "success");
                    ClearForm();
                    LoadRoles();
                }
                else
                {
                    ShowMessage("خطا در ویرایش نقش", "error");
                }
            }
            else
            {
                // ایجاد جدید
                var role = new Role
                {
                    RoleName = txtRoleName.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    Priority = priority,
                    IsActive = chkIsActive.Checked
                };

                int newRoleId = rbacManager.CreateRole(role, currentUserId);

                if (newRoleId > 0)
                {
                    ShowMessage("نقش با موفقیت ایجاد شد", "success");
                    ClearForm();
                    LoadRoles();
                }
                else
                {
                    ShowMessage("خطا در ایجاد نقش", "error");
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

    #endregion

    #region تخصیص دسترسی‌ها

    protected void ddlRoles_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadPermissions();
    }

    protected void btnSavePermissions_Click(object sender, EventArgs e)
    {
        try
        {
            int currentUserId = SecurityHelper.GetCurrentUserId();
            int selectedRoleId = Convert.ToInt32(ddlRoles.SelectedValue);

            if (selectedRoleId <= 0)
            {
                ShowMessage("نقشی انتخاب نشده است", "error");
                return;
            }

            // جمع‌آوری دسترسی‌های انتخاب شده
            var selectedPermissionIds = new List<int>();

            foreach (RepeaterItem item in rptPermissionsByCategory.Items)
            {
                CheckBoxList cbl = (CheckBoxList)item.FindControl("cblPermissions");

                if (cbl != null)
                {
                    foreach (ListItem listItem in cbl.Items)
                    {
                        if (listItem.Selected)
                        {
                            selectedPermissionIds.Add(Convert.ToInt32(listItem.Value));
                        }
                    }
                }
            }

            if (selectedPermissionIds.Count == 0)
            {
                ShowMessage("هیچ دسترسی انتخاب نشده است", "error");
                return;
            }

            // ذخیره دسترسی‌ها
            bool success = rbacManager.AssignPermissionsToRole(
                selectedRoleId,
                selectedPermissionIds,
                currentUserId
            );

            if (success)
            {
                ShowMessage(
                    string.Format("{0} دسترسی با موفقیت تخصیص یافت", selectedPermissionIds.Count),
                    "success"
                );
            }
            else
            {
                ShowMessage("خطا در تخصیص دسترسی‌ها", "error");
            }
        }
        catch (Exception ex)
        {
            ShowMessage("خطا: " + ex.Message, "error");
        }
    }

    #endregion

    #region GridView Events

    protected void gvRoles_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "EditRole")
        {
            int roleId = Convert.ToInt32(e.CommandArgument);
            LoadRoleForEdit(roleId);
        }
    }

    private void LoadRoleForEdit(int roleId)
    {
        try
        {
            Role role = rbacManager.GetAllRoles(true)
                .FirstOrDefault(r => r.RoleId == roleId);

            if (role != null)
            {
                hfEditRoleId.Value = role.RoleId.ToString();
                txtRoleName.Text = role.RoleName;
                txtDescription.Text = role.Description;
                txtPriority.Text = role.Priority.ToString();
                chkIsActive.Checked = role.IsActive;

                btnSaveRole.Text = "💾 بروزرسانی نقش";
                btnCancelEdit.Visible = true;

                // اسکرول به بالا
                ScriptManager.RegisterStartupScript(this, GetType(), "ScrollToTop",
                    "window.scrollTo(0,0);", true);
            }
        }
        catch (Exception ex)
        {
            ShowMessage("خطا: " + ex.Message, "error");
        }
    }

    #endregion

    #region Load Data

    private void LoadRoles()
    {
        try
        {
            // بارگذاری GridView
            List<Role> roles = rbacManager.GetAllRoles(true);
            gvRoles.DataSource = roles;
            gvRoles.DataBind();

            // بارگذاری DropDownList
            ddlRoles.DataSource = roles.Where(r => r.IsActive).ToList();
            ddlRoles.DataTextField = "RoleName";
            ddlRoles.DataValueField = "RoleId";
            ddlRoles.DataBind();
            ddlRoles.Items.Insert(0, new ListItem("-- انتخاب نقش --", "0"));
        }
        catch (Exception ex)
        {
            ShowMessage("خطا در بارگذاری نقش‌ها: " + ex.Message, "error");
        }
    }

    private void LoadPermissions()
    {
        try
        {
            int selectedRoleId = Convert.ToInt32(ddlRoles.SelectedValue);

            // دریافت دسترسی‌ها به صورت دسته‌بندی شده
            Dictionary<string, List<Permission>> groupedPermissions =
                rbacManager.GetPermissionsGroupedByCategory(false);

            // دریافت دسترسی‌های فعلی نقش
            List<Permission> rolePermissions = new List<Permission>();

            if (selectedRoleId > 0)
            {
                rolePermissions = rbacManager.GetRolePermissions(selectedRoleId);
            }

            // Bind به Repeater
            rptPermissionsByCategory.DataSource = groupedPermissions;
            rptPermissionsByCategory.DataBind();

            // انتخاب دسترسی‌های فعلی
            if (selectedRoleId > 0)
            {
                foreach (RepeaterItem item in rptPermissionsByCategory.Items)
                {
                    CheckBoxList cbl = (CheckBoxList)item.FindControl("cblPermissions");

                    if (cbl != null)
                    {
                        foreach (ListItem listItem in cbl.Items)
                        {
                            int permissionId = Convert.ToInt32(listItem.Value);

                            if (rolePermissions.Any(p => p.PermissionId == permissionId))
                            {
                                listItem.Selected = true;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage("خطا در بارگذاری دسترسی‌ها: " + ex.Message, "error");
        }
    }

    #endregion

    #region Helper Methods

    private void ClearForm()
    {
        hfEditRoleId.Value = "0";
        txtRoleName.Text = string.Empty;
        txtDescription.Text = string.Empty;
        txtPriority.Text = "0";
        chkIsActive.Checked = true;
        btnSaveRole.Text = "💾 ذخیره نقش";
        btnCancelEdit.Visible = false;
    }

    private void ShowMessage(string message, string type)
    {
        pnlMessage.Visible = true;
        lblMessage.Text = message;

        if (type == "success")
        {
            lblMessage.CssClass = "alert alert-success";
        }
        else
        {
            lblMessage.CssClass = "alert alert-error";
        }
    }

    #endregion

    // ────────────────────────────────────────────────────────────────
    // متد Update که در RoleManager موجود نیست - باید اضافه شود
    // ────────────────────────────────────────────────────────────────

    // TODO: این متد باید به RoleManager اضافه شود
    private bool UpdateRole(Role role, int modifiedBy)
    {
        // فعلاً از RoleRepository مستقیم استفاده می‌کنیم
        var roleRepo = new RBACSystem.DataAccess.RBAC.RoleRepository();
        return roleRepo.UpdateRole(role);
    }
}
