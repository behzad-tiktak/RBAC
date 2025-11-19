using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using RBACSystem.Models.RBAC;

namespace RBACSystem.DataAccess.RBAC
{
    // ====================================================================================================
    // 3. PermissionRepository - مخزن دسترسی‌ها
    // ====================================================================================================

    /// <summary>
    /// مخزن عملیات مربوط به دسترسی‌ها
    /// </summary>
    public class PermissionRepository
    {
        #region دریافت دسترسی

        /// <summary>
        /// دریافت دسترسی بر اساس شناسه
        /// </summary>
        public Permission GetPermissionById(int permissionId)
        {
            string query = @"
                SELECT PermissionId, PermissionName, DisplayName, Description,
                       Category, ResourceType, ResourcePath,
                       IsActive, IsDeleted, CreatedBy, CreatedDate,
                       ModifiedBy, ModifiedDate
                FROM Permissions
                WHERE PermissionId = @PermissionId AND IsDeleted = 0";

            var parameters = new Dictionary<string, object>
            {
                { "@PermissionId", permissionId }
            };

            using (var select = new JCGSQLSelect(query, parameters, false, true))
            {
                if (select.Status())
                {
                    var reader = select.GetDataReader();
                    var permission = MapReaderToPermission(reader);
                    select.CloseConnection();
                    return permission;
                }
            }

            return null;
        }

        /// <summary>
        /// دریافت دسترسی بر اساس نام
        /// </summary>
        public Permission GetPermissionByName(string permissionName)
        {
            string query = @"
                SELECT PermissionId, PermissionName, DisplayName, Description,
                       Category, ResourceType, ResourcePath,
                       IsActive, IsDeleted, CreatedBy, CreatedDate,
                       ModifiedBy, ModifiedDate
                FROM Permissions
                WHERE PermissionName = @PermissionName AND IsDeleted = 0";

            var parameters = new Dictionary<string, object>
            {
                { "@PermissionName", permissionName }
            };

            using (var select = new JCGSQLSelect(query, parameters, false, true))
            {
                if (select.Status())
                {
                    var reader = select.GetDataReader();
                    var permission = MapReaderToPermission(reader);
                    select.CloseConnection();
                    return permission;
                }
            }

            return null;
        }

        /// <summary>
        /// دریافت تمام دسترسی‌ها
        /// </summary>
        public List<Permission> GetAllPermissions(bool includeInactive = false)
        {
            string query = @"
                SELECT PermissionId, PermissionName, DisplayName, Description,
                       Category, ResourceType, ResourcePath,
                       IsActive, IsDeleted, CreatedBy, CreatedDate,
                       ModifiedBy, ModifiedDate
                FROM Permissions
                WHERE IsDeleted = 0";

            if (!includeInactive)
                query += " AND IsActive = 1";

            query += " ORDER BY Category, PermissionName";

            using (var select = new JCGSQLSelect(query, true, false))
            {
                if (select.Status())
                {
                    var dt = select.GetDataTable();
                    return MapDataTableToPermissions(dt);
                }
            }

            return new List<Permission>();
        }

        /// <summary>
        /// دریافت دسترسی‌ها بر اساس دسته
        /// </summary>
        public List<Permission> GetPermissionsByCategory(string category)
        {
            string query = @"
                SELECT PermissionId, PermissionName, DisplayName, Description,
                       Category, ResourceType, ResourcePath,
                       IsActive, IsDeleted, CreatedBy, CreatedDate,
                       ModifiedBy, ModifiedDate
                FROM Permissions
                WHERE Category = @Category AND IsDeleted = 0 AND IsActive = 1
                ORDER BY PermissionName";

            var parameters = new Dictionary<string, object>
            {
                { "@Category", category }
            };

            using (var select = new JCGSQLSelect(query, parameters, true, false))
            {
                if (select.Status())
                {
                    var dt = select.GetDataTable();
                    return MapDataTableToPermissions(dt);
                }
            }

            return new List<Permission>();
        }

        #endregion

        #region ثبت و ویرایش

        /// <summary>
        /// ثبت دسترسی جدید
        /// </summary>
        public int InsertPermission(Permission permission)
        {
            string query = @"
                INSERT INTO Permissions (PermissionName, DisplayName, Description, 
                                        Category, ResourceType, ResourcePath,
                                        IsActive, CreatedBy, CreatedDate)
                VALUES (@PermissionName, @DisplayName, @Description, 
                        @Category, @ResourceType, @ResourcePath,
                        @IsActive, @CreatedBy, @CreatedDate);
                SELECT SCOPE_IDENTITY();";

            var parameters = new Dictionary<string, object>
            {
                { "@PermissionName", permission.PermissionName },
                { "@DisplayName", permission.DisplayName },
                { "@Description", permission.Description ?? (object)DBNull.Value },
                { "@Category", permission.Category ?? (object)DBNull.Value },
                { "@ResourceType", permission.ResourceType ?? (object)DBNull.Value },
                { "@ResourcePath", permission.ResourcePath ?? (object)DBNull.Value },
                { "@IsActive", permission.IsActive },
                { "@CreatedBy", permission.CreatedBy ?? (object)DBNull.Value },
                { "@CreatedDate", permission.CreatedDate }
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

        /// <summary>
        /// ویرایش دسترسی
        /// </summary>
        public bool UpdatePermission(Permission permission)
        {
            string query = @"
                UPDATE Permissions 
                SET PermissionName = @PermissionName,
                    DisplayName = @DisplayName,
                    Description = @Description,
                    Category = @Category,
                    ResourceType = @ResourceType,
                    ResourcePath = @ResourcePath,
                    IsActive = @IsActive,
                    ModifiedBy = @ModifiedBy,
                    ModifiedDate = @ModifiedDate
                WHERE PermissionId = @PermissionId AND IsDeleted = 0";

            var parameters = new Dictionary<string, object>
            {
                { "@PermissionId", permission.PermissionId },
                { "@PermissionName", permission.PermissionName },
                { "@DisplayName", permission.DisplayName },
                { "@Description", permission.Description ?? (object)DBNull.Value },
                { "@Category", permission.Category ?? (object)DBNull.Value },
                { "@ResourceType", permission.ResourceType ?? (object)DBNull.Value },
                { "@ResourcePath", permission.ResourcePath ?? (object)DBNull.Value },
                { "@IsActive", permission.IsActive },
                { "@ModifiedBy", permission.ModifiedBy ?? (object)DBNull.Value },
                { "@ModifiedDate", DateTime.Now }
            };

            var update = new JCGSQLUpdate(query, parameters, false);
            return update.Status();
        }

        /// <summary>
        /// حذف دسترسی (Soft Delete)
        /// </summary>
        public bool DeletePermission(int permissionId, int deletedBy)
        {
            string query = @"
                UPDATE Permissions 
                SET IsDeleted = 1,
                    IsActive = 0,
                    ModifiedBy = @ModifiedBy,
                    ModifiedDate = @ModifiedDate
                WHERE PermissionId = @PermissionId";

            var parameters = new Dictionary<string, object>
            {
                { "@PermissionId", permissionId },
                { "@ModifiedBy", deletedBy },
                { "@ModifiedDate", DateTime.Now }
            };

            var update = new JCGSQLUpdate(query, parameters, false);
            return update.Status();
        }

        #endregion

        #region بررسی استفاده

        /// <summary>
        /// بررسی اینکه آیا دسترسی در نقش‌ها استفاده شده
        /// </summary>
        public int GetPermissionUsageCount(int permissionId)
        {
            string query = @"
                SELECT COUNT(*) 
                FROM RolePermissions 
                WHERE PermissionId = @PermissionId 
                  AND IsDeleted = 0";

            var parameters = new Dictionary<string, object>
            {
                { "@PermissionId", permissionId }
            };

            using (var select = new JCGSQLSelect(query, parameters, false, true))
            {
                if (select.Status())
                {
                    var reader = select.GetDataReader();
                    int count = reader.GetInt32(0);
                    select.CloseConnection();
                    return count;
                }
            }

            return 0;
        }

        /// <summary>
        /// دریافت نقش‌هایی که از این دسترسی استفاده می‌کنند
        /// </summary>
        public List<Role> GetRolesUsingPermission(int permissionId)
        {
            string query = @"
                SELECT DISTINCT r.RoleId, r.RoleName, r.Description, r.Priority, r.IsActive
                FROM Roles r
                INNER JOIN RolePermissions rp ON r.RoleId = rp.RoleId
                WHERE rp.PermissionId = @PermissionId 
                  AND rp.IsDeleted = 0 
                  AND r.IsDeleted = 0
                ORDER BY r.RoleName";

            var parameters = new Dictionary<string, object>
            {
                { "@PermissionId", permissionId }
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

        #endregion

        #region متدهای کمکی

        private Permission MapReaderToPermission(SqlDataReader reader)
        {
            return new Permission
            {
                PermissionId = reader.GetInt32(0),
                PermissionName = reader.GetString(1),
                DisplayName = reader.GetString(2),
                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                Category = reader.IsDBNull(4) ? null : reader.GetString(4),
                ResourceType = reader.IsDBNull(5) ? null : reader.GetString(5),
                ResourcePath = reader.IsDBNull(6) ? null : reader.GetString(6),
                IsActive = reader.GetBoolean(7),
                IsDeleted = reader.GetBoolean(8),
                CreatedBy = reader.IsDBNull(9) ? (int?)null : reader.GetInt32(9),
                CreatedDate = reader.GetDateTime(10),
                ModifiedBy = reader.IsDBNull(11) ? (int?)null : reader.GetInt32(11),
                ModifiedDate = reader.IsDBNull(12) ? (DateTime?)null : reader.GetDateTime(12)
            };
        }

        public List<Permission> MapDataTableToPermissions(DataTable dt)
        {
            var permissions = new List<Permission>();

            foreach (DataRow row in dt.Rows)
            {
                permissions.Add(new Permission
                {
                    PermissionId = Convert.ToInt32(row["PermissionId"]),
                    PermissionName = row["PermissionName"].ToString(),
                    DisplayName = row["DisplayName"].ToString(),
                    Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : null,
                    Category = row["Category"] != DBNull.Value ? row["Category"].ToString() : null,
                    ResourceType = row["ResourceType"] != DBNull.Value ? row["ResourceType"].ToString() : null,
                    ResourcePath = row["ResourcePath"] != DBNull.Value ? row["ResourcePath"].ToString() : null,
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    IsDeleted = Convert.ToBoolean(row["IsDeleted"]),
                    CreatedBy = row["CreatedBy"] != DBNull.Value ? Convert.ToInt32(row["CreatedBy"]) : (int?)null,
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    ModifiedBy = row["ModifiedBy"] != DBNull.Value ? Convert.ToInt32(row["ModifiedBy"]) : (int?)null,
                    ModifiedDate = row["ModifiedDate"] != DBNull.Value ? Convert.ToDateTime(row["ModifiedDate"]) : (DateTime?)null
                });
            }

            return permissions;
        }

        #endregion
    }
}