using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using RBACSystem.Models.RBAC;

namespace RBACSystem.DataAccess.RBAC
{
    public class UserRepository
    {
        #region دریافت کاربر

        public User GetUserById(int userId)
        {
            string query = @"
                SELECT UserId, Username, PasswordHash, Email, FullName, 
                       IsActive, IsDeleted, CreatedBy, CreatedDate, 
                       ModifiedBy, ModifiedDate, LastLoginDate
                FROM Users
                WHERE UserId = @UserId AND IsDeleted = 0";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", userId }
            };

            using (var select = new JCGSQLSelect(query, parameters, false, true))
            {
                if (select.Status())
                {
                    var reader = select.GetDataReader();
                    var user = MapReaderToUser(reader);
                    select.CloseConnection();
                    return user;
                }
            }

            return null;
        }

        public User GetUserByUsername(string username)
        {
            string query = @"
                SELECT UserId, Username, PasswordHash, Email, FullName, 
                       IsActive, IsDeleted, CreatedBy, CreatedDate, 
                       ModifiedBy, ModifiedDate, LastLoginDate
                FROM Users
                WHERE Username = @Username AND IsDeleted = 0";

            var parameters = new Dictionary<string, object>
            {
                { "@Username", username }
            };

            using (var select = new JCGSQLSelect(query, parameters, false, true))
            {
                if (select.Status())
                {
                    var reader = select.GetDataReader();
                    var user = MapReaderToUser(reader);
                    select.CloseConnection();
                    return user;
                }
            }

            return null;
        }

        public User GetUserByEmail(string email)
        {
            string query = @"
                SELECT UserId, Username, PasswordHash, Email, FullName, 
                       IsActive, IsDeleted, CreatedBy, CreatedDate, 
                       ModifiedBy, ModifiedDate, LastLoginDate
                FROM Users
                WHERE Email = @Email AND IsDeleted = 0";

            var parameters = new Dictionary<string, object>
            {
                { "@Email", email }
            };

            using (var select = new JCGSQLSelect(query, parameters, false, true))
            {
                if (select.Status())
                {
                    var reader = select.GetDataReader();
                    var user = MapReaderToUser(reader);
                    select.CloseConnection();
                    return user;
                }
            }

            return null;
        }

        public List<User> GetAllUsers(bool includeInactive)
        {
            string query = @"
                SELECT UserId, Username, PasswordHash, Email, FullName, 
                       IsActive, IsDeleted, CreatedBy, CreatedDate, 
                       ModifiedBy, ModifiedDate, LastLoginDate
                FROM Users
                WHERE IsDeleted = 0";

            if (!includeInactive)
                query += " AND IsActive = 1";

            query += " ORDER BY FullName";

            using (var select = new JCGSQLSelect(query, true, false))
            {
                if (select.Status())
                {
                    var dt = select.GetDataTable();
                    return MapDataTableToUsers(dt);
                }
            }

            return new List<User>();
        }
        

        #endregion

        #region ثبت و ویرایش

        public int InsertUser(User user)
        {
            string query = @"
                INSERT INTO Users (Username, PasswordHash, Email, FullName, 
                                   IsActive, CreatedBy, CreatedDate)
                VALUES (@Username, @PasswordHash, @Email, @FullName, 
                        @IsActive, @CreatedBy, @CreatedDate);
                SELECT SCOPE_IDENTITY();";

            var parameters = new Dictionary<string, object>
            {
                { "@Username", user.Username },
                { "@PasswordHash", user.PasswordHash },
                { "@Email", user.Email },
                { "@FullName", user.FullName },
                { "@IsActive", user.IsActive },
                { "@CreatedBy", user.CreatedBy ?? (object)DBNull.Value },
                { "@CreatedDate", user.CreatedDate }
            };

            var insert = new JCGSQLInsert(query, parameters, true);

            if (insert.Status())
            {
                // ✅ نسخه سازگار
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

        public bool UpdateUser(User user)
        {
            string query = @"
                UPDATE Users 
                SET Username = @Username,
                    Email = @Email,
                    FullName = @FullName,
                    IsActive = @IsActive,
                    ModifiedBy = @ModifiedBy,
                    ModifiedDate = @ModifiedDate
                WHERE UserId = @UserId AND IsDeleted = 0";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", user.UserId },
                { "@Username", user.Username },
                { "@Email", user.Email },
                { "@FullName", user.FullName },
                { "@IsActive", user.IsActive },
                { "@ModifiedBy", user.ModifiedBy ?? (object)DBNull.Value },
                { "@ModifiedDate", DateTime.Now }
            };

            var update = new JCGSQLUpdate(query, parameters, false);
            return update.Status();
        }

        public bool UpdatePassword(int userId, string newPasswordHash, int modifiedBy)
        {
            string query = @"
                UPDATE Users 
                SET PasswordHash = @PasswordHash,
                    ModifiedBy = @ModifiedBy,
                    ModifiedDate = @ModifiedDate
                WHERE UserId = @UserId AND IsDeleted = 0";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@PasswordHash", newPasswordHash },
                { "@ModifiedBy", modifiedBy },
                { "@ModifiedDate", DateTime.Now }
            };

            var update = new JCGSQLUpdate(query, parameters, false);
            return update.Status();
        }

        public bool UpdateUserStatus(int userId, bool isActive, int modifiedBy)
        {
            string query = @"
                UPDATE Users 
                SET IsActive = @IsActive,
                    ModifiedBy = @ModifiedBy,
                    ModifiedDate = @ModifiedDate
                WHERE UserId = @UserId AND IsDeleted = 0";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@IsActive", isActive },
                { "@ModifiedBy", modifiedBy },
                { "@ModifiedDate", DateTime.Now }
            };

            var update = new JCGSQLUpdate(query, parameters, false);
            return update.Status();
        }

        public bool UpdateLastLogin(int userId)
        {
            string query = @"
                UPDATE Users 
                SET LastLoginDate = @LastLoginDate
                WHERE UserId = @UserId";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@LastLoginDate", DateTime.Now }
            };

            var update = new JCGSQLUpdate(query, parameters, false);
            return update.Status();
        }

        #endregion

        #region حذف

        public bool DeleteUser(int userId, int deletedBy)
        {
            string query = @"
                UPDATE Users 
                SET IsDeleted = 1,
                    IsActive = 0,
                    ModifiedBy = @ModifiedBy,
                    ModifiedDate = @ModifiedDate
                WHERE UserId = @UserId";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@ModifiedBy", deletedBy },
                { "@ModifiedDate", DateTime.Now }
            };

            var update = new JCGSQLUpdate(query, parameters, false);
            return update.Status();
        }

        #endregion

        #region بررسی وجود

        public bool UsernameExists(string username, int? excludeUserId)
        {
            string query = @"
                SELECT COUNT(*) 
                FROM Users 
                WHERE Username = @Username AND IsDeleted = 0";

            var parameters = new Dictionary<string, object>
            {
                { "@Username", username }
            };

            if (excludeUserId.HasValue)
            {
                query += " AND UserId != @UserId";
                parameters.Add("@UserId", excludeUserId.Value);
            }

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

        public bool EmailExists(string email, int? excludeUserId)
        {
            string query = @"
                SELECT COUNT(*) 
                FROM Users 
                WHERE Email = @Email AND IsDeleted = 0";

            var parameters = new Dictionary<string, object>
            {
                { "@Email", email }
            };

            if (excludeUserId.HasValue)
            {
                query += " AND UserId != @UserId";
                parameters.Add("@UserId", excludeUserId.Value);
            }

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

        #region متدهای کمکی

        private User MapReaderToUser(SqlDataReader reader)
        {
            return new User
            {
                UserId = reader.GetInt32(0),
                Username = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                Email = reader.GetString(3),
                FullName = reader.GetString(4),
                IsActive = reader.GetBoolean(5),
                IsDeleted = reader.GetBoolean(6),
                CreatedBy = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7),
                CreatedDate = reader.GetDateTime(8),
                ModifiedBy = reader.IsDBNull(9) ? (int?)null : reader.GetInt32(9),
                ModifiedDate = reader.IsDBNull(10) ? (DateTime?)null : reader.GetDateTime(10),
                LastLoginDate = reader.IsDBNull(11) ? (DateTime?)null : reader.GetDateTime(11)
            };
        }

        private List<User> MapDataTableToUsers(DataTable dt)
        {
            var users = new List<User>();

            foreach (DataRow row in dt.Rows)
            {
                users.Add(new User
                {
                    UserId = Convert.ToInt32(row["UserId"]),
                    Username = row["Username"].ToString(),
                    PasswordHash = row["PasswordHash"].ToString(),
                    Email = row["Email"].ToString(),
                    FullName = row["FullName"].ToString(),
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    IsDeleted = Convert.ToBoolean(row["IsDeleted"]),
                    CreatedBy = row["CreatedBy"] != DBNull.Value ? Convert.ToInt32(row["CreatedBy"]) : (int?)null,
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    ModifiedBy = row["ModifiedBy"] != DBNull.Value ? Convert.ToInt32(row["ModifiedBy"]) : (int?)null,
                    ModifiedDate = row["ModifiedDate"] != DBNull.Value ? Convert.ToDateTime(row["ModifiedDate"]) : (DateTime?)null,
                    LastLoginDate = row["LastLoginDate"] != DBNull.Value ? Convert.ToDateTime(row["LastLoginDate"]) : (DateTime?)null
                });
            }

            return users;
        }

        #endregion
    }
}