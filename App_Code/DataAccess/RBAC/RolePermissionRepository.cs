using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using RBACSystem.Models.RBAC;

namespace RBACSystem.DataAccess.RBAC
{
    // ====================================================================================================
    // 5. RolePermissionRepository - مخزن رابطه نقش و دسترسی
    // ====================================================================================================

    /// <summary>
    /// مخزن عملیات مربوط به تخصیص دسترسی به نقش‌ها
    /// </summary>
    public class RolePermissionRepository
    {
        #region دریافت

        /// <summary>
        /// دریافت دسترسی‌های یک نقش
        /// </summary>
        public List<Permission> GetRolePermissions(int roleId)
        {
            string query = @"
                SELECT p.PermissionId, p.PermissionName, p.DisplayName, 
                       p.Description, p.Category, p.ResourceType, p.ResourcePath,
                       p.IsActive, p.IsDeleted, p.CreatedBy, p.CreatedDate,
                       p.ModifiedBy, p.ModifiedDate
                FROM RolePermissions rp
                INNER JOIN Permissions p ON rp.PermissionId = p.PermissionId
                WHERE rp.RoleId = @RoleId 
                  AND rp.IsDeleted = 0 
                  AND p.IsDeleted = 0
                ORDER BY p.Category, p.PermissionName";

            var parameters = new Dictionary<string, object>
            {
                { "@RoleId", roleId }
            };

            using (var select = new JCGSQLSelect(query, parameters, true, false))
            {
                if (select.Status())
                {
                    var dt = select.GetDataTable();
                    var repo = new PermissionRepository();
                    return repo.MapDataTableToPermissions(dt);
                }
            }

            return new List<Permission>();
        }

        /// <summary>
        /// بررسی اینکه آیا نقش دسترسی خاصی دارد
        /// </summary>
        public bool RoleHasPermission(int roleId, int permissionId)
        {
            string query = @"
                SELECT COUNT(*) 
                FROM RolePermissions 
                WHERE RoleId = @RoleId 
                  AND PermissionId = @PermissionId 
                  AND IsDeleted = 0";

            var parameters = new Dictionary<string, object>
            {
                { "@RoleId", roleId },
                { "@PermissionId", permissionId }
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
        /// تخصیص دسترسی به نقش
        /// </summary>
        public bool AssignPermissionToRole(int roleId, int permissionId, int assignedBy)
        {
            // چک کنیم قبلاً تخصیص داده نشده باشه
            if (RoleHasPermission(roleId, permissionId))
                return false;

            string query = @"
                INSERT INTO RolePermissions (RoleId, PermissionId, AssignedBy, AssignedDate)
                VALUES (@RoleId, @PermissionId, @AssignedBy, @AssignedDate)";

            var parameters = new Dictionary<string, object>
            {
                { "@RoleId", roleId },
                { "@PermissionId", permissionId },
                { "@AssignedBy", assignedBy },
                { "@AssignedDate", DateTime.Now }
            };

            var insert = new JCGSQLInsert(query, parameters, false);
            return insert.Status();
        }

        /// <summary>
        /// حذف دسترسی از نقش
        /// </summary>
        public bool RemovePermissionFromRole(int roleId, int permissionId)
        {
            string query = @"
                UPDATE RolePermissions 
                SET IsDeleted = 1
                WHERE RoleId = @RoleId AND PermissionId = @PermissionId";

            var parameters = new Dictionary<string, object>
            {
                { "@RoleId", roleId },
                { "@PermissionId", permissionId }
            };

            var update = new JCGSQLUpdate(query, parameters, false);
            return update.Status();
        }

        /// <summary>
        /// تخصیص چندین دسترسی به یک نقش
        /// </summary>
        public bool AssignMultiplePermissionsToRole(int roleId, List<int> permissionIds, int assignedBy)
        {
            bool allSuccess = true;

            foreach (int permissionId in permissionIds)
            {
                if (!AssignPermissionToRole(roleId, permissionId, assignedBy))
                    allSuccess = false;
            }

            return allSuccess;
        }

        #endregion
    }
}