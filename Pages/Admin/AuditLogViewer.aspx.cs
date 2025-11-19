using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using RBACSystem.BusinessLogic.RBAC;
using RBACSystem.DataAccess.RBAC;
using RBACSystem.Models.RBAC;
using RBACSystem.Base;

public partial class Pages_Admin_AuditLogViewer : BasePage
{
    private RBACManager rbacManager = new RBACManager();
    private AuditRepository auditRepo = new AuditRepository();

    protected override string[] RequiredPermissions
    {
        get { return new string[] { "AuditLog.View" }; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadFilters();
            LoadLogs();
            LoadStats();
        }
    }

    // ═════════════════════════════════════════════════════════════════
    // Filter
    // ═════════════════════════════════════════════════════════════════

    protected void btnFilter_Click(object sender, EventArgs e)
    {
        LoadLogs();
    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        ddlUsers.SelectedIndex = 0;
        ddlAction.SelectedIndex = 0;
        ddlTable.SelectedIndex = 0;
        ddlCount.SelectedValue = "100";

        LoadLogs();
    }

    // ═════════════════════════════════════════════════════════════════
    // Load Data
    // ═════════════════════════════════════════════════════════════════

    private void LoadFilters()
    {
        try
        {
            // بارگذاری کاربران
            var users = rbacManager.GetAllUsers(true);
            ddlUsers.DataSource = users;
            ddlUsers.DataTextField = "FullName";
            ddlUsers.DataValueField = "UserId";
            ddlUsers.DataBind();
            ddlUsers.Items.Insert(0, new ListItem("همه کاربران", "0"));

            // بارگذاری جداول
            var tables = GetUniqueTables();
            ddlTable.DataSource = tables;
            ddlTable.DataBind();
            ddlTable.Items.Insert(0, new ListItem("همه جداول", ""));
        }
        catch { }
    }

    private void LoadLogs()
    {
        try
        {
            int count = Convert.ToInt32(ddlCount.SelectedValue);
            var logs = auditRepo.GetRecentLogs(count);

            // اعمال فیلترها
            int selectedUserId = Convert.ToInt32(ddlUsers.SelectedValue);
            if (selectedUserId > 0)
            {
                logs = logs.Where(l => l.UserId == selectedUserId).ToList();
            }

            string selectedAction = ddlAction.SelectedValue;
            if (!string.IsNullOrEmpty(selectedAction))
            {
                logs = logs.Where(l => l.Action == selectedAction).ToList();
            }

            string selectedTable = ddlTable.SelectedValue;
            if (!string.IsNullOrEmpty(selectedTable))
            {
                logs = logs.Where(l => l.TableName == selectedTable).ToList();
            }

            gvLogs.DataSource = logs;
            gvLogs.DataBind();

            // رنگ‌آمیزی بر اساس نوع عملیات
            ColorizeRows();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Error loading logs: " + ex.Message);
        }
    }

    private void LoadStats()
    {
        try
        {
            var allLogs = auditRepo.GetRecentLogs(10000);

            // تعداد کل
            lblTotalLogs.Text = allLogs.Count.ToString();

            // کاربران یکتا
            var uniqueUsers = allLogs
                .Where(l => l.UserId.HasValue)
                .Select(l => l.UserId.Value)
                .Distinct()
                .Count();
            lblActiveUsers.Text = uniqueUsers.ToString();

            // امروز
            var today = DateTime.Today;
            var todayLogs = allLogs
                .Where(l => l.ActionDate.Date == today)
                .Count();
            lblTodayLogs.Text = todayLogs.ToString();
        }
        catch { }
    }

    // ═════════════════════════════════════════════════════════════════
    // Helper Methods
    // ═════════════════════════════════════════════════════════════════

    private List<string> GetUniqueTables()
    {
        try
        {
            var logs = auditRepo.GetRecentLogs(1000);
            return logs
                .Where(l => !string.IsNullOrEmpty(l.TableName))
                .Select(l => l.TableName)
                .Distinct()
                .OrderBy(t => t)
                .ToList();
        }
        catch
        {
            return new List<string>();
        }
    }

    private void ColorizeRows()
    {
        foreach (GridViewRow row in gvLogs.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                string action = row.Cells[2].Text; // ستون Action

                if (action == "Insert")
                    row.CssClass = "action-insert";
                else if (action == "Update")
                    row.CssClass = "action-update";
                else if (action == "Delete")
                    row.CssClass = "action-delete";
                else if (action == "Login" || action == "Logout")
                    row.CssClass = "action-login";
            }
        }
    }
}