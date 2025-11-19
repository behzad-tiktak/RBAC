using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using RBACSystem.Models.RBAC;

namespace RBACSystem.DataAccess.RBAC
{
    // ====================================================================================================
    // 2. RoleRepository - مخزن نقش‌ها
    // ====================================================================================================

    /// <summary>
    /// مخزن عملیات مربوط به نقش‌ها
    /// </summary>
    public class RoleRepository
    {
        public Role GetRoleById(int roleId)
        {
            string query = @"
                SELECT RoleId, RoleName, Description, Priority, 
                       IsActive, IsDeleted, CreatedBy, CreatedDate,
                       ModifiedBy, ModifiedDate
                FROM Roles
                WHERE RoleId = @RoleId AND IsDeleted = 0";

            var parameters = new Dictionary<string, object>
            {
                { "@RoleId", roleId }
            };

            using (var select = new JCGSQLSelect(query, parameters, false, true))
            {
                if (select.Status())
                {
                    var reader = select.GetDataReader();
                    var role = MapReaderToRole(reader);
                    select.CloseConnection();
                    return role;
                }
            }

            return null;
        }

        public Role GetRoleByName(string roleName)
        {
            string query = @"
                SELECT RoleId, RoleName, Description, Priority, 
                       IsActive, IsDeleted, CreatedBy, CreatedDate,
                       ModifiedBy, ModifiedDate
                FROM Roles
                WHERE RoleName = @RoleName AND IsDeleted = 0";

            var parameters = new Dictionary<string, object>
            {
                { "@RoleName", roleName }
            };

            using (var select = new JCGSQLSelect(query, parameters, false, true))
            {
                if (select.Status())
                {
                    var reader = select.GetDataReader();
                    var role = MapReaderToRole(reader);
                    select.CloseConnection();
                    return role;
                }
            }

            return null;
        }

        public List<Role> GetAllRoles(bool includeInactive)
        {
            string query = @"
                SELECT RoleId, RoleName, Description, Priority, 
                       IsActive, IsDeleted, CreatedBy, CreatedDate,
                       ModifiedBy, ModifiedDate,
                       (SELECT COUNT(*) FROM UserRoles WHERE RoleId = r.RoleId AND IsDeleted = 0) AS UserCount
                FROM Roles r
                WHERE IsDeleted = 0";

            if (!includeInactive)
                query += " AND IsActive = 1";

            query += " ORDER BY Priority DESC";

            using (var select = new JCGSQLSelect(query, true, false))
            {
                if (select.Status())
                {
                    var dt = select.GetDataTable();
                    return MapDataTableToRoles(dt);
                }
            }

            return new List<Role>();
        }

        public int InsertRole(Role role)
        {
            string query = @"
                INSERT INTO Roles (RoleName, Description, Priority, IsActive, CreatedBy, CreatedDate)
                VALUES (@RoleName, @Description, @Priority, @IsActive, @CreatedBy, @CreatedDate);
                SELECT SCOPE_IDENTITY();";

            var parameters = new Dictionary<string, object>
            {
                { "@RoleName", role.RoleName },
                { "@Description", role.Description ?? (object)DBNull.Value },
                { "@Priority", role.Priority },
                { "@IsActive", role.IsActive },
                { "@CreatedBy", role.CreatedBy ?? (object)DBNull.Value },
                { "@CreatedDate", role.CreatedDate }
            };

            var insert = new JCGSQLInsert(query, parameters, true);

            if (insert.Status())
            {
                try
                {
                    return Convert.ToInt32(insert.ReturnValue);
                }
                catch
                {
                    return 0;
                }
            }

            return 0;
        }

        public bool UpdateRole(Role role)
        {
            string query = @"
                UPDATE Roles 
                SET RoleName = @RoleName,
                    Description = @Description,
                    Priority = @Priority,
                    IsActive = @IsActive,
                    ModifiedBy = @ModifiedBy,
                    ModifiedDate = @ModifiedDate
                WHERE RoleId = @RoleId AND IsDeleted = 0";

            var parameters = new Dictionary<string, object>
            {
                { "@RoleId", role.RoleId },
                { "@RoleName", role.RoleName },
                { "@Description", role.Description ?? (object)DBNull.Value },
                { "@Priority", role.Priority },
                { "@IsActive", role.IsActive },
                { "@ModifiedBy", role.ModifiedBy ?? (object)DBNull.Value },
                { "@ModifiedDate", DateTime.Now }
            };

            var update = new JCGSQLUpdate(query, parameters, false);
            return update.Status();
        }

        public bool DeleteRole(int roleId, int deletedBy)
        {
            string query = @"
                UPDATE Roles 
                SET IsDeleted = 1,
                    IsActive = 0,
                    ModifiedBy = @ModifiedBy,
                    ModifiedDate = @ModifiedDate
                WHERE RoleId = @RoleId";

            var parameters = new Dictionary<string, object>
            {
                { "@RoleId", roleId },
                { "@ModifiedBy", deletedBy },
                { "@ModifiedDate", DateTime.Now }
            };

            var update = new JCGSQLUpdate(query, parameters, false);
            return update.Status();
        }

        private Role MapReaderToRole(SqlDataReader reader)
        {
            return new Role
            {
                RoleId = reader.GetInt32(0),
                RoleName = reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                Priority = reader.GetInt32(3),
                IsActive = reader.GetBoolean(4),
                IsDeleted = reader.GetBoolean(5),
                CreatedBy = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                CreatedDate = reader.GetDateTime(7),
                ModifiedBy = reader.IsDBNull(8) ? (int?)null : reader.GetInt32(8),
                ModifiedDate = reader.IsDBNull(9) ? (DateTime?)null : reader.GetDateTime(9)
            };
        }

        private List<Role> MapDataTableToRoles(DataTable dt)
        {
            var roles = new List<Role>();

            foreach (DataRow row in dt.Rows)
            {
                roles.Add(new Role
                {
                    RoleId = Convert.ToInt32(row["RoleId"]),
                    RoleName = row["RoleName"].ToString(),
                    Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : null,
                    Priority = Convert.ToInt32(row["Priority"]),
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    IsDeleted = Convert.ToBoolean(row["IsDeleted"]),
                    CreatedBy = row["CreatedBy"] != DBNull.Value ? Convert.ToInt32(row["CreatedBy"]) : (int?)null,
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    ModifiedBy = row["ModifiedBy"] != DBNull.Value ? Convert.ToInt32(row["ModifiedBy"]) : (int?)null,
                    ModifiedDate = row["ModifiedDate"] != DBNull.Value ? Convert.ToDateTime(row["ModifiedDate"]) : (DateTime?)null,
                    UserCount = dt.Columns.Contains("UserCount") ? Convert.ToInt32(row["UserCount"]) : 0
                });
            }

            return roles;
        }
    }
}