using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using RBACSystem.Models.RBAC;

namespace RBACSystem.DataAccess.RBAC
{
    // ====================================================================================================
    // 4. UserRoleRepository - مخزن رابطه کاربر و نقش
    // ====================================================================================================

    /// <summary>
    /// مخزن عملیات مربوط به تخصیص نقش به کاربران
    /// </summary>
    public class UserRoleRepository
    {
        #region دریافت

        /// <summary>
        /// دریافت نقش‌های یک کاربر
        /// </summary>
        public List<Role> GetUserRoles(int userId)
        {
            string query = @"
                SELECT r.RoleId, r.RoleName, r.Description, r.Priority,
                       r.IsActive, r.IsDeleted, r.CreatedBy, r.CreatedDate,
                       r.ModifiedBy, r.ModifiedDate,
                       ur.AssignedDate, u.Username AS AssignedByUsername
                FROM UserRoles ur
                INNER JOIN Roles r ON ur.RoleId = r.RoleId
                LEFT JOIN Users u ON ur.AssignedBy = u.UserId
                WHERE ur.UserId = @UserId 
                  AND ur.IsDeleted = 0 
                  AND r.IsDeleted = 0
                ORDER BY r.Priority DESC";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", userId }
            };

            using (var select = new JCGSQLSelect(query, parameters, true, false))
            {
                if (select.Status())
                {
                    var dt = select.GetDataTable();
                    var roles = new List<Role>();

                    foreach (DataRow row in dt.Rows)
                    {
                        roles.Add(new Role
                        {
                            RoleId = Convert.ToInt32(row["RoleId"]),
                            RoleName = row["RoleName"].ToString(),
                            Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : null,
                            Priority = Convert.ToInt32(row["Priority"]),
                            IsActive = Convert.ToBoolean(row["IsActive"])
                        });
                    }

                    return roles;
                }
            }

            return new List<Role>();
        }

        /// <summary>
        /// بررسی اینکه آیا کاربر نقش خاصی دارد
        /// </summary>
        public bool UserHasRole(int userId, int roleId)
        {
            string query = @"
                SELECT COUNT(*) 
                FROM UserRoles 
                WHERE UserId = @UserId 
                  AND RoleId = @RoleId 
                  AND IsDeleted = 0";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@RoleId", roleId }
            };

            var select = new JCGSQLSelect(query, parameters, false, true);

            if (select.Status())
            {
                var reader = select.GetDataReader();
                int count = reader.GetInt32(0);
                select.CloseConnection();
                return count > 0;
            }

            return false;
        }

        #endregion

        #region تخصیص و حذف

        /// <summary>
        /// تخصیص نقش به کاربر
        /// </summary>
        public bool AssignRoleToUser(int userId, int roleId, int assignedBy)
        {
            // اول چک کنیم که قبلاً تخصیص داده نشده باشه
            if (UserHasRole(userId, roleId))
                return false;

            string query = @"
                INSERT INTO UserRoles (UserId, RoleId, AssignedBy, AssignedDate)
                VALUES (@UserId, @RoleId, @AssignedBy, @AssignedDate)";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@RoleId", roleId },
                { "@AssignedBy", assignedBy },
                { "@AssignedDate", DateTime.Now }
            };

            var insert = new JCGSQLInsert(query, parameters, false);
            return insert.Status();
        }

        /// <summary>
        /// حذف نقش از کاربر (Soft Delete)
        /// </summary>
        public bool RemoveRoleFromUser(int userId, int roleId)
        {
            string query = @"
                UPDATE UserRoles 
                SET IsDeleted = 1
                WHERE UserId = @UserId AND RoleId = @RoleId";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@RoleId", roleId }
            };

            var update = new JCGSQLUpdate(query, parameters, false);
            return update.Status();
        }

        /// <summary>
        /// حذف تمام نقش‌های یک کاربر
        /// </summary>
        public bool RemoveAllRolesFromUser(int userId)
        {
            string query = @"
                UPDATE UserRoles 
                SET IsDeleted = 1
                WHERE UserId = @UserId";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", userId }
            };

            var update = new JCGSQLUpdate(query, parameters, false);
            return update.Status();
        }

        #endregion
    }
}