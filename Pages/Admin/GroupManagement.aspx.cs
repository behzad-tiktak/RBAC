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
// GroupManagement.aspx.cs - مدیریت گروه‌های کاربری
// بدون namespace - فایل در پوشه Admin
// ═══════════════════════════════════════════════════════════════════

public partial class Pages_Admin_GroupManagement : BasePage
{
    private RBACManager rbacManager = new RBACManager();

    // صفحه نیاز به دسترسی Groups.Manage دارد
    protected override string[] RequiredPermissions
    {
        get { return new string[] { "Groups.Manage" }; }
        
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadGroups();
            LoadDropDowns();
        }
    }

    // ═════════════════════════════════════════════════════════════════
    // بخش 1: ایجاد/ویرایش گروه
    // ═════════════════════════════════════════════════════════════════

    protected void btnSaveGroup_Click(object sender, EventArgs e)
    {
        try
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtGroupName.Text))
            {
                ShowMessage("نام گروه را وارد کنید", "error");
                return;
            }

            int editGroupId = Convert.ToInt32(hfEditGroupId.Value);

            if (editGroupId > 0)
            {
                // ویرایش
                var group = rbacManager.GetAllGroups(true).FirstOrDefault(g => g.GroupId == editGroupId);

                if (group == null)
                {
                    ShowMessage("گروه یافت نشد", "error");
                    return;
                }

                group.GroupName = txtGroupName.Text.Trim();
                group.Description = txtDescription.Text.Trim();
                group.IsActive = chkIsActive.Checked;

                bool updated = rbacManager.UpdateGroup(group, CurrentUserId);

                if (updated)
                {
                    ShowMessage("گروه با موفقیت ویرایش شد", "success");
                    ClearForm();
                    LoadGroups();
                    LoadDropDowns();
                }
                else
                {
                    ShowMessage("خطا در ویرایش گروه", "error");
                }
            }
            else
            {
                
                // ایجاد جدید
                var group = new UserGroup
                {
                    GroupName = txtGroupName.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    IsActive = chkIsActive.Checked
                };

                int newGroupId = rbacManager.CreateGroup(group, CurrentUserId);

                if (newGroupId > 0)
                {
                    ShowMessage("گروه با موفقیت ایجاد شد", "success");
                    ClearForm();
                    LoadGroups();
                    LoadDropDowns();
                }
                else
                {
                    ShowMessage("خطا در ایجاد گروه", "error");
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
    // بخش 2: مدیریت اعضای گروه
    // ═════════════════════════════════════════════════════════════════

    protected void ddlGroupsForMembers_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadGroupMembers();
        LoadAvailableUsers();
    }

    protected void btnAddMembers_Click(object sender, EventArgs e)
    {
        try
        {
            int groupId = Convert.ToInt32(ddlGroupsForMembers.SelectedValue);

            if (groupId <= 0)
            {
                ShowMessage("گروهی انتخاب نشده است", "error");
                return;
            }

            // جمع‌آوری کاربران انتخاب شده
            var selectedUserIds = new List<int>();

            foreach (ListItem item in cblAvailableUsers.Items)
            {
                if (item.Selected)
                {
                    selectedUserIds.Add(Convert.ToInt32(item.Value));
                }
            }

            if (selectedUserIds.Count == 0)
            {
                ShowMessage("هیچ کاربری انتخاب نشده است", "error");
                return;
            }

            // افزودن به گروه
            bool success = rbacManager.AddMultipleUsersToGroup(
                selectedUserIds, 
                groupId, 
                CurrentUserId
            );

            if (success)
            {
                ShowMessage(
                    string.Format("{0} کاربر به گروه اضافه شد", selectedUserIds.Count), 
                    "success"
                );
                
                LoadGroupMembers();
                LoadAvailableUsers();
                LoadGroups(); // برای بروز شدن تعداد اعضا
            }
            else
            {
                ShowMessage("خطا در افزودن کاربران", "error");
            }
        }
        catch (Exception ex)
        {
            ShowMessage("خطا: " + ex.Message, "error");
        }
    }

    protected void rptGroupMembers_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "RemoveMember")
        {
            try
            {
                int userId = Convert.ToInt32(e.CommandArgument);
                int groupId = Convert.ToInt32(ddlGroupsForMembers.SelectedValue);

                bool removed = rbacManager.RemoveUserFromGroup(
                    userId, 
                    groupId, 
                    CurrentUserId
                );

                if (removed)
                {
                    ShowMessage("کاربر از گروه حذف شد", "success");
                    LoadGroupMembers();
                    LoadAvailableUsers();
                    LoadGroups();
                }
                else
                {
                    ShowMessage("خطا در حذف کاربر", "error");
                }
            }
            catch (Exception ex)
            {
                ShowMessage("خطا: " + ex.Message, "error");
            }
        }
    }

    // ═════════════════════════════════════════════════════════════════
    // بخش 3: تخصیص آبشاری نقش
    // ═════════════════════════════════════════════════════════════════

    protected void btnAssignRoleToGroup_Click(object sender, EventArgs e)
    {
        try
        {
            int groupId = Convert.ToInt32(ddlGroupsForRole.SelectedValue);
            int roleId = Convert.ToInt32(ddlRolesForGroup.SelectedValue);

            if (groupId <= 0)
            {
                ShowMessage("گروهی انتخاب نشده است", "error");
                return;
            }

            if (roleId <= 0)
            {
                ShowMessage("نقشی انتخاب نشده است", "error");
                return;
            }

            bool success = rbacManager.AssignRoleToGroupMembers(
                groupId, 
                roleId, 
                CurrentUserId
            );

            if (success)
            {
                ShowMessage("نقش با موفقیت به تمام اعضای گروه تخصیص یافت", "success");
            }
            else
            {
                ShowMessage("خطا در تخصیص نقش", "error");
            }
        }
        catch (Exception ex)
        {
            ShowMessage("خطا: " + ex.Message, "error");
        }
    }

    protected void btnRemoveRoleFromGroup_Click(object sender, EventArgs e)
    {
        try
        {
            int groupId = Convert.ToInt32(ddlGroupsForRole.SelectedValue);
            int roleId = Convert.ToInt32(ddlRolesForGroup.SelectedValue);

            if (groupId <= 0)
            {
                ShowMessage("گروهی انتخاب نشده است", "error");
                return;
            }

            if (roleId <= 0)
            {
                ShowMessage("نقشی انتخاب نشده است", "error");
                return;
            }

            bool success = rbacManager.RemoveRoleFromGroupMembers(
                groupId, 
                roleId, 
                CurrentUserId
            );

            if (success)
            {
                ShowMessage("نقش از تمام اعضای گروه حذف شد", "success");
            }
            else
            {
                ShowMessage("خطا در حذف نقش", "error");
            }
        }
        catch (Exception ex)
        {
            ShowMessage("خطا: " + ex.Message, "error");
        }
    }

    // ═════════════════════════════════════════════════════════════════
    // بخش 4: GridView Events
    // ═════════════════════════════════════════════════════════════════

    protected void gvGroups_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int groupId = Convert.ToInt32(e.CommandArgument);

        if (e.CommandName == "EditGroup")
        {
            LoadGroupForEdit(groupId);
        }
        else if (e.CommandName == "DeleteGroup")
        {
            DeleteGroup(groupId);
        }
    }

    private void LoadGroupForEdit(int groupId)
    {
        try
        {
            var group = rbacManager.GetAllGroups(true)
                .FirstOrDefault(g => g.GroupId == groupId);

            if (group != null)
            {
                hfEditGroupId.Value = group.GroupId.ToString();
                txtGroupName.Text = group.GroupName;
                txtDescription.Text = group.Description;
                chkIsActive.Checked = group.IsActive;

                btnSaveGroup.Text = "💾 بروزرسانی گروه";
                btnCancelEdit.Visible = true;

                // اسکرول به بالا
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

    private void DeleteGroup(int groupId)
    {
        try
        {
            bool deleted = rbacManager.DeleteGroup(groupId, CurrentUserId);

            if (deleted)
            {
                ShowMessage("گروه با موفقیت حذف شد", "success");
                LoadGroups();
                LoadDropDowns();
            }
            else
            {
                ShowMessage("خطا در حذف گروه", "error");
            }
        }
        catch (Exception ex)
        {
            ShowMessage("خطا: " + ex.Message, "error");
        }
    }

    // ═════════════════════════════════════════════════════════════════
    // Load Data Methods
    // ═════════════════════════════════════════════════════════════════

    private void LoadGroups()
    {
        try
        {
            var groups = rbacManager.GetAllGroups(true);
            gvGroups.DataSource = groups;
            gvGroups.DataBind();
        }
        catch (Exception ex)
        {
            ShowMessage("خطا در بارگذاری گروه‌ها: " + ex.Message, "error");
        }
    }

    private void LoadDropDowns()
    {
        try
        {
            var groups = rbacManager.GetAllGroups(false);

            // DropDown اعضا
            ddlGroupsForMembers.DataSource = groups;
            ddlGroupsForMembers.DataTextField = "GroupName";
            ddlGroupsForMembers.DataValueField = "GroupId";
            ddlGroupsForMembers.DataBind();
            ddlGroupsForMembers.Items.Insert(0, new ListItem("-- انتخاب گروه --", "0"));

            // DropDown نقش
            ddlGroupsForRole.DataSource = groups;
            ddlGroupsForRole.DataTextField = "GroupName";
            ddlGroupsForRole.DataValueField = "GroupId";
            ddlGroupsForRole.DataBind();
            ddlGroupsForRole.Items.Insert(0, new ListItem("-- انتخاب گروه --", "0"));

            // DropDown نقش‌ها
            var roles = rbacManager.GetAllRoles(false);
            ddlRolesForGroup.DataSource = roles;
            ddlRolesForGroup.DataTextField = "RoleName";
            ddlRolesForGroup.DataValueField = "RoleId";
            ddlRolesForGroup.DataBind();
            ddlRolesForGroup.Items.Insert(0, new ListItem("-- انتخاب نقش --", "0"));
        }
        catch (Exception ex)
        {
            ShowMessage("خطا در بارگذاری لیست‌ها: " + ex.Message, "error");
        }
    }

    private void LoadGroupMembers()
    {
        try
        {
            int groupId = Convert.ToInt32(ddlGroupsForMembers.SelectedValue);

            if (groupId > 0)
            {
                var members = rbacManager.GetGroupMembers(groupId);
                
                if (members.Count > 0)
                {
                    rptGroupMembers.DataSource = members;
                    rptGroupMembers.DataBind();
                    rptGroupMembers.Visible = true;
                    pnlNoMembers.Visible = false;
                }
                else
                {
                    rptGroupMembers.Visible = false;
                    pnlNoMembers.Visible = true;
                }
            }
            else
            {
                rptGroupMembers.Visible = false;
                pnlNoMembers.Visible = false;
            }
        }
        catch (Exception ex)
        {
            ShowMessage("خطا در بارگذاری اعضا: " + ex.Message, "error");
        }
    }

    private void LoadAvailableUsers()
    {
        try
        {
            int groupId = Convert.ToInt32(ddlGroupsForMembers.SelectedValue);

            if (groupId > 0)
            {
                // دریافت تمام کاربران
                var allUsers = rbacManager.GetAllUsers(false);

                // دریافت اعضای فعلی گروه
                var currentMembers = rbacManager.GetGroupMembers(groupId);
                var memberIds = currentMembers.Select(m => m.UserId).ToList();

                // فیلتر کاربران (فقط کسانی که عضو نیستند)
                var availableUsers = allUsers
                    .Where(u => !memberIds.Contains(u.UserId))
                    .ToList();

                cblAvailableUsers.DataSource = availableUsers;
                cblAvailableUsers.DataTextField = "FullName";
                cblAvailableUsers.DataValueField = "UserId";
                cblAvailableUsers.DataBind();
            }
            else
            {
                cblAvailableUsers.Items.Clear();
            }
        }
        catch (Exception ex)
        {
            ShowMessage("خطا در بارگذاری کاربران: " + ex.Message, "error");
        }
    }

    // ═════════════════════════════════════════════════════════════════
    // Helper Methods
    // ═════════════════════════════════════════════════════════════════

    private void ClearForm()
    {
        hfEditGroupId.Value = "0";
        txtGroupName.Text = string.Empty;
        txtDescription.Text = string.Empty;
        chkIsActive.Checked = true;
        btnSaveGroup.Text = "💾 ذخیره گروه";
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
    protected void btnSaveGroupNew_Click(object sender, EventArgs e)
    {

    }
}