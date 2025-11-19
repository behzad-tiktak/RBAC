using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using RBACSystem.Models.RBAC;

namespace RBACSystem.DataAccess.RBAC
{
    // ====================================================================================================
    // 6. UserGroupRepository - مخزن گروه‌های کاربری
    // ====================================================================================================

    /// <summary>
    /// مخزن عملیات مربوط به گروه‌های کاربری
    /// </summary>
    public class UserGroupRepository
    {
        #region دریافت گروه

        /// <summary>
        /// دریافت گروه بر اساس شناسه
        /// </summary>
        public UserGroup GetGroupById(int groupId)
        {
            string query = @"
                SELECT GroupId, GroupName, Description, 
                       IsActive, IsDeleted, CreatedBy, CreatedDate,
                       ModifiedBy, ModifiedDate,
                       (SELECT COUNT(*) FROM UserGroupMembers WHERE GroupId = @GroupId AND IsDeleted = 0) AS MemberCount
                FROM UserGroups
                WHERE GroupId = @GroupId AND IsDeleted = 0";

            var parameters = new Dictionary<string, object>
            {
                { "@GroupId", groupId }
            };

            using (var select = new JCGSQLSelect(query, parameters, false, true))
            {
                if (select.Status())
                {
                    var reader = select.GetDataReader();
                    var group = new UserGroup
                    {
                        GroupId = reader.GetInt32(0),
                        GroupName = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        IsActive = reader.GetBoolean(3),
                        IsDeleted = reader.GetBoolean(4),
                        CreatedBy = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                        CreatedDate = reader.GetDateTime(6),
                        ModifiedBy = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7),
                        ModifiedDate = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8),
                        MemberCount = reader.GetInt32(9)
                    };
                    select.CloseConnection();
                    return group;
                }
            }

            return null;
        }

        /// <summary>
        /// دریافت تمام گروه‌ها
        /// </summary>
        public List<UserGroup> GetAllGroups(bool includeInactive = false)
        {
            string query = @"
                SELECT GroupId, GroupName, Description, 
                       IsActive, IsDeleted, CreatedBy, CreatedDate,
                       ModifiedBy, ModifiedDate,
                       (SELECT COUNT(*) FROM UserGroupMembers WHERE GroupId = g.GroupId AND IsDeleted = 0) AS MemberCount
                FROM UserGroups g
                WHERE IsDeleted = 0";

            if (!includeInactive)
                query += " AND IsActive = 1";

            query += " ORDER BY GroupName";

            using (var select = new JCGSQLSelect(query, true, false))
            {
                if (select.Status())
                {
                    var dt = select.GetDataTable();
                    var groups = new List<UserGroup>();

                    foreach (DataRow row in dt.Rows)
                    {
                        groups.Add(new UserGroup
                        {
                            GroupId = Convert.ToInt32(row["GroupId"]),
                            GroupName = row["GroupName"].ToString(),
                            Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : null,
                            IsActive = Convert.ToBoolean(row["IsActive"]),
                            MemberCount = Convert.ToInt32(row["MemberCount"])
                        });
                    }

                    return groups;
                }
            }

            return new List<UserGroup>();
        }

        #endregion

        #region ثبت و ویرایش

        /// <summary>
        /// ثبت گروه جدید
        /// </summary>
        public int InsertGroup(UserGroup group)
        {
            string query = @"
                INSERT INTO UserGroups (GroupName, Description, IsActive, CreatedBy, CreatedDate)
                VALUES (@GroupName, @Description, @IsActive, @CreatedBy, @CreatedDate);
                SELECT SCOPE_IDENTITY();";

            var parameters = new Dictionary<string, object>
            {
                { "@GroupName", group.GroupName },
                { "@Description", group.Description ?? (object)DBNull.Value },
                { "@IsActive", group.IsActive },
                { "@CreatedBy", group.CreatedBy ?? (object)DBNull.Value },
                { "@CreatedDate", group.CreatedDate }
            };

            var insert = new JCGSQLInsert(query, parameters, true);

            if (insert.Status())
            {
                //int.TryParse(insert.ReturnValue, out int newGroupId);
                //return newGroupId;
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
        /// ویرایش گروه
        /// </summary>
        public bool UpdateGroup(UserGroup group)
        {
            string query = @"
                UPDATE UserGroups 
                SET GroupName = @GroupName,
                    Description = @Description,
                    IsActive = @IsActive,
                    ModifiedBy = @ModifiedBy,
                    ModifiedDate = @ModifiedDate
                WHERE GroupId = @GroupId AND IsDeleted = 0";

            var parameters = new Dictionary<string, object>
            {
                { "@GroupId", group.GroupId },
                { "@GroupName", group.GroupName },
                { "@Description", group.Description ?? (object)DBNull.Value },
                { "@IsActive", group.IsActive },
                { "@ModifiedBy", group.ModifiedBy ?? (object)DBNull.Value },
                { "@ModifiedDate", DateTime.Now }
            };

            var update = new JCGSQLUpdate(query, parameters, false);
            return update.Status();
        }

        #endregion

        #region مدیریت اعضا

        /// <summary>
        /// افزودن کاربر به گروه
        /// </summary>
        public bool AddUserToGroup(int userId, int groupId, int assignedBy)
        {
            // چک کنیم قبلاً عضو نباشه
            if (IsUserInGroup(userId, groupId))
                return false;

            string query = @"
                INSERT INTO UserGroupMembers (UserId, GroupId, AssignedBy, AssignedDate)
                VALUES (@UserId, @GroupId, @AssignedBy, @AssignedDate)";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@GroupId", groupId },
                { "@AssignedBy", assignedBy },
                { "@AssignedDate", DateTime.Now }
            };

            var insert = new JCGSQLInsert(query, parameters, false);
            return insert.Status();
        }

        /// <summary>
        /// حذف کاربر از گروه
        /// </summary>
        public bool RemoveUserFromGroup(int userId, int groupId)
        {
            string query = @"
                UPDATE UserGroupMembers 
                SET IsDeleted = 1
                WHERE UserId = @UserId AND GroupId = @GroupId";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@GroupId", groupId }
            };

            var update = new JCGSQLUpdate(query, parameters, false);
            return update.Status();
        }

        /// <summary>
        /// بررسی عضویت کاربر در گروه
        /// </summary>
        public bool IsUserInGroup(int userId, int groupId)
        {
            string query = @"
                SELECT COUNT(*) 
                FROM UserGroupMembers 
                WHERE UserId = @UserId 
                  AND GroupId = @GroupId 
                  AND IsDeleted = 0";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@GroupId", groupId }
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

        /// <summary>
        /// دریافت اعضای یک گروه
        /// </summary>
        public List<User> GetGroupMembers(int groupId)
        {
            string query = @"
                SELECT u.UserId, u.Username, u.Email, u.FullName, 
                       u.IsActive, ugm.AssignedDate
                FROM UserGroupMembers ugm
                INNER JOIN Users u ON ugm.UserId = u.UserId
                WHERE ugm.GroupId = @GroupId 
                  AND ugm.IsDeleted = 0 
                  AND u.IsDeleted = 0
                ORDER BY u.FullName";

            var parameters = new Dictionary<string, object>
            {
                { "@GroupId", groupId }
            };

            using (var select = new JCGSQLSelect(query, parameters, true, false))
            {
                if (select.Status())
                {
                    var dt = select.GetDataTable();
                    var users = new List<User>();

                    foreach (DataRow row in dt.Rows)
                    {
                        users.Add(new User
                        {
                            UserId = Convert.ToInt32(row["UserId"]),
                            Username = row["Username"].ToString(),
                            Email = row["Email"].ToString(),
                            FullName = row["FullName"].ToString(),
                            IsActive = Convert.ToBoolean(row["IsActive"])
                        });
                    }

                    return users;
                }
            }

            return new List<User>();
        }

        #endregion
    }
}