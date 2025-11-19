using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using RBACSystem.BusinessLogic.RBAC;
using RBACSystem.Models.RBAC;
using RBACSystem.Base;

public partial class Pages_Admin_PermissionManagement : BasePage
{
    private RBACManager rbacManager = new RBACManager();

    protected override string[] RequiredPermissions
    {
        get { return new string[] { "Permissions.Manage" }; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadFilterCategories();
            LoadPermissions();
        }
    }

    // ═════════════════════════════════════════════════════════════════
    // ذخیره دسترسی
    // ═════════════════════════════════════════════════════════════════

    protected void btnSavePermission_Click(object sender, EventArgs e)
    {
        try
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtPermissionName.Text))
            {
                ShowMessage("نام دسترسی را وارد کنید", "error");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDisplayName.Text))
            {
                ShowMessage("نام نمایشی را وارد کنید", "error");
                return;
            }

            // بررسی فرمت نام (Category.Action)
            string permissionName = txtPermissionName.Text.Trim();
            if (!permissionName.Contains("."))
            {
                ShowMessage("فرمت نام دسترسی باید به صورت Category.Action باشد (مثلاً: Users.View)", "error");
                return;
            }

            int editPermissionId = Convert.ToInt32(hfEditPermissionId.Value);

            if (editPermissionId > 0)
            {
                // ویرایش
                var permission = rbacManager.GetAllPermissions(true)
                    .FirstOrDefault(p => p.PermissionId == editPermissionId);

                if (permission == null)
                {
                    ShowMessage("دسترسی یافت نشد", "error");
                    return;
                }

                permission.PermissionName = permissionName;
                permission.DisplayName = txtDisplayName.Text.Trim();
                permission.Description = txtDescription.Text.Trim();
                permission.Category = GetSelectedCategory();
                permission.ResourceType = ddlResourceType.SelectedValue;
                permission.ResourcePath = txtResourcePath.Text.Trim();
                permission.IsActive = chkIsActive.Checked;

                var permManager = new PermissionManager();
                bool updated = permManager.UpdatePermission(permission, CurrentUserId);

                if (updated)
                {
                    ShowMessage("دسترسی با موفقیت ویرایش شد", "success");
                    ClearForm();
                    LoadPermissions();
                }
                else
                {
                    ShowMessage("خطا در ویرایش دسترسی", "error");
                }
            }
            else
            {
                // ایجاد جدید
                var permission = new Permission
                {
                    PermissionName = permissionName,
                    DisplayName = txtDisplayName.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    Category = GetSelectedCategory(),
                    ResourceType = ddlResourceType.SelectedValue,
                    ResourcePath = txtResourcePath.Text.Trim(),
                    IsActive = chkIsActive.Checked
                };

                int newPermissionId = rbacManager.CreatePermission(permission, CurrentUserId);

                if (newPermissionId > 0)
                {
                    ShowMessage("دسترسی با موفقیت ایجاد شد", "success");
                    ClearForm();
                    LoadPermissions();
                    LoadFilterCategories();
                }
                else
                {
                    ShowMessage("خطا در ایجاد دسترسی", "error");
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
    // فیلتر
    // ═════════════════════════════════════════════════════════════════

    protected void ddlFilterCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadPermissions();
    }

    // ═════════════════════════════════════════════════════════════════
    // GridView Events
    // ═════════════════════════════════════════════════════════════════

    protected void gvPermissions_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int permissionId = Convert.ToInt32(e.CommandArgument);

        if (e.CommandName == "EditPermission")
        {
            LoadPermissionForEdit(permissionId);
        }
        else if (e.CommandName == "DeletePermission")
        {
            DeletePermission(permissionId);
        }
    }

    private void LoadPermissionForEdit(int permissionId)
    {
        try
        {
            var permission = rbacManager.GetAllPermissions(true)
                .FirstOrDefault(p => p.PermissionId == permissionId);

            if (permission != null)
            {
                hfEditPermissionId.Value = permission.PermissionId.ToString();
                txtPermissionName.Text = permission.PermissionName;
                txtDisplayName.Text = permission.DisplayName;
                txtDescription.Text = permission.Description;
                txtResourcePath.Text = permission.ResourcePath;
                chkIsActive.Checked = permission.IsActive;

                // Category
                if (!string.IsNullOrEmpty(permission.Category))
                {
                    if (ddlCategory.Items.FindByValue(permission.Category) != null)
                    {
                        ddlCategory.SelectedValue = permission.Category;
                    }
                    else
                    {
                        ddlCategory.Items.Insert(1, new ListItem(permission.Category, permission.Category));
                        ddlCategory.SelectedValue = permission.Category;
                    }
                }

                // ResourceType
                if (!string.IsNullOrEmpty(permission.ResourceType))
                {
                    if (ddlResourceType.Items.FindByValue(permission.ResourceType) != null)
                    {
                        ddlResourceType.SelectedValue = permission.ResourceType;
                    }
                }

                btnSavePermission.Text = "💾 بروزرسانی دسترسی";
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

    private void DeletePermission(int permissionId)
    {
        try
        {
            var permManager = new PermissionManager();
            bool deleted = permManager.DeletePermission(permissionId, CurrentUserId);

            if (deleted)
            {
                ShowMessage("دسترسی با موفقیت حذف شد", "success");
                LoadPermissions();
                LoadFilterCategories();
            }
            else
            {
                ShowMessage("خطا در حذف دسترسی", "error");
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

    private void LoadPermissions()
    {
        try
        {
            // فیلتر وضعیت
            bool includeInactive = (ddlFilterStatus.SelectedValue == "all");
            var permissions = rbacManager.GetAllPermissions(includeInactive);

            // فیلتر دسته
            string selectedCategory = ddlFilterCategory.SelectedValue;
            if (!string.IsNullOrEmpty(selectedCategory))
            {
                permissions = permissions
                    .Where(p => p.Category == selectedCategory)
                    .ToList();
            }

            // فیلتر فعال/غیرفعال
            if (ddlFilterStatus.SelectedValue == "active")
            {
                permissions = permissions.Where(p => p.IsActive).ToList();
            }
            else if (ddlFilterStatus.SelectedValue == "inactive")
            {
                permissions = permissions.Where(p => !p.IsActive).ToList();
            }

            // مرتب‌سازی
            permissions = permissions
                .OrderBy(p => p.Category)
                .ThenBy(p => p.PermissionName)
                .ToList();

            gvPermissions.DataSource = permissions;
            gvPermissions.DataBind();
        }
        catch (Exception ex)
        {
            ShowMessage("خطا در بارگذاری دسترسی‌ها: " + ex.Message, "error");
        }
    }

    private void LoadFilterCategories()
    {
        try
        {
            var categories = rbacManager.GetPermissionCategories();

            ddlFilterCategory.DataSource = categories;
            ddlFilterCategory.DataBind();
            ddlFilterCategory.Items.Insert(0, new ListItem("همه دسته‌ها", ""));
        }
        catch { }
    }

    // ═════════════════════════════════════════════════════════════════
    // Helper Methods
    // ═════════════════════════════════════════════════════════════════

    private string GetSelectedCategory()
    {
        string category = ddlCategory.SelectedValue;

        // اگر کاربر Category دستی وارد کرده
        if (string.IsNullOrEmpty(category))
        {
            // از PermissionName استخراج کن
            string permissionName = txtPermissionName.Text.Trim();
            if (permissionName.Contains("."))
            {
                category = permissionName.Split('.')[0];
            }
        }

        return category;
    }

    private void ClearForm()
    {
        hfEditPermissionId.Value = "0";
        txtPermissionName.Text = string.Empty;
        txtDisplayName.Text = string.Empty;
        txtDescription.Text = string.Empty;
        txtResourcePath.Text = string.Empty;
        ddlCategory.SelectedIndex = 0;
        ddlResourceType.SelectedIndex = 0;
        chkIsActive.Checked = true;
        btnSavePermission.Text = "💾 ذخیره دسترسی";
        btnCancelEdit.Visible = false;
    }

    private void ShowMessage(string message, string type)
    {
        pnlMessage.Visible = true;
        lblMessage.Text = message;
        lblMessage.CssClass = type == "success" ? "alert alert-success" : "alert alert-error";
    }
}