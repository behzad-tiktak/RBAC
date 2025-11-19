using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RBACSystem.Models.RBAC;
using RBACSystem.DataAccess.RBAC;

namespace RBACSystem.BusinessLogic.RBAC
{
    // ====================================================================================================
    // 4. GroupManager - مدیریت گروه‌های کاربری
    // ====================================================================================================

    /// <summary>
    /// مدیریت عملیات مربوط به گروه‌های کاربری
    /// شامل: مدیریت اعضا، تخصیص آبشاری دسترسی‌ها
    /// </summary>
    public class GroupManager
    {
        private readonly UserGroupRepository _groupRepo;
        private readonly UserRoleRepository _userRoleRepo;
        private readonly AuditRepository _auditRepo;
        private readonly AuthorizationManager _authManager;
        string auditLogMessage = "";
        string exeptionMessage;
        public GroupManager()
        {
            _groupRepo = new UserGroupRepository();
            _userRoleRepo = new UserRoleRepository();
            _auditRepo = new AuditRepository();
            _authManager = new AuthorizationManager();
        }

        #region ایجاد و ویرایش گروه

        /// <summary>
        /// ایجاد گروه جدید
        /// </summary>
        public int CreateGroup(UserGroup group, int createdBy)
        {
            try
            {
                // Validation
                if (group == null)
                    throw new ArgumentNullException("group", "اطلاعات گروه نمی‌تواند خالی باشد");

                if (string.IsNullOrWhiteSpace(group.GroupName))
                    throw new ArgumentException("نام گروه نمی‌تواند خالی باشد");

                if (group.GroupName.Length < 2 || group.GroupName.Length > 100)
                    throw new ArgumentException("نام گروه باید بین 2 تا 100 کاراکتر باشد");

                // بررسی تکراری بودن نام
                if (IsGroupNameExists(group.GroupName, null))
                    throw new InvalidOperationException("این نام گروه قبلاً ثبت شده است");

                // تنظیم
                group.CreatedBy = createdBy;
                group.CreatedDate = DateTime.Now;
                group.IsActive = true;
                group.IsDeleted = false;

                // ذخیره
                int newGroupId = _groupRepo.InsertGroup(group);

                if (newGroupId > 0)
                {
                    auditLogMessage = "ایجاد گروه جدید";
                    auditLogMessage = "ایجاد گروه جدید: " + group.GroupName;
                    LogAudit(createdBy, "Insert", "UserGroups", newGroupId, null, auditLogMessage);
                }

                return newGroupId;
            }
            catch (Exception ex)
            {
                LogError("CreateGroup", ex);
                throw;
            }
        }

        /// <summary>
        /// ویرایش گروه
        /// </summary>
        public bool UpdateGroup(UserGroup group, int modifiedBy)
        {
            try
            {
                // Validation
                if (group == null)
                    throw new ArgumentNullException("group");

                if (group.GroupId <= 0)
                    throw new ArgumentException("شناسه گروه نامعتبر است");

                if (string.IsNullOrWhiteSpace(group.GroupName))
                    throw new ArgumentException("نام گروه نمی‌تواند خالی باشد");

                // بررسی وجود گروه
                UserGroup existingGroup = _groupRepo.GetGroupById(group.GroupId);
                if (existingGroup == null)
                    throw new InvalidOperationException("گروه یافت نشد");

                // بررسی تکراری (به جز خودش)
                if (IsGroupNameExists(group.GroupName, group.GroupId))
                    throw new InvalidOperationException("این نام گروه قبلاً ثبت شده است");

                // تنظیم
                group.ModifiedBy = modifiedBy;
                group.ModifiedDate = DateTime.Now;

                // ذخیره
                bool updated = _groupRepo.UpdateGroup(group);

                if (updated)
                {
                    LogAudit(modifiedBy, "Update", "UserGroups", group.GroupId, existingGroup.GroupName, group.GroupName);
                }

                return updated;
            }
            catch (Exception ex)
            {
                LogError("UpdateGroup", ex);
                throw;
            }
        }

        /// <summary>
        /// حذف گروه (Soft Delete)
        /// </summary>
        public bool DeleteGroup(int groupId, int deletedBy)
        {
            try
            {
                if (groupId <= 0)
                    throw new ArgumentException("شناسه گروه نامعتبر است");

                // بررسی وجود گروه
                UserGroup group = _groupRepo.GetGroupById(groupId);
                if (group == null)
                    throw new InvalidOperationException("گروه یافت نشد");

                // بررسی اینکه آیا گروه عضو دارد
                List<User> members = _groupRepo.GetGroupMembers(groupId);
                if (members.Count > 0)
                {
                    throw new InvalidOperationException("گروه دارای " + members.Count + " عضو است. ابتدا اعضا را حذف کنید");
                }

                // حذف (این متد باید در Repository اضافه شود)
                // فعلاً می‌توانید IsActive را false کنید
                group.IsActive = false;
                group.ModifiedBy = deletedBy;
                bool updated = _groupRepo.UpdateGroup(group);

                if (updated)
                {
                    auditLogMessage = "حذف گروه: " + group.GroupName;
                    LogAudit(deletedBy, "Delete", "UserGroups", groupId, auditLogMessage, null);
                }

                return updated;
            }
            catch (Exception ex)
            {
                LogError("DeleteGroup", ex);
                throw;
            }
        }

        #endregion

        #region مدیریت اعضای گروه

        /// <summary>
        /// افزودن کاربر به گروه
        /// </summary>
        public bool AddUserToGroup(int userId, int groupId, int assignedBy)
        {
            try
            {
                // Validation
                if (userId <= 0)
                    throw new ArgumentException("شناسه کاربر نامعتبر است");

                if (groupId <= 0)
                    throw new ArgumentException("شناسه گروه نامعتبر است");

                // بررسی وجود کاربر
                var userRepo = new UserRepository();
                if (userRepo.GetUserById(userId) == null)
                    throw new InvalidOperationException("کاربر یافت نشد");

                // بررسی وجود گروه
                if (_groupRepo.GetGroupById(groupId) == null)
                    throw new InvalidOperationException("گروه یافت نشد");

                // بررسی عضویت قبلی
                if (_groupRepo.IsUserInGroup(userId, groupId))
                    throw new InvalidOperationException("کاربر قبلاً عضو این گروه است");

                // افزودن
                bool added = _groupRepo.AddUserToGroup(userId, groupId, assignedBy);

                if (added)
                {
                    auditLogMessage = "افزودن کاربر " + userId + " به گروه " + groupId;
                    LogAudit(assignedBy, "Insert", "UserGroupMembers", userId, null, auditLogMessage);
                }

                return added;
            }
            catch (Exception ex)
            {
                LogError("AddUserToGroup", ex);
                throw;
            }
        }

        /// <summary>
        /// حذف کاربر از گروه
        /// </summary>
        public bool RemoveUserFromGroup(int userId, int groupId, int removedBy)
        {
            try
            {
                // Validation
                if (userId <= 0)
                    throw new ArgumentException("شناسه کاربر نامعتبر است");

                if (groupId <= 0)
                    throw new ArgumentException("شناسه گروه نامعتبر است");

                // بررسی عضویت
                if (!_groupRepo.IsUserInGroup(userId, groupId))
                    throw new InvalidOperationException("کاربر عضو این گروه نیست");

                // حذف
                bool removed = _groupRepo.RemoveUserFromGroup(userId, groupId);

                if (removed)
                {
                    auditLogMessage = "حذف کاربر " + userId + " از گروه " + groupId;
                    LogAudit(removedBy, "Delete", "UserGroupMembers", userId, auditLogMessage, null);
                }

                return removed;
            }
            catch (Exception ex)
            {
                LogError("RemoveUserFromGroup", ex);
                throw;
            }
        }

        /// <summary>
        /// افزودن چندین کاربر به گروه به صورت دسته‌جمعی
        /// </summary>
        public bool AddMultipleUsersToGroup(List<int> userIds, int groupId, int assignedBy)
        {
            try
            {
                if (userIds == null || userIds.Count == 0)
                    throw new ArgumentException("لیست کاربران نمی‌تواند خالی باشد");

                if (groupId <= 0)
                    throw new ArgumentException("شناسه گروه نامعتبر است");

                bool allSuccess = true;
                int successCount = 0;

                foreach (int userId in userIds)
                {
                    try
                    {
                        if (AddUserToGroup(userId, groupId, assignedBy))
                            successCount++;
                    }
                    catch
                    {
                        allSuccess = false;
                    }
                }
                auditLogMessage = "افزودن دسته‌جمعی " + successCount + " کاربر به گروه";
                LogAudit(assignedBy, "Insert", "UserGroupMembers", groupId, null, auditLogMessage);

                return allSuccess;
            }
            catch (Exception ex)
            {
                LogError("AddMultipleUsersToGroup", ex);
                throw;
            }
        }

        #endregion

        #region تخصیص آبشاری نقش به اعضای گروه

        /// <summary>
        /// تخصیص یک نقش به تمام اعضای گروه (آبشاری)
        /// </summary>
        public bool AssignRoleToGroupMembers(int groupId, int roleId, int assignedBy)
        {
            try
            {
                // Validation
                if (groupId <= 0)
                    throw new ArgumentException("شناسه گروه نامعتبر است");

                if (roleId <= 0)
                    throw new ArgumentException("شناسه نقش نامعتبر است");

                // دریافت اعضای گروه
                List<User> members = _groupRepo.GetGroupMembers(groupId);

                if (members.Count == 0)
                    throw new InvalidOperationException("گروه عضوی ندارد");

                // بررسی وجود نقش
                var roleRepo = new RoleRepository();
                if (roleRepo.GetRoleById(roleId) == null)
                    throw new InvalidOperationException("نقش یافت نشد");

                int successCount = 0;

                // تخصیص به هر عضو
                foreach (User member in members)
                {
                    try
                    {
                        // فقط اگر قبلاً نداشته باشه
                        if (!_userRoleRepo.UserHasRole(member.UserId, roleId))
                        {
                            if (_userRoleRepo.AssignRoleToUser(member.UserId, roleId, assignedBy))
                            {
                                successCount++;
                                // پاک کردن Cache دسترسی‌های کاربر
                                _authManager.ClearUserPermissionsCache(member.UserId);
                            }
                        }
                    }
                    catch
                    {
                        // ادامه بده حتی اگر یکی خطا داد
                    }
                }
                auditLogMessage = "تخصیص آبشاری نقش " + roleId + " به " + successCount + " عضو گروه " + groupId; ;
                LogAudit(assignedBy, "Insert", "UserRoles", groupId, null, auditLogMessage);

                return successCount > 0;
            }
            catch (Exception ex)
            {
                LogError("AssignRoleToGroupMembers", ex);
                throw;
            }
        }

        /// <summary>
        /// حذف یک نقش از تمام اعضای گروه (آبشاری)
        /// </summary>
        public bool RemoveRoleFromGroupMembers(int groupId, int roleId, int removedBy)
        {
            try
            {
                // Validation
                if (groupId <= 0)
                    throw new ArgumentException("شناسه گروه نامعتبر است");

                if (roleId <= 0)
                    throw new ArgumentException("شناسه نقش نامعتبر است");

                // دریافت اعضای گروه
                List<User> members = _groupRepo.GetGroupMembers(groupId);

                if (members.Count == 0)
                    throw new InvalidOperationException("گروه عضوی ندارد");

                int successCount = 0;

                // حذف از هر عضو
                foreach (User member in members)
                {
                    try
                    {
                        if (_userRoleRepo.UserHasRole(member.UserId, roleId))
                        {
                            if (_userRoleRepo.RemoveRoleFromUser(member.UserId, roleId))
                            {
                                successCount++;
                                // پاک کردن Cache
                                _authManager.ClearUserPermissionsCache(member.UserId);
                            }
                        }
                    }
                    catch
                    {
                        // ادامه بده
                    }
                }
                auditLogMessage = "حذف آبشاری نقش " + roleId + " از " + successCount + " عضو گروه " + groupId;
                LogAudit(removedBy, "Delete", "UserRoles", groupId, auditLogMessage, null);

                return successCount > 0;
            }
            catch (Exception ex)
            {
                LogError("RemoveRoleFromGroupMembers", ex);
                throw;
            }
        }

        #endregion

        #region دریافت اطلاعات

        /// <summary>
        /// دریافت گروه بر اساس شناسه
        /// </summary>
        public UserGroup GetGroupById(int groupId)
        {
            try
            {
                if (groupId <= 0)
                    throw new ArgumentException("شناسه گروه نامعتبر است");

                return _groupRepo.GetGroupById(groupId);
            }
            catch (Exception ex)
            {
                LogError("GetGroupById", ex);
                throw;
            }
        }

        /// <summary>
        /// دریافت تمام گروه‌ها
        /// </summary>
        public List<UserGroup> GetAllGroups(bool includeInactive = false)
        {
            try
            {
                return _groupRepo.GetAllGroups(includeInactive);
            }
            catch (Exception ex)
            {
                LogError("GetAllGroups", ex);
                throw;
            }
        }

        /// <summary>
        /// دریافت اعضای یک گروه
        /// </summary>
        public List<User> GetGroupMembers(int groupId)
        {
            try
            {
                if (groupId <= 0)
                    throw new ArgumentException("شناسه گروه نامعتبر است");

                return _groupRepo.GetGroupMembers(groupId);
            }
            catch (Exception ex)
            {
                LogError("GetGroupMembers", ex);
                throw;
            }
        }

        /// <summary>
        /// دریافت گروه‌های یک کاربر
        /// </summary>
        public List<UserGroup> GetUserGroups(int userId)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentException("شناسه کاربر نامعتبر است");

                // این متد باید در Repository اضافه شود
                // فعلاً می‌توانید یک Query مشابه بنویسید

                // TODO: باید متد GetUserGroups به UserGroupRepository اضافه شود
                return new List<UserGroup>();
            }
            catch (Exception ex)
            {
                LogError("GetUserGroups", ex);
                throw;
            }
        }

        #endregion

        #region متدهای کمکی

        /// <summary>
        /// بررسی تکراری بودن نام گروه
        /// </summary>
        private bool IsGroupNameExists(string groupName, int? excludeGroupId)
        {
            try
            {
                List<UserGroup> groups = _groupRepo.GetAllGroups(true);

                foreach (UserGroup group in groups)
                {
                    if (group.GroupName == groupName)
                    {
                        if (excludeGroupId.HasValue && group.GroupId == excludeGroupId.Value)
                            continue;

                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
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
            auditLogMessage = string.Format("[ERROR in {0}]: {1}", methodName, ex.Message);
            System.Diagnostics.Debug.WriteLine(auditLogMessage);
        }

        #endregion
    }
}