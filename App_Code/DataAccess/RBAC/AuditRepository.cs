using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using RBACSystem.Models.RBAC;

namespace RBACSystem.DataAccess.RBAC
{
    // ====================================================================================================
    // 7. AuditRepository - مخزن لاگ‌ها
    // ====================================================================================================

    /// <summary>
    /// مخزن عملیات مربوط به ثبت و خواندن لاگ‌های سیستم
    /// </summary>
    public class AuditRepository
    {
        /// <summary>
        /// ثبت لاگ در دیتابیس
        /// </summary>
        public bool InsertLog(AuditLog log)
        {
            string query = @"
                INSERT INTO AuditLog (UserId, Action, TableName, RecordId, 
                                     OldValue, NewValue, IpAddress, UserAgent, ActionDate)
                VALUES (@UserId, @Action, @TableName, @RecordId, 
                        @OldValue, @NewValue, @IpAddress, @UserAgent, @ActionDate)";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", log.UserId ?? (object)DBNull.Value },
                { "@Action", log.Action },
                { "@TableName", log.TableName },
                { "@RecordId", log.RecordId ?? (object)DBNull.Value },
                { "@OldValue", log.OldValue ?? (object)DBNull.Value },
                { "@NewValue", log.NewValue ?? (object)DBNull.Value },
                { "@IpAddress", log.IpAddress ?? (object)DBNull.Value },
                { "@UserAgent", log.UserAgent ?? (object)DBNull.Value },
                { "@ActionDate", log.ActionDate }
            };

            var insert = new JCGSQLInsert(query, parameters, false);
            return insert.Status();
        }

        /// <summary>
        /// دریافت آخرین لاگ‌ها
        /// </summary>
        public List<AuditLog> GetRecentLogs(int count = 50)
        {
            string query = @"
                SELECT TOP (@Count)
                       al.LogId, al.UserId, al.Action, al.TableName, 
                       al.RecordId, al.ActionDate, al.IpAddress,
                       u.Username
                FROM AuditLog al
                LEFT JOIN Users u ON al.UserId = u.UserId
                ORDER BY al.ActionDate DESC";

            var parameters = new Dictionary<string, object>
            {
                { "@Count", count }
            };

            using (var select = new JCGSQLSelect(query, parameters, true, false))
            {
                if (select.Status())
                {
                    var dt = select.GetDataTable();
                    var logs = new List<AuditLog>();

                    foreach (DataRow row in dt.Rows)
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

                    return logs;
                }
            }

            return new List<AuditLog>();
        }

        /// <summary>
        /// دریافت لاگ‌های یک کاربر خاص
        /// </summary>
        public List<AuditLog> GetUserLogs(int userId, int count = 50)
        {
            string query = @"
                SELECT TOP (@Count)
                       LogId, UserId, Action, TableName, RecordId, 
                       OldValue, NewValue, ActionDate, IpAddress
                FROM AuditLog
                WHERE UserId = @UserId
                ORDER BY ActionDate DESC";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@Count", count }
            };

            using (var select = new JCGSQLSelect(query, parameters, true, false))
            {
                if (select.Status())
                {
                    var dt = select.GetDataTable();
                    var logs = new List<AuditLog>();

                    foreach (DataRow row in dt.Rows)
                    {
                        logs.Add(new AuditLog
                        {
                            LogId = Convert.ToInt64(row["LogId"]),
                            UserId = Convert.ToInt32(row["UserId"]),
                            Action = row["Action"].ToString(),
                            TableName = row["TableName"].ToString(),
                            RecordId = row["RecordId"] != DBNull.Value ? Convert.ToInt32(row["RecordId"]) : (int?)null,
                            OldValue = row["OldValue"] != DBNull.Value ? row["OldValue"].ToString() : null,
                            NewValue = row["NewValue"] != DBNull.Value ? row["NewValue"].ToString() : null,
                            ActionDate = Convert.ToDateTime(row["ActionDate"]),
                            IpAddress = row["IpAddress"] != DBNull.Value ? row["IpAddress"].ToString() : null
                        });
                    }

                    return logs;
                }
            }

            return new List<AuditLog>();
        }
    }
}