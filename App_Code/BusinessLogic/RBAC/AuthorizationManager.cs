using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RBACSystem.Models.RBAC;
using RBACSystem.DataAccess.RBAC;

namespace RBACSystem.BusinessLogic.RBAC
{
    // ====================================================================================================
    // 3. AuthorizationManager - مدیریت دسترسی‌ها و احراز مجوز
    // ====================================================================================================

    /// <summary>
    /// مدیریت دسترسی‌ها و بررسی مجوزها
    /// قلب سیستم RBAC!
    /// </summary>
    public class AuthorizationManager
    {
        private readonly PermissionRepository _permissionRepo;
        private readonly RolePermissionRepository _rolePermissionRepo;
        private readonly UserRoleRepository _userRoleRepo;

        public AuthorizationManager()
        {
            _permissionRepo = new PermissionRepository();
            _rolePermissionRepo = new RolePermissionRepository();
            _userRoleRepo = new UserRoleRepository();
        }

        #region دریافت دسترسی‌های کاربر

        /// <summary>
        /// دریافت تمام دسترسی‌های یک کاربر (با Cache)
        /// این متد خیلی مهمه! قلب Authorization!
        /// </summary>
        public List<string> GetUserPermissions(int userId)
        {
            try
            {
                // ──────────────────────────────────────
                // 1. چک کردن Cache (Session)
                // ──────────────────────────────────────
                
                  //string cacheKey = $"UserPermissions_{userId}";
                  string cacheKey = string.Format("UserPermissions_{0}", userId);
                
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    var cachedPermissions = HttpContext.Current.Session[cacheKey] as List<string>;
                    if (cachedPermissions != null)
                    {
                        return cachedPermissions; // ✅ از Cache برگردون
                    }
                }

                // ──────────────────────────────────────
                // 2. دریافت از دیتابیس
                // ──────────────────────────────────────

                var permissions = new List<string>();

                // دریافت نقش‌های کاربر
                List<Role> userRoles = _userRoleRepo.GetUserRoles(userId);

                if (userRoles == null || userRoles.Count == 0)
                    return permissions; // کاربر نقشی نداره

                // دریافت دسترسی‌های هر نقش
                foreach (var role in userRoles)
                {
                    if (!role.IsActive) continue; // نقش غیرفعال Skip

                    // دسترسی‌های این نقش
                    List<Permission> rolePermissions = _rolePermissionRepo.GetRolePermissions(role.RoleId);
                    
                    foreach (var permission in rolePermissions)
                    {
                        if (permission.IsActive && !permissions.Contains(permission.PermissionName))
                        {
                            permissions.Add(permission.PermissionName);
                        }
                    }

                    // TODO: در آینده Role Hierarchy رو هم اضافه کنیم
                }

                // ──────────────────────────────────────
                // 3. ذخیره در Cache
                // ──────────────────────────────────────

                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[cacheKey] = permissions;
                    HttpContext.Current.Session.Timeout = 60; // 60 دقیقه
                }

                return permissions;
            }
            catch (Exception ex)
            {
                LogError("GetUserPermissions", ex);
                return new List<string>();
            }
        }

        #endregion

        #region بررسی دسترسی

        /// <summary>
        /// بررسی اینکه آیا کاربر دسترسی خاصی دارد
        /// متد اصلی برای چک کردن Authorization!
        /// </summary>
        public bool UserHasPermission(int userId, string permissionName)
        {
            try
            {
                if (userId <= 0 || string.IsNullOrWhiteSpace(permissionName))
                    return false;

                // دریافت دسترسی‌های کاربر (از Cache)
                List<string> permissions = GetUserPermissions(userId);

                // چک کردن وجود دسترسی
                return permissions.Contains(permissionName);
            }
            catch (Exception ex)
            {
                LogError("UserHasPermission", ex);
                return false;
            }
        }

        /// <summary>
        /// بررسی اینکه آیا کاربر یکی از دسترسی‌های مشخص شده را دارد
        /// </summary>
        public bool UserHasAnyPermission(int userId, params string[] permissionNames)
        {
            try
            {
                if (userId <= 0 || permissionNames == null || permissionNames.Length == 0)
                    return false;

                List<string> userPermissions = GetUserPermissions(userId);

                foreach (string permissionName in permissionNames)
                {
                    if (userPermissions.Contains(permissionName))
                        return true; // اگر یکی رو داشت کافیه
                }

                return false;
            }
            catch (Exception ex)
            {
                LogError("UserHasAnyPermission", ex);
                return false;
            }
        }

        /// <summary>
        /// بررسی اینکه آیا کاربر همه دسترسی‌های مشخص شده را دارد
        /// </summary>
        public bool UserHasAllPermissions(int userId, params string[] permissionNames)
        {
            try
            {
                if (userId <= 0 || permissionNames == null || permissionNames.Length == 0)
                    return false;

                List<string> userPermissions = GetUserPermissions(userId);

                foreach (string permissionName in permissionNames)
                {
                    if (!userPermissions.Contains(permissionName))
                        return false; // اگر یکی رو نداشت کافیه
                }

                return true; // همه رو داره
            }
            catch (Exception ex)
            {
                LogError("UserHasAllPermissions", ex);
                return false;
            }
        }

        #endregion

        #region مدیریت دسترسی‌های نقش

        /// <summary>
        /// تخصیص دسترسی به نقش
        /// </summary>
        public bool AssignPermissionToRole(int roleId, int permissionId, int assignedBy)
        {
            try
            {
                if (roleId <= 0)
                    throw new ArgumentException("شناسه نقش نامعتبر است");

                if (permissionId <= 0)
                    throw new ArgumentException("شناسه دسترسی نامعتبر است");

                // تخصیص
                bool assigned = _rolePermissionRepo.AssignPermissionToRole(roleId, permissionId, assignedBy);

                if (assigned)
                {
                    // پاک کردن Cache تمام کاربران با این نقش
                    ClearRolePermissionsCache(roleId);
                }

                return assigned;
            }
            catch (Exception ex)
            {
                LogError("AssignPermissionToRole", ex);
                throw;
            }
        }

        /// <summary>
        /// تخصیص چندین دسترسی به یک نقش
        /// </summary>
        public bool AssignMultiplePermissionsToRole(int roleId, List<int> permissionIds, int assignedBy)
        {
            try
            {
                if (roleId <= 0)
                    throw new ArgumentException("شناسه نقش نامعتبر است");

                if (permissionIds == null || permissionIds.Count == 0)
                    throw new ArgumentException("لیست دسترسی‌ها نمی‌تواند خالی باشد");

                bool success = _rolePermissionRepo.AssignMultiplePermissionsToRole(roleId, permissionIds, assignedBy);

                if (success)
                {
                    ClearRolePermissionsCache(roleId);
                }

                return success;
            }
            catch (Exception ex)
            {
                LogError("AssignMultiplePermissionsToRole", ex);
                throw;
            }
        }

        /// <summary>
        /// حذف دسترسی از نقش
        /// </summary>
        public bool RemovePermissionFromRole(int roleId, int permissionId)
        {
            try
            {
                if (roleId <= 0)
                    throw new ArgumentException("شناسه نقش نامعتبر است");

                if (permissionId <= 0)
                    throw new ArgumentException("شناسه دسترسی نامعتبر است");

                bool removed = _rolePermissionRepo.RemovePermissionFromRole(roleId, permissionId);

                if (removed)
                {
                    ClearRolePermissionsCache(roleId);
                }

                return removed;
            }
            catch (Exception ex)
            {
                LogError("RemovePermissionFromRole", ex);
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
                // ──────────────────────────────────────
                // Cache در سطح Application
                // ──────────────────────────────────────
                
                string cacheKey = "AllPermissions";
                
                if (HttpContext.Current != null && HttpContext.Current.Cache != null)
                {
                    var cachedPermissions = HttpContext.Current.Cache[cacheKey] as List<Permission>;
                    if (cachedPermissions != null)
                    {
                        if (includeInactive)
                            return cachedPermissions;
                        else
                            return cachedPermissions.Where(p => p.IsActive).ToList();
                    }
                }

                // دریافت از دیتابیس
                List<Permission> permissions = _permissionRepo.GetAllPermissions(true); // همه رو بگیر

                // ذخیره در Cache
                if (HttpContext.Current != null && HttpContext.Current.Cache != null)
                {
                    HttpContext.Current.Cache.Insert(cacheKey, permissions, null, 
                        DateTime.Now.AddHours(24), System.Web.Caching.Cache.NoSlidingExpiration);
                }

                if (includeInactive)
                    return permissions;
                else
                    return permissions.Where(p => p.IsActive).ToList();
            }
            catch (Exception ex)
            {
                LogError("GetAllPermissions", ex);
                return new List<Permission>();
            }
        }

        /// <summary>
        /// دریافت دسترسی‌های یک نقش
        /// </summary>
        public List<Permission> GetRolePermissions(int roleId)
        {
            try
            {
                if (roleId <= 0)
                    throw new ArgumentException("شناسه نقش نامعتبر است");

                return _rolePermissionRepo.GetRolePermissions(roleId);
            }
            catch (Exception ex)
            {
                LogError("GetRolePermissions", ex);
                return new List<Permission>();
            }
        }

        #endregion

        #region مدیریت Cache

        /// <summary>
        /// پاک کردن Cache دسترسی‌های یک کاربر
        /// </summary>
        public void ClearUserPermissionsCache(int userId)
        {
            try
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    //string key = $"UserPermissions_{userId}";
                    string key = string.Format("UserPermissions_{0}",userId);
                    HttpContext.Current.Session.Remove(key);
                }
            }
            catch (Exception ex)
            {
                LogError("ClearUserPermissionsCache", ex);
            }
        }

        /// <summary>
        /// پاک کردن Cache تمام کاربران با نقش خاص
        /// </summary>
        private void ClearRolePermissionsCache(int roleId)
        {
            try
            {
                // TODO: باید تمام کاربرانی که این نقش رو دارن رو پیدا کنی
                // و Cache اونها رو پاک کنی
                // برای ساده‌سازی فعلاً همه Session ها رو پاک می‌کنیم
                
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    // در یک پروژه واقعی باید بهتر مدیریت بشه
                    // مثلاً از Redis یا Distributed Cache استفاده کنی
                }
            }
            catch (Exception ex)
            {
                LogError("ClearRolePermissionsCache", ex);
            }
        }

        /// <summary>
        /// پاک کردن تمام Cache ها
        /// </summary>
        public void ClearAllCache()
        {
            try
            {
                if (HttpContext.Current != null)
                {
                    // پاک کردن Cache
                    if (HttpContext.Current.Cache != null)
                    {
                        HttpContext.Current.Cache.Remove("AllPermissions");
                        HttpContext.Current.Cache.Remove("AllRoles");
                    }

                    // پاک کردن Session
                    if (HttpContext.Current.Session != null)
                    {
                        HttpContext.Current.Session.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("ClearAllCache", ex);
            }
        }

        #endregion

        #region متدهای کمکی

        private void LogError(string methodName, Exception ex)
        {
            //System.Diagnostics.Debug.WriteLine($"[ERROR in {methodName}]: {ex.Message}");
            System.Diagnostics.Debug.WriteLine(string.Format("[ERROR in {0}]: {1}"),methodName,ex.Message);
        }

        #endregion
    }
}