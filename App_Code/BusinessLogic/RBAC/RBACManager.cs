using System;
using System.Collections.Generic;
using RBACSystem.Models.RBAC;

namespace RBACSystem.BusinessLogic.RBAC
{
    // ====================================================================================================
    // RBACManager - Facade Pattern
    // ====================================================================================================

    /// <summary>
    /// مدیریت کامل RBAC - Facade Pattern
    /// یک نقطه دسترسی واحد به تمام عملیات RBAC
    /// </summary>
    public class RBACManager
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly GroupManager _groupManager;
        private readonly PermissionManager _permissionManager;
        private readonly AuthorizationManager _authManager;

        /// <summary>
        /// سازنده
        /// </summary>
        public RBACManager()
        {
            _userManager = new UserManager();
            _roleManager = new RoleManager();
            _groupManager = new GroupManager();
            _permissionManager = new PermissionManager();
            _authManager = new AuthorizationManager();
        }

        #region User Management

        public int CreateUser(User user, string password, int createdBy)
        {
            return _userManager.CreateUser(user, password, createdBy);
        }

        public bool UpdateUser(User user, int modifiedBy)
        {
            return _userManager.UpdateUser(user, modifiedBy);
        }

        public bool DeleteUser(int userId, int deletedBy)
        {
            return _userManager.DeleteUser(userId, deletedBy);
        }

        public User GetUserById(int userId)
        {
            return _userManager.GetUserById(userId);
        }

        public List<User> GetAllUsers(bool includeInactive = false)
        {
            return _userManager.GetAllUsers(includeInactive);
        }

        public User Login(string username, string password)
        {
            return _userManager.Login(username, password);
        }

        public bool ChangePassword(int userId, string oldPassword, string newPassword, int modifiedBy)
        {
            return _userManager.ChangePassword(userId, oldPassword, newPassword, modifiedBy);
        }

        #endregion

        #region Role Management

        public int CreateRole(Role role, int createdBy)
        {
            return _roleManager.CreateRole(role, createdBy);
        }
        public bool UpdateRole(Role role, int modifyBy)
        {
            return _roleManager.UpdateRole(role, modifyBy);
        }
        public bool AssignRoleToUser(int userId, int roleId, int assignedBy)
        {
            return _roleManager.AssignRoleToUser(userId, roleId, assignedBy);
        }

        public bool RemoveRoleFromUser(int userId, int roleId, int removedBy)
        {
            return _roleManager.RemoveRoleFromUser(userId, roleId, removedBy);
        }

        public List<Role> GetAllRoles(bool includeInactive = false)
        {
            return _roleManager.GetAllRoles(includeInactive);
        }

        public List<Role> GetUserRoles(int userId)
        {
            return _roleManager.GetUserRoles(userId);
        }

        #endregion

        #region Group Management

        public int CreateGroup(UserGroup group, int createdBy)
        {
            return _groupManager.CreateGroup(group, createdBy);
        }

        public bool UpdateGroup(UserGroup group, int modifiedBy)
        {
            return _groupManager.UpdateGroup(group, modifiedBy);
        }

        public bool DeleteGroup(int groupId, int deletedBy)
        {
            return _groupManager.DeleteGroup(groupId, deletedBy);
        }

        public bool AddUserToGroup(int userId, int groupId, int assignedBy)
        {
            return _groupManager.AddUserToGroup(userId, groupId, assignedBy);
        }

        public bool RemoveUserFromGroup(int userId, int groupId, int removedBy)
        {
            return _groupManager.RemoveUserFromGroup(userId, groupId, removedBy);
        }

        public bool AddMultipleUsersToGroup(List<int> userIds, int groupId, int assignedBy)
        {
            return _groupManager.AddMultipleUsersToGroup(userIds, groupId, assignedBy);
        }

        public bool AssignRoleToGroupMembers(int groupId, int roleId, int assignedBy)
        {
            return _groupManager.AssignRoleToGroupMembers(groupId, roleId, assignedBy);
        }

        public bool RemoveRoleFromGroupMembers(int groupId, int roleId, int removedBy)
        {
            return _groupManager.RemoveRoleFromGroupMembers(groupId, roleId, removedBy);
        }

        public List<UserGroup> GetAllGroups(bool includeInactive = false)
        {
            return _groupManager.GetAllGroups(includeInactive);
        }

        public List<User> GetGroupMembers(int groupId)
        {
            return _groupManager.GetGroupMembers(groupId);
        }

        #endregion

        #region Permission Management

        public int CreatePermission(Permission permission, int createdBy)
        {
            return _permissionManager.CreatePermission(permission, createdBy);
        }

        public bool UpdatePermission(Permission permission, int modifiedBy)
        {
            return _permissionManager.UpdatePermission(permission, modifiedBy);
        }

        public bool DeletePermission(int permissionId, int deletedBy)
        {
            return _permissionManager.DeletePermission(permissionId, deletedBy);
        }

        public List<Permission> GetAllPermissions(bool includeInactive = false)
        {
            return _permissionManager.GetAllPermissions(includeInactive);
        }

        public List<string> GetPermissionCategories()
        {
            return _permissionManager.GetPermissionCategories();
        }

        public Dictionary<string, List<Permission>> GetPermissionsGroupedByCategory(bool includeInactive = false)
        {
            return _permissionManager.GetPermissionsGroupedByCategory(includeInactive);
        }

        public bool AssignPermissionsToRole(int roleId, List<int> permissionIds, int assignedBy)
        {
            return _permissionManager.AssignPermissionsToRole(roleId, permissionIds, assignedBy);
        }

        public List<Permission> GetRolePermissions(int roleId)
        {
            return _authManager.GetRolePermissions(roleId);
        }

        #endregion

        #region Authorization

        public List<string> GetUserPermissions(int userId)
        {
            return _authManager.GetUserPermissions(userId);
        }

        public bool UserHasPermission(int userId, string permissionName)
        {
            return _authManager.UserHasPermission(userId, permissionName);
        }

        public bool UserHasAnyPermission(int userId, params string[] permissionNames)
        {
            return _authManager.UserHasAnyPermission(userId, permissionNames);
        }

        public bool UserHasAllPermissions(int userId, params string[] permissionNames)
        {
            return _authManager.UserHasAllPermissions(userId, permissionNames);
        }

        public void ClearUserPermissionsCache(int userId)
        {
            _authManager.ClearUserPermissionsCache(userId);
        }

        public void ClearAllCache()
        {
            _authManager.ClearAllCache();
        }

        #endregion

        #region سناریوهای پیچیده

        /// <summary>
        /// سناریو کامل: ایجاد کاربر + تخصیص نقش + افزودن به گروه
        /// </summary>
        public int CreateUserWithRoleAndGroup(User user, string password, int roleId, int groupId, int createdBy)
        {
            try
            {
                // 1. ایجاد کاربر
                int newUserId = CreateUser(user, password, createdBy);

                if (newUserId > 0)
                {
                    // 2. تخصیص نقش
                    if (roleId > 0)
                    {
                        AssignRoleToUser(newUserId, roleId, createdBy);
                    }

                    // 3. افزودن به گروه
                    if (groupId > 0)
                    {
                        AddUserToGroup(newUserId, groupId, createdBy);
                    }
                }

                return newUserId;
            }
            catch (Exception ex)
            {
                LogError("CreateUserWithRoleAndGroup", ex);
                throw;
            }
        }

        /// <summary>
        /// سناریو: ایجاد نقش + تخصیص دسترسی‌ها
        /// </summary>
        public int CreateRoleWithPermissions(Role role, List<int> permissionIds, int createdBy)
        {
            try
            {
                // 1. ایجاد نقش
                int newRoleId = CreateRole(role, createdBy);

                if (newRoleId > 0 && permissionIds != null && permissionIds.Count > 0)
                {
                    // 2. تخصیص دسترسی‌ها
                    AssignPermissionsToRole(newRoleId, permissionIds, createdBy);
                }

                return newRoleId;
            }
            catch (Exception ex)
            {
                LogError("CreateRoleWithPermissions", ex);
                throw;
            }
        }

        /// <summary>
        /// سناریو: ایجاد گروه + افزودن اعضا + تخصیص نقش آبشاری
        /// </summary>
        public int CreateGroupWithMembersAndRole(UserGroup group, List<int> userIds, int roleId, int createdBy)
        {
            try
            {
                // 1. ایجاد گروه
                int newGroupId = CreateGroup(group, createdBy);

                if (newGroupId > 0)
                {
                    // 2. افزودن اعضا
                    if (userIds != null && userIds.Count > 0)
                    {
                        AddMultipleUsersToGroup(userIds, newGroupId, createdBy);
                    }

                    // 3. تخصیص نقش به تمام اعضا
                    if (roleId > 0)
                    {
                        AssignRoleToGroupMembers(newGroupId, roleId, createdBy);
                    }
                }

                return newGroupId;
            }
            catch (Exception ex)
            {
                LogError("CreateGroupWithMembersAndRole", ex);
                throw;
            }
        }

        /// <summary>
        /// دریافت کامل اطلاعات کاربر (با نقش‌ها و دسترسی‌ها)
        /// </summary>
        public User GetUserFullInfo(int userId)
        {
            try
            {
                User user = GetUserById(userId);

                if (user != null)
                {
                    // دریافت نقش‌ها
                    user.Roles = GetUserRoles(userId);

                    // دریافت دسترسی‌ها
                    user.Permissions = GetUserPermissions(userId);
                }

                return user;
            }
            catch (Exception ex)
            {
                LogError("GetUserFullInfo", ex);
                throw;
            }
        }

        #endregion

        #region Helper Methods

        private void LogError(string methodName, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("[ERROR in RBACManager.{0}]: {1}", methodName, ex.Message));
        }

        #endregion
    }
}