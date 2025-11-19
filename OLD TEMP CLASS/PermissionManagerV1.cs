using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RBACSystem.Models.RBAC;
using RBACSystem.DataAccess.RBAC;

namespace RBACSystem.BusinessLogic.RBAC
{
    // ====================================================================================================
    // 5. PermissionManager - مدیریت دسترسی‌ها
    // ====================================================================================================

    /// <summary>
    /// مدیریت عملیات مربوط به دسترسی‌ها
    /// ایجاد، ویرایش، دسته‌بندی دسترسی‌ها
    /// </summary>
    public class PermissionManager
    {
        private readonly PermissionRepository _permissionRepo;
        private readonly RolePermissionRepository _rolePermissionRepo;
        private readonly AuditRepository _auditRepo;

        public PermissionManager()
        {
            _permissionRepo = new PermissionRepository();
            _rolePermissionRepo = new RolePermissionRepository();
            _auditRepo = new AuditRepository();
        }

        #region ایجاد و ویرایش دسترسی

        /// <summary>
        /// ایجاد دسترسی جدید
        /// </summary>
        public int CreatePermission(Permission permission, int createdBy)
        {
            try
            {
                // Validation
                if (permission == null)
                    throw new ArgumentNullException("permission", "اطلاعات دسترسی نمی‌تواند خالی باشد");

                if (string.IsNullOrWhiteSpace(permission.PermissionName))
                    throw new ArgumentException("نام دسترسی نمی‌تواند خالی باشد");

                if (string.IsNullOrWhiteSpace(permission.DisplayName))
                    throw new ArgumentException("نام نمایشی نمی‌تواند خالی باشد");

                // فرمت نام: Category.Action
                if (!IsValidPermissionNameFormat(permission.PermissionName))
                    throw new ArgumentException("فرمت نام دسترسی باید به صورت Category.Action باشد (مثلاً Users.View)");

                // بررسی تکراری
                if (_permissionRepo.GetPermissionByName(permission.PermissionName) != null)
                    throw new InvalidOperationException("این دسترسی قبلاً ثبت شده است");

                // اگر Category خالی باشد، از PermissionName استخراج کن
                if (string.IsNullOrWhiteSpace(permission.Category))
                {
                    permission.Category = ExtractCategory(permission.PermissionName);
                }

                // تنظیم
                permission.CreatedBy = createdBy;
                permission.CreatedDate = DateTime.Now;
                permission.IsActive = true;
                permission.IsDeleted = false;

                // ذخیره (این متد باید در Repository اضافه شود)
                int newPermissionId = InsertPermission(permission);

                if (newPermissionId > 0)
                {
                    LogAudit(createdBy, "Insert", "Permissions", newPermissionId,
                        null, string.Format("ایجاد دسترسی جدید: {0}", permission.PermissionName));

                    // پاک کردن Cache
                    ClearPermissionsCache();
                }

                return newPermissionId;
            }
            catch (Exception ex)
            {
                LogError("CreatePermission", ex);
                throw;
            }
        }

        /// <summary>
        /// ویرایش دسترسی
        /// </summary>
        public bool UpdatePermission(Permission permission, int modifiedBy)
        {
            try
            {
                // Validation
                if (permission == null)
                    throw new ArgumentNullException("permission");

                if (permission.PermissionId <= 0)
                    throw new ArgumentException("شناسه دسترسی نامعتبر است");

                // بررسی وجود
                Permission existingPermission = _permissionRepo.GetPermissionById(permission.PermissionId);
                if (existingPermission == null)
                    throw new InvalidOperationException("دسترسی یافت نشد");

                // بررسی تکراری نام (به جز خودش)
                Permission duplicateCheck = _permissionRepo.GetPermissionByName(permission.PermissionName);
                if (duplicateCheck != null && duplicateCheck.PermissionId != permission.PermissionId)
                    throw new InvalidOperationException("این نام دسترسی قبلاً ثبت شده است");

                // تنظیم
                permission.ModifiedBy = modifiedBy;
                permission.ModifiedDate = DateTime.Now;

                // ذخیره (این متد باید در Repository اضافه شود)
                bool updated = UpdatePermissionInRepo(permission);

                if (updated)
                {
                    LogAudit(modifiedBy, "Update", "Permissions", permission.PermissionId,
                        existingPermission.PermissionName, permission.PermissionName);

                    // پاک کردن Cache
                    ClearPermissionsCache();
                }

                return updated;
            }
            catch (Exception ex)
            {
                LogError("UpdatePermission", ex);
                throw;
            }
        }

        /// <summary>
        /// حذف دسترسی (Soft Delete)
        /// توجه: قبل از حذف، باید از تمام نقش‌ها حذف شود
        /// </summary>
        public bool DeletePermission(int permissionId, int deletedBy)
        {
            try
            {
                if (permissionId <= 0)
                    throw new ArgumentException("شناسه دسترسی نامعتبر است");

                // بررسی وجود
                Permission permission = _permissionRepo.GetPermissionById(permissionId);
                if (permission == null)
                    throw new InvalidOperationException("دسترسی یافت نشد");

                // بررسی استفاده در نقش‌ها
                // TODO: باید متد CheckPermissionUsage در Repository اضافه شود

                // حذف (Soft Delete)
                // این متد باید در Repository اضافه شود
                bool deleted = DeletePermissionInRepo(permissionId, deletedBy);

                if (deleted)
                {
                    LogAudit(deletedBy, "Delete", "Permissions", permissionId,
                        string.Format("حذف دسترسی: {0}", permission.PermissionName), null);

                    // پاک کردن Cache
                    ClearPermissionsCache();
                }

                return deleted;
            }
            catch (Exception ex)
            {
                LogError("DeletePermission", ex);
                throw;
            }
        }

        #endregion

        #region دریافت دسترسی‌ها

        /// <summary>
        /// دریافت تمام دسترسی‌ها
        /// </summary>
        public List<Permission> GetAllPermissions(bool includeInactive = false)
        {
            try
            {
                return _permissionRepo.GetAllPermissions(includeInactive);
            }
            catch (Exception ex)
            {
                LogError("GetAllPermissions", ex);
                throw;
            }
        }

        /// <summary>
        /// دریافت دسترسی بر اساس شناسه
        /// </summary>
        public Permission GetPermissionById(int permissionId)
        {
            try
            {
                if (permissionId <= 0)
                    throw new ArgumentException("شناسه دسترسی نامعتبر است");

                return _permissionRepo.GetPermissionById(permissionId);
            }
            catch (Exception ex)
            {
                LogError("GetPermissionById", ex);
                throw;
            }
        }

        /// <summary>
        /// دریافت دسترسی بر اساس نام
        /// </summary>
        public Permission GetPermissionByName(string permissionName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(permissionName))
                    throw new ArgumentException("نام دسترسی نمی‌تواند خالی باشد");

                return _permissionRepo.GetPermissionByName(permissionName);
            }
            catch (Exception ex)
            {
                LogError("GetPermissionByName", ex);
                throw;
            }
        }

        /// <summary>
        /// دریافت دسترسی‌ها بر اساس دسته
        /// </summary>
        public List<Permission> GetPermissionsByCategory(string category)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(category))
                    throw new ArgumentException("نام دسته نمی‌تواند خالی باشد");

                return _permissionRepo.GetPermissionsByCategory(category);
            }
            catch (Exception ex)
            {
                LogError("GetPermissionsByCategory", ex);
                throw;
            }
        }

        /// <summary>
        /// دریافت لیست دسته‌های دسترسی (یکتا)
        /// </summary>
        public List<string> GetPermissionCategories()
        {
            try
            {
                List<Permission> allPermissions = _permissionRepo.GetAllPermissions(false);
                
                var categories = new List<string>();
                
                foreach (Permission permission in allPermissions)
                {
                    if (!string.IsNullOrWhiteSpace(permission.Category) && 
                        !categories.Contains(permission.Category))
                    {
                        categories.Add(permission.Category);
                    }
                }

                categories.Sort();
                return categories;
            }
            catch (Exception ex)
            {
                LogError("GetPermissionCategories", ex);
                return new List<string>();
            }
        }

        /// <summary>
        /// دریافت دسترسی‌ها به صورت گروه‌بندی شده بر اساس دسته
        /// مناسب برای نمایش در CheckBoxList
        /// </summary>
        public Dictionary<string, List<Permission>> GetPermissionsGroupedByCategory(bool includeInactive = false)
        {
            try
            {
                List<Permission> allPermissions = _permissionRepo.GetAllPermissions(includeInactive);
                var grouped = new Dictionary<string, List<Permission>>();

                foreach (Permission permission in allPermissions)
                {
                    string category = string.IsNullOrWhiteSpace(permission.Category) 
                        ? "Other" 
                        : permission.Category;

                    if (!grouped.ContainsKey(category))
                    {
                        grouped[category] = new List<Permission>();
                    }

                    grouped[category].Add(permission);
                }

                return grouped;
            }
            catch (Exception ex)
            {
                LogError("GetPermissionsGroupedByCategory", ex);
                return new Dictionary<string, List<Permission>>();
            }
        }

        #endregion

        #region تخصیص دسترسی به نقش

        /// <summary>
        /// تخصیص چندین دسترسی به یک نقش
        /// مناسب برای استفاده با CheckBoxList
        /// </summary>
        public bool AssignPermissionsToRole(int roleId, List<int> permissionIds, int assignedBy)
        {
            try
            {
                if (roleId <= 0)
                    throw new ArgumentException("شناسه نقش نامعتبر است");

                if (permissionIds == null || permissionIds.Count == 0)
                    throw new ArgumentException("لیست دسترسی‌ها نمی‌تواند خالی باشد");

                // پاک کردن دسترسی‌های قبلی نقش
                RemoveAllPermissionsFromRole(roleId);

                // تخصیص دسترسی‌های جدید
                bool success = _rolePermissionRepo.AssignMultiplePermissionsToRole(roleId, permissionIds, assignedBy);

                if (success)
                {
                    LogAudit(assignedBy, "Update", "RolePermissions", roleId,
                        null, string.Format("تخصیص {0} دسترسی به نقش", permissionIds.Count));

                    // پاک کردن Cache کاربران با این نقش
                    ClearRolePermissionsCache(roleId);
                }

                return success;
            }
            catch (Exception ex)
            {
                LogError("AssignPermissionsToRole", ex);
                throw;
            }
        }

        /// <summary>
        /// حذف تمام دسترسی‌های یک نقش
        /// </summary>
        private bool RemoveAllPermissionsFromRole(int roleId)
        {
            try
            {
                // دریافت دسترسی‌های فعلی
                List<Permission> currentPermissions = _rolePermissionRepo.GetRolePermissions(roleId);

                foreach (Permission permission in currentPermissions)
                {
                    _rolePermissionRepo.RemovePermissionFromRole(roleId, permission.PermissionId);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region متدهای کمکی

        /// <summary>
        /// بررسی فرمت نام دسترسی (Category.Action)
        /// </summary>
        private bool IsValidPermissionNameFormat(string permissionName)
        {
            if (string.IsNullOrWhiteSpace(permissionName))
                return false;

            // باید شامل دقیقاً یک نقطه باشد
            int dotCount = 0;
            foreach (char c in permissionName)
            {
                if (c == '.') dotCount++;
            }

            return dotCount == 1 && permissionName.Length > 3;
        }

        /// <summary>
        /// استخراج دسته از نام دسترسی (قسمت قبل از نقطه)
        /// </summary>
        private string ExtractCategory(string permissionName)
        {
            if (string.IsNullOrWhiteSpace(permissionName))
                return "Other";

            int dotIndex = permissionName.IndexOf('.');
            if (dotIndex > 0)
            {
                return permissionName.Substring(0, dotIndex);
            }

            return "Other";
        }

        /// <summary>
        /// پاک کردن Cache دسترسی‌ها
        /// </summary>
        private void ClearPermissionsCache()
        {
            try
            {
                if (HttpContext.Current != null && HttpContext.Current.Cache != null)
                {
                    HttpContext.Current.Cache.Remove("AllPermissions");
                }
            }
            catch { }
        }

        /// <summary>
        /// پاک کردن Cache کاربران با نقش خاص
        /// </summary>
        private void ClearRolePermissionsCache(int roleId)
        {
            try
            {
                // TODO: باید تمام کاربران با این نقش رو پیدا کنی و Cache اونها رو پاک کنی
                // در پروژه واقعی از Distributed Cache استفاده کن
            }
            catch { }
        }

        // ────────────────────────────────────────────────────────────────
        // متدهای Repository که باید اضافه شوند
        // ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Insert Permission - موقتی (باید در PermissionRepository اضافه شود)
        /// </summary>
        private int InsertPermission(Permission permission)
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
        /// Update Permission - موقتی (باید در PermissionRepository اضافه شود)
        /// </summary>
        private bool UpdatePermissionInRepo(Permission permission)
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
        /// Delete Permission - موقتی (باید در PermissionRepository اضافه شود)
        /// </summary>
        private bool DeletePermissionInRepo(int permissionId, int deletedBy)
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

        private void LogAudit(int userId, string action, string tableName, int recordId, string oldValue, string newValue)
        {
            try
            {
                var log = new AuditLog
                {
                    UserId = userId,
                    Action = action,
                    TableName = tableName,
                    RecordId = recordId,
                    OldValue = oldValue,
                    NewValue = newValue,
                    ActionDate = DateTime.Now
                };

                _auditRepo.InsertLog(log);
            }
            catch { }
        }

        private void LogError(string methodName, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("[ERROR in {0}]: {1}", methodName, ex.Message));
        }

        #endregion
    }
}