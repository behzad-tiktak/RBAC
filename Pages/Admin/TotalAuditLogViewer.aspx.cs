using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using RBACSystem.Base;
using RBACSystem.DataAccess.RBAC;
using RBACSystem.Models.RBAC;

public partial class Pages_Admin_TotalAuditLogViewer : BasePage
{

    private readonly AuditRepository _auditRepo;

    public Pages_Admin_TotalAuditLogViewer()
    {
        _auditRepo = new AuditRepository();
    }

    protected override string[] RequiredPermissions
    {
        get { return new string[] { "AuditLog.View" }; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadAuditLogs();
        }
    }

    private void LoadAuditLogs()
    {
        try
        {
            List<AuditLog> logs = _auditRepo.GetRecentLogs(100);
            gvAuditLogs.DataSource = logs;
            gvAuditLogs.DataBind();
        }
        catch (Exception ex)
        {
            HandleException(ex, "LoadAuditLogs");
        }
    }

    protected void btnDetails_Click(object sender, EventArgs e)
    {
        try
        {
            LinkButton btn = (LinkButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;

            // دریافت LogId از DataKey
            long logId = Convert.ToInt64(gvAuditLogs.DataKeys[row.RowIndex].Value);

            // دریافت جزئیات کامل
            AuditLog log = GetLogDetails(logId);

            if (log != null)
            {
                // پر کردن اطلاعات Modal
                lblModalLogId.Text = log.LogId.ToString();
                lblModalUsername.Text = log.Username ?? "سیستم";

                // رنگ‌بندی عملیات
                string actionText = GetActionFriendlyName(log.Action);
                string actionCss = GetActionCssClass(log.Action);
                lblModalAction.Text = actionText;
                lblModalAction.CssClass = actionCss;

                lblModalTable.Text = log.TableName;
                lblModalRecordId.Text = log.RecordId.HasValue ? log.RecordId.Value.ToString() : "-";
                lblModalDate.Text = log.ActionDate.ToString("yyyy/MM/dd HH:mm:ss");
                lblModalIp.Text = log.IpAddress ?? "-";
                lblModalUserAgent.Text = !string.IsNullOrEmpty(log.UserAgent) ? log.UserAgent : "-";

                // نمایش مقادیر قبلی و جدید
                lblOldValue.Text = FormatValue(log.OldValue);
                lblNewValue.Text = FormatValue(log.NewValue);

                // تنظیم HiddenField برای نمایش Modal
                hfShowModal.Value = "1";
            }
            else
            {
                ShowErrorMessage("اطلاعات لاگ یافت نشد");
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "btnDetails_Click");
        }
    }

    private AuditLog GetLogDetails(long logId)
    {
        string query = @"
                SELECT al.*, u.Username, u.FullName
                FROM AuditLog al
                LEFT JOIN Users u ON al.UserId = u.UserId
                WHERE al.LogId = @LogId";

        var parameters = new Dictionary<string, object>
            {
                { "@LogId", logId }
            };

        using (var select = new JCGSQLSelect(query, parameters, false, true))
        {
            if (select.Status())
            {
                var reader = select.GetDataReader();

                var log = new AuditLog
                {
                    LogId = Convert.ToInt64(reader["LogId"]),
                    UserId = reader["UserId"] != DBNull.Value ? Convert.ToInt32(reader["UserId"]) : (int?)null,
                    Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : "سیستم",
                    Action = reader["Action"].ToString(),
                    TableName = reader["TableName"].ToString(),
                    RecordId = reader["RecordId"] != DBNull.Value ? Convert.ToInt32(reader["RecordId"]) : (int?)null,
                    OldValue = reader["OldValue"] != DBNull.Value ? reader["OldValue"].ToString() : null,
                    NewValue = reader["NewValue"] != DBNull.Value ? reader["NewValue"].ToString() : null,
                    ActionDate = Convert.ToDateTime(reader["ActionDate"]),
                    IpAddress = reader["IpAddress"] != DBNull.Value ? reader["IpAddress"].ToString() : null,
                    UserAgent = reader["UserAgent"] != DBNull.Value ? reader["UserAgent"].ToString() : null
                };

                return log;
            }
        }

        return null;
    }

    private string GetActionFriendlyName(string action)
    {
        switch (action)
        {
            case "Insert": return "افزودن";
            case "Update": return "ویرایش";
            case "Delete": return "حذف";
            case "Login": return "ورود";
            case "Logout": return "خروج";
            default: return action;
        }
    }

    private string GetActionCssClass(string action)
    {
        switch (action)
        {
            case "Insert": return "label label-success badge-action";
            case "Update": return "label label-info badge-action";
            case "Delete": return "label label-danger badge-action";
            case "Login": return "label label-warning badge-action";
            case "Logout": return "label label-default badge-action";
            default: return "label label-primary badge-action";
        }
    }

    private string FormatValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "بدون تغییر";

        // اگر مقدار JSON است، سعی می‌کنیم فرمت بندی کنیم
        try
        {
            // بررسی اینکه آیا یک JSON معتبر است
            if ((value.StartsWith("{") && value.EndsWith("}")) ||
                (value.StartsWith("[") && value.EndsWith("]")))
            {
                // فرمت بندی دستی JSON
                return PrettyPrintJson(value);
            }
        }
        catch
        {
            // اگر خطا داد، همان رشته اصلی را برمی‌گردانیم
        }

        return Server.HtmlEncode(value);
    }

    private string PrettyPrintJson(string json)
    {
        try
        {
            int indent = 0;
            bool inString = false;
            string result = "";

            foreach (char ch in json)
            {
                if (ch == '"' && (result.Length == 0 || result[result.Length - 1] != '\\'))
                {
                    inString = !inString;
                }

                if (!inString)
                {
                    if (ch == '{' || ch == '[')
                    {
                        result += ch + "\n" + new string(' ', ++indent * 2);
                    }
                    else if (ch == '}' || ch == ']')
                    {
                        result += "\n" + new string(' ', --indent * 2) + ch;
                    }
                    else if (ch == ',')
                    {
                        result += ch + "\n" + new string(' ', indent * 2);
                    }
                    else if (ch == ':')
                    {
                        result += ch + " ";
                    }
                    else
                    {
                        result += ch;
                    }
                }
                else
                {
                    result += ch;
                }
            }

            return Server.HtmlEncode(result);
        }
        catch
        {
            return Server.HtmlEncode(json);
        }
    }

    protected void gvAuditLogs_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            AuditLog log = (AuditLog)e.Row.DataItem;

            // رنگ‌بندی عملیات در ستون Action
            TableCell actionCell = e.Row.Cells[2]; // ستون Action

            switch (log.Action)
            {
                case "Insert":
                    actionCell.CssClass = "action-insert";
                    actionCell.Text = "<span class='glyphicon glyphicon-plus-sign'></span> افزودن";
                    break;
                case "Update":
                    actionCell.CssClass = "action-update";
                    actionCell.Text = "<span class='glyphicon glyphicon-edit'></span> ویرایش";
                    break;
                case "Delete":
                    actionCell.CssClass = "action-delete";
                    actionCell.Text = "<span class='glyphicon glyphicon-trash'></span> حذف";
                    break;
                case "Login":
                    actionCell.CssClass = "action-login";
                    actionCell.Text = "<span class='glyphicon glyphicon-log-in'></span> ورود";
                    break;
                case "Logout":
                    actionCell.Text = "<span class='glyphicon glyphicon-log-out'></span> خروج";
                    break;
            }
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            LoadFilteredLogs();
        }
        catch (Exception ex)
        {
            HandleException(ex, "btnSearch_Click");
        }
    }

    private void LoadFilteredLogs()
    {
        string username = txtSearchUsername.Text.Trim();
        string action = ddlAction.SelectedValue;
        DateTime? fromDate = string.IsNullOrEmpty(txtFromDate.Text) ? (DateTime?)null : DateTime.Parse(txtFromDate.Text);
        DateTime? toDate = string.IsNullOrEmpty(txtToDate.Text) ? (DateTime?)null : DateTime.Parse(txtToDate.Text);

        string query = @"
                SELECT TOP 100 al.LogId, al.UserId, al.Action, al.TableName, 
                       al.RecordId, al.ActionDate, al.IpAddress, u.Username
                FROM AuditLog al
                LEFT JOIN Users u ON al.UserId = u.UserId
                WHERE 1=1";

        var parameters = new Dictionary<string, object>();

        if (!string.IsNullOrEmpty(username))
        {
            query += " AND u.Username LIKE @Username";
            parameters.Add("@Username", "%" + username + "%");
        }

        if (!string.IsNullOrEmpty(action) && action != "All")
        {
            query += " AND al.Action = @Action";
            parameters.Add("@Action", action);
        }

        if (fromDate.HasValue)
        {
            query += " AND al.ActionDate >= @FromDate";
            parameters.Add("@FromDate", fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query += " AND al.ActionDate <= @ToDate";
            parameters.Add("@ToDate", toDate.Value.AddDays(1));
        }

        query += " ORDER BY al.ActionDate DESC";

        using (var select = new JCGSQLSelect(query, parameters, true, false))
        {
            if (select.Status())
            {
                var dt = select.GetDataTable();
                var logs = new List<AuditLog>();

                foreach (System.Data.DataRow row in dt.Rows)
                {
                    logs.Add(new AuditLog
                    {
                        LogId = Convert.ToInt64(row["LogId"]),
                        UserId = row["UserId"] != DBNull.Value ? Convert.ToInt32(row["UserId"]) : (int?)null,
                        Username = row["Username"] != DBNull.Value ? row["Username"].ToString() : "سیستم",
                        Action = row["Action"].ToString(),
                        TableName = row["TableName"].ToString(),
                        RecordId = row["RecordId"] != DBNull.Value ? Convert.ToInt32(row["RecordId"]) : (int?)null,
                        ActionDate = Convert.ToDateTime(row["ActionDate"]),
                        IpAddress = row["IpAddress"] != DBNull.Value ? row["IpAddress"].ToString() : null
                    });
                }

                gvAuditLogs.DataSource = logs;
                gvAuditLogs.DataBind();
            }
            else
            {
                gvAuditLogs.DataSource = null;
                gvAuditLogs.DataBind();
            }
        }
    }

    protected void btnClearSearch_Click(object sender, EventArgs e)
    {
        txtSearchUsername.Text = string.Empty;
        ddlAction.SelectedIndex = 0;
        txtFromDate.Text = string.Empty;
        txtToDate.Text = string.Empty;
        LoadAuditLogs();
    }
}

