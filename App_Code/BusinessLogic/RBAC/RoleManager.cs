using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RBACSystem.Models.RBAC;
using RBACSystem.DataAccess.RBAC;

namespace RBACSystem.BusinessLogic.RBAC
{
    // ====================================================================================================
    // RoleManager - مدیریت نقش‌ها (نسخه کامل و بدون خطا)
    // ====================================================================================================

    /// <summary>
    /// مدیریت عملیات مربوط به نقش‌ها
    /// </summary>
    public class RoleManager
    {
        private readonly RoleRepository _roleRepo;
        private readonly UserRoleRepository _userRoleRepo;
        private readonly AuditRepository _auditRepo;

        public RoleManager()
        {
            _roleRepo = new RoleRepository();
            _userRoleRepo = new UserRoleRepository();
            _auditRepo = new AuditRepository();
        }

        #region ایجاد نقش

        /// <summary>
        /// ایجاد نقش جدید
        /// </summary>
        public int CreateRole(Role role, int createdBy)
        {
            try
            {
                // Validation
                if (role == null)
                    throw new ArgumentNullException("role");

                if (string.IsNullOrWhiteSpace(role.RoleName))
                    throw new ArgumentException("نام نقش نمی‌تواند خالی باشد");

                // بررسی تکراری
                if (_roleRepo.GetRoleByName(role.RoleName) != null)
                    throw new InvalidOperationException("این نقش قبلاً ثبت شده است");

                // تنظیم
                role.CreatedBy = createdBy;
                role.CreatedDate = DateTime.Now;
                role.IsActive = true;
                role.IsDeleted = false;

                // ذخیره
                int newRoleId = _roleRepo.InsertRole(role);

                if (newRoleId > 0)
                {
                    LogAudit(createdBy, "Insert", "Roles", newRoleId,
                        null, string.Format("ایجاد نقش جدید: {0}", role.RoleName));
                }

                return newRoleId;
            }
            catch (Exception ex)
            {
                LogError("CreateRole", ex);
                throw;
            }
        }

        #endregion

        #region ویرایش نقش

        /// <summary>
        /// ویرایش نقش
        /// </summary>
        public bool UpdateRole(Role role, int modifiedBy)
        {
            try
            {
                // Validation
                if (role == null)
                    throw new ArgumentNullException("role");

                if (role.RoleId <= 0)
                    throw new ArgumentException("شناسه نقش نامعتبر است");

                if (string.IsNullOrWhiteSpace(role.RoleName))
                    throw new ArgumentException("نام نقش نمی‌تواند خالی باشد");

                // بررسی وجود نقش
                Role existingRole = _roleRepo.GetRoleById(role.RoleId);
                if (existingRole == null)
                    throw new InvalidOperationException("نقش یافت نشد");

                // بررسی تکراری (به جز خودش)
                Role duplicateCheck = _roleRepo.GetRoleByName(role.RoleName);
                if (duplicateCheck != null && duplicateCheck.RoleId != role.RoleId)
                    throw new InvalidOperationException("این نام نقش قبلاً ثبت شده است");

                // تنظیم
                role.ModifiedBy = modifiedBy;
                role.ModifiedDate = DateTime.Now;

                // ذخیره
                bool updated = _roleRepo.UpdateRole(role);

                if (updated)
                {
                    LogAudit(modifiedBy, "Update", "Roles", role.RoleId,
                        existingRole.RoleName, role.RoleName);

                    // پاک کردن Cache کاربران با این نقش
                    ClearRoleUsersCache(role.RoleId);
                }

                return updated;
            }
            catch (Exception ex)
            {
                LogError("UpdateRole", ex);
                throw;
            }
        }

        /// <summary>
        /// حذف نقش (Soft Delete)
        /// </summary>
        public bool DeleteRole(int roleId, int deletedBy)
        {
            try
            {
                if (roleId <= 0)
                    throw new ArgumentException("شناسه نقش نامعتبر است");

                // بررسی وجود نقش
                Role role = _roleRepo.GetRoleById(roleId);
                if (role == null)
                    throw new InvalidOperationException("نقش یافت نشد");

                // حذف (Soft Delete)
                bool deleted = _roleRepo.DeleteRole(roleId, deletedBy);

                if (deleted)
                {
                    LogAudit(deletedBy, "Delete", "Roles", roleId,
                        string.Format("حذف نقش: {0}", role.RoleName), null);

                    ClearRoleUsersCache(roleId);
                }

                return deleted;
            }
            catch (Exception ex)
            {
                LogError("DeleteRole", ex);
                throw;
            }
        }

        #endregion

        #region تخصیص نقش به کاربر

        /// <summary>
        /// تخصیص نقش به کاربر
        /// </summary>
        public bool AssignRoleToUser(int userId, int roleId, int assignedBy)
        {
            try
            {
                // Validation
                if (userId <= 0)
                    throw new ArgumentException("شناسه کاربر نامعتبر است");

                if (roleId <= 0)
                    throw new ArgumentException("شناسه نقش نامعتبر است");

                // بررسی وجود کاربر
                var userRepo = new UserRepository();
                if (userRepo.GetUserById(userId) == null)
                    throw new InvalidOperationException("کاربر یافت نشد");

                // بررسی وجود نقش
                if (_roleRepo.GetRoleById(roleId) == null)
                    throw new InvalidOperationException("نقش یافت نشد");

                // بررسی تکراری
                if (_userRoleRepo.UserHasRole(userId, roleId))
                    throw new InvalidOperationException("این نقش قبلاً به کاربر تخصیص داده شده است");

                // تخصیص
                bool assigned = _userRoleRepo.AssignRoleToUser(userId, roleId, assignedBy);

                if (assigned)
                {
                    LogAudit(assignedBy, "Insert", "UserRoles", userId,
                        null, string.Format("تخصیص نقش {0} به کاربر {1}", roleId, userId));

                    // پاک کردن Cache دسترسی‌های کاربر
                    ClearUserPermissionsCache(userId);
                }

                return assigned;
            }
            catch (Exception ex)
            {
                LogError("AssignRoleToUser", ex);
                throw;
            }
        }

        /// <summary>
        /// حذف نقش از کاربر
        /// </summary>
        public bool RemoveRoleFromUser(int userId, int roleId, int removedBy)
        {
            try
            {
                // Validation
                if (userId <= 0)
                    throw new ArgumentException("شناسه کاربر نامعتبر است");

                if (roleId <= 0)
                    throw new ArgumentException("شناسه نقش نامعتبر است");

                // حذف
                bool removed = _userRoleRepo.RemoveRoleFromUser(userId, roleId);

                if (removed)
                {
                    LogAudit(removedBy, "Delete", "UserRoles", userId,
                        string.Format("حذف نقش {0} از کاربر {1}", roleId, userId), null);

                    // پاک کردن Cache
                    ClearUserPermissionsCache(userId);
                }

                return removed;
            }
            catch (Exception ex)
            {
                LogError("RemoveRoleFromUser", ex);
                throw;
            }
        }

        #endregion

        #region دریافت نقش‌ها

        /// <summary>
        /// دریافت تمام نقش‌ها
        /// </summary>
        public List<Role> GetAllRoles(bool includeInactive = false)
        {
            try
            {
                return _roleRepo.GetAllRoles(includeInactive);
            }
            catch (Exception ex)
            {
                LogError("GetAllRoles", ex);
                throw;
            }
        }

        /// <summary>
        /// دریافت نقش‌های یک کاربر
        /// </summary>
        public List<Role> GetUserRoles(int userId)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentException("شناسه کاربر نامعتبر است");

                return _userRoleRepo.GetUserRoles(userId);
            }
            catch (Exception ex)
            {
                LogError("GetUserRoles", ex);
                throw;
            }
        }

        /// <summary>
        /// دریافت نقش بر اساس شناسه
        /// </summary>
        public Role GetRoleById(int roleId)
        {
            try
            {
                if (roleId <= 0)
                    throw new ArgumentException("شناسه نقش نامعتبر است");

                return _roleRepo.GetRoleById(roleId);
            }
            catch (Exception ex)
            {
                LogError("GetRoleById", ex);
                throw;
            }
        }

        #endregion

        #region متدهای کمکی

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

        private void ClearUserPermissionsCache(int userId)
        {
            // پاک کردن Cache دسترسی‌های کاربر از Session
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                string key = string.Format("UserPermissions_{0}", userId);
                HttpContext.Current.Session.Remove(key);
            }
        }

        /// <summary>
        /// پاک کردن Cache تمام کاربران با نقش خاص
        /// </summary>
        private void ClearRoleUsersCache(int roleId)
        {
            try
            {
                // دریافت کاربران با این نقش
                List<User> usersWithRole = GetUsersWithRole(roleId);

                foreach (User user in usersWithRole)
                {
                    ClearUserPermissionsCache(user.UserId);
                }
            }
            catch { }
        }

        /// <summary>
        /// دریافت کاربران دارای نقش خاص
        /// </summary>
        private List<User> GetUsersWithRole(int roleId)
        {
            try
            {
                string query = @"
                    SELECT DISTINCT u.UserId, u.Username, u.FullName
                    FROM Users u
                    INNER JOIN UserRoles ur ON u.UserId = ur.UserId
                    WHERE ur.RoleId = @RoleId 
                      AND ur.IsDeleted = 0 
                      AND u.IsDeleted = 0";

                var parameters = new Dictionary<string, object>
                {
                    { "@RoleId", roleId }
                };

                using (var select = new JCGSQLSelect(query, parameters, true, false))
                {
                    if (select.Status())
                    {
                        var dt = select.GetDataTable();
                        var users = new List<User>();

                        foreach (System.Data.DataRow row in dt.Rows)
                        {
                            users.Add(new User
                            {
                                UserId = Convert.ToInt32(row["UserId"]),
                                Username = row["Username"].ToString(),
                                FullName = row["FullName"].ToString()
                            });
                        }

                        return users;
                    }
                }
            }
            catch { }

            return new List<User>();
        }

        #endregion
    }
}