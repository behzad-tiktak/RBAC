using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RBACSystem.Models.RBAC;
using RBACSystem.DataAccess.RBAC;
using RBACSystem.Utilities;

namespace RBACSystem.BusinessLogic.RBAC
{
    // ====================================================================================================
    // 1. UserManager - مدیریت کاربران
    // ====================================================================================================

    /// <summary>
    /// مدیریت عملیات مربوط به کاربران
    /// شامل: Validation، Business Logic، Authorization
    /// </summary>
    public class UserManager
    {
        private readonly UserRepository _userRepo;
        private readonly AuditRepository _auditRepo;

        public UserManager()
        {
            _userRepo = new UserRepository();
            _auditRepo = new AuditRepository();
        }

        #region ایجاد کاربر

        /// <summary>
        /// ایجاد کاربر جدید
        /// </summary>
        /// <param name="user">اطلاعات کاربر</param>
        /// <param name="password">رمز عبور (Plain Text)</param>
        /// <param name="createdBy">شناسه کاربر ایجادکننده</param>
        /// <returns>شناسه کاربر جدید یا 0 در صورت خطا</returns>
        public int CreateUser(User user, string password, int createdBy)
        {
            try
            {
                // ──────────────────────────────────────
                // 1. Validation ورودی‌ها
                // ──────────────────────────────────────
                
                if (user == null)
                    //throw new ArgumentNullException(nameof(user), "اطلاعات کاربر نمی‌تواند خالی باشد");
                    throw new ArgumentNullException("user", "اطلاعات کاربر نمی‌تواند خالی باشد");

                if (string.IsNullOrWhiteSpace(user.Username))
                    throw new ArgumentException("نام کاربری نمی‌تواند خالی باشد");

                if (string.IsNullOrWhiteSpace(password))
                    throw new ArgumentException("رمز عبور نمی‌تواند خالی باشد");

                if (string.IsNullOrWhiteSpace(user.Email))
                    throw new ArgumentException("ایمیل نمی‌تواند خالی باشد");

                // ──────────────────────────────────────
                // 2. Validation قوانین کسب‌وکار
                // ──────────────────────────────────────

                // طول نام کاربری
                if (user.Username.Length < 3 || user.Username.Length > 50)
                    throw new ArgumentException("نام کاربری باید بین 3 تا 50 کاراکتر باشد");

                // طول رمز عبور
                if (password.Length < 6)
                    throw new ArgumentException("رمز عبور باید حداقل 6 کاراکتر باشد");

                // فرمت ایمیل
                if (!IsValidEmail(user.Email))
                    throw new ArgumentException("فرمت ایمیل معتبر نیست");

                // ──────────────────────────────────────
                // 3. بررسی تکراری
                // ──────────────────────────────────────

                if (_userRepo.UsernameExists(user.Username,null))
                    throw new InvalidOperationException("این نام کاربری قبلاً ثبت شده است");

                if (_userRepo.EmailExists(user.Email,user.UserId))
                    throw new InvalidOperationException("این ایمیل قبلاً ثبت شده است");

                // ──────────────────────────────────────
                // 4. Hash کردن رمز عبور
                // ──────────────────────────────────────

                user.PasswordHash = SecurityHelper.HashPassword(password);

                // ──────────────────────────────────────
                // 5. تنظیم اطلاعات پایه
                // ──────────────────────────────────────

                user.CreatedBy = createdBy;
                user.CreatedDate = DateTime.Now;
                user.IsActive = true;
                user.IsDeleted = false;

                // ──────────────────────────────────────
                // 6. ذخیره در دیتابیس
                // ──────────────────────────────────────

                int newUserId = _userRepo.InsertUser(user);

                if (newUserId > 0)
                {
                    // ──────────────────────────────────────
                    // 7. ثبت لاگ
                    // ──────────────────────────────────────

                    LogAudit(createdBy, "Insert", "Users", newUserId, 
                        //null, $"ایجاد کاربر جدید: {user.Username}");
                    null, string.Format("ایجاد کاربر جدید: {0}",user.Username));

                    return newUserId;
                }

                throw new Exception("خطا در ذخیره کاربر در دیتابیس");
            }
            catch (Exception ex)
            {
                // لاگ خطا
                LogError("CreateUser", ex);
                throw; // پرتاب مجدد برای مدیریت در UI
            }
        }

        #endregion

        #region ویرایش کاربر

        /// <summary>
        /// ویرایش اطلاعات کاربر
        /// </summary>
        public bool UpdateUser(User user, int modifiedBy)
        {
            try
            {
                // Validation
                if (user == null)
                    //throw new ArgumentNullException(nameof(user));
                    throw new ArgumentNullException("user");

                if (user.UserId <= 0)
                    throw new ArgumentException("شناسه کاربر نامعتبر است");

                if (string.IsNullOrWhiteSpace(user.Username))
                    throw new ArgumentException("نام کاربری نمی‌تواند خالی باشد");

                if (string.IsNullOrWhiteSpace(user.Email))
                    throw new ArgumentException("ایمیل نمی‌تواند خالی باشد");

                // بررسی وجود کاربر
                User existingUser = _userRepo.GetUserById(user.UserId);
                if (existingUser == null)
                    throw new InvalidOperationException("کاربر یافت نشد");

                // بررسی تکراری (به جز خود کاربر)
                if (_userRepo.UsernameExists(user.Username, user.UserId))
                    throw new InvalidOperationException("این نام کاربری قبلاً ثبت شده است");

                if (_userRepo.EmailExists(user.Email, user.UserId))
                    throw new InvalidOperationException("این ایمیل قبلاً ثبت شده است");

                // تنظیم اطلاعات
                user.ModifiedBy = modifiedBy;
                user.ModifiedDate = DateTime.Now;

                // ذخیره
                bool updated = _userRepo.UpdateUser(user);

                if (updated)
                {
                    // ثبت لاگ
                    LogAudit(modifiedBy, "Update", "Users", user.UserId, 
                        SerializeUser(existingUser), SerializeUser(user));
                }

                return updated;
            }
            catch (Exception ex)
            {
                LogError("UpdateUser", ex);
                throw;
            }
        }

        /// <summary>
        /// تغییر رمز عبور کاربر
        /// </summary>
        public bool ChangePassword(int userId, string oldPassword, string newPassword, int modifiedBy)
        {
            try
            {
                // Validation
                if (userId <= 0)
                    throw new ArgumentException("شناسه کاربر نامعتبر است");

                if (string.IsNullOrWhiteSpace(oldPassword))
                    throw new ArgumentException("رمز عبور قدیم نمی‌تواند خالی باشد");

                if (string.IsNullOrWhiteSpace(newPassword))
                    throw new ArgumentException("رمز عبور جدید نمی‌تواند خالی باشد");

                if (newPassword.Length < 6)
                    throw new ArgumentException("رمز عبور جدید باید حداقل 6 کاراکتر باشد");

                // بررسی وجود کاربر
                User user = _userRepo.GetUserById(userId);
                if (user == null)
                    throw new InvalidOperationException("کاربر یافت نشد");

                // بررسی رمز عبور قدیم
                string oldPasswordHash = SecurityHelper.HashPassword(oldPassword);
                if (user.PasswordHash != oldPasswordHash)
                    throw new UnauthorizedAccessException("رمز عبور قدیم اشتباه است");

                // Hash کردن رمز جدید
                string newPasswordHash = SecurityHelper.HashPassword(newPassword);

                // ذخیره
                bool updated = _userRepo.UpdatePassword(userId, newPasswordHash, modifiedBy);

                if (updated)
                {
                    // ثبت لاگ
                    LogAudit(modifiedBy, "Update", "Users", userId, 
                        null, "تغییر رمز عبور");
                }

                return updated;
            }
            catch (Exception ex)
            {
                LogError("ChangePassword", ex);
                throw;
            }
        }

        /// <summary>
        /// فعال/غیرفعال کردن کاربر
        /// </summary>
        /// <param name="isActive">true / false
        /// <para>:بهترین روش به وضعیت غیر جاری، استفاده ازعبارت مخالف ااست</para>
        /// <para> NOT ==> ! </para>
        /// </param>
        public bool ToggleUserStatus(int userId, bool isActive, int modifiedBy)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentException("شناسه کاربر نامعتبر است");

                // بررسی وجود کاربر
                User user = _userRepo.GetUserById(userId);
                if (user == null)
                    throw new InvalidOperationException("کاربر یافت نشد");

                // نمی‌شه خودت رو غیرفعال کنی!
                if (userId == modifiedBy && !isActive)
                    throw new InvalidOperationException("نمی‌توانید خودتان را غیرفعال کنید");

                // ذخیره
                bool updated = _userRepo.UpdateUserStatus(userId, isActive, modifiedBy);

                if (updated)
                {
                    string action = isActive ? "فعال" : "غیرفعال";
                    LogAudit(modifiedBy, "Update", "Users", userId, 
                        //null, $"کاربر {action} شد");
                    null, string.Format("کاربر {0} شد",action));
                }

                return updated;
            }
            catch (Exception ex)
            {
                LogError("ToggleUserStatus", ex);
                throw;
            }
        }

        #endregion

        #region حذف کاربر

        /// <summary>
        /// حذف کاربر (Soft Delete)
        /// </summary>
        public bool DeleteUser(int userId, int deletedBy)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentException("شناسه کاربر نامعتبر است");

                // بررسی وجود کاربر
                User user = _userRepo.GetUserById(userId);
                if (user == null)
                    throw new InvalidOperationException("کاربر یافت نشد");

                // نمی‌شه خودت رو حذف کنی!
                if (userId == deletedBy)
                    throw new InvalidOperationException("نمی‌توانید خودتان را حذف کنید");

                // ذخیره
                bool deleted = _userRepo.DeleteUser(userId, deletedBy);

                if (deleted)
                {
                    LogAudit(deletedBy, "Delete", "Users", userId, 
                        SerializeUser(user), null);
                }

                return deleted;
            }
            catch (Exception ex)
            {
                LogError("DeleteUser", ex);
                throw;
            }
        }

        #endregion

        #region احراز هویت (Authentication)

        /// <summary>
        /// ورود به سیستم
        /// </summary>
        /// <param name="username">نام کاربری</param>
        /// <param name="password">رمز عبور</param>
        /// <returns>شیء کاربر یا null</returns>
        public User Login(string username, string password)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(username))
                    throw new ArgumentException("نام کاربری نمی‌تواند خالی باشد");

                if (string.IsNullOrWhiteSpace(password))
                    throw new ArgumentException("رمز عبور نمی‌تواند خالی باشد");

                // دریافت کاربر
                User user = _userRepo.GetUserByUsername(username);

                if (user == null)
                    return null; // کاربر یافت نشد

                // بررسی فعال بودن
                if (!user.IsActive)
                    throw new UnauthorizedAccessException("حساب کاربری شما غیرفعال است");

                // بررسی رمز عبور
                string passwordHash = SecurityHelper.HashPassword(password);
                if (user.PasswordHash != passwordHash)
                    return null; // رمز اشتباه

                // بروزرسانی آخرین ورود
                _userRepo.UpdateLastLogin(user.UserId);

                // ثبت لاگ
                LogAudit(user.UserId, "Login", "Users", user.UserId, 
                    null, "ورود موفق به سیستم");

                return user;
            }
            catch (Exception ex)
            {
                LogError("Login", ex);
                throw;
            }
        }

        #endregion

        #region دریافت کاربران

        /// <summary>
        /// دریافت کاربر بر اساس شناسه
        /// </summary>
        public User GetUserById(int userId)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentException("شناسه کاربر نامعتبر است");

                return _userRepo.GetUserById(userId);
            }
            catch (Exception ex)
            {
                LogError("GetUserById", ex);
                throw;
            }
        }

        /// <summary>
        /// دریافت کاربران
        /// </summary>
        /// <param name="includeInactive">        
        /// <para>true: مقدار پیشفرض - فقط کاربران فعال</para>
        /// <para> -----------------------------------------</para>
        /// <para>false: کاربران غیرفعال هم شامل شود</para>
        /// </param>
        public List<User> GetAllUsers(bool includeInactive = false)
        {
            try
            {
                return _userRepo.GetAllUsers(includeInactive);
            }
            catch (Exception ex)
            {
                LogError("GetAllUsers", ex);
                throw;
            }
        }
      
        #endregion

        #region متدهای کمکی

        /// <summary>
        /// اعتبارسنجی فرمت ایمیل
        /// </summary>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// تبدیل User به رشته JSON (برای لاگ)
        /// </summary>
        private string SerializeUser(User user)
        {
            if (user == null) return null;
            
            //return $"{{UserId:{user.UserId}, Username:'{user.Username}', Email:'{user.Email}', FullName:'{user.FullName}', IsActive:{user.IsActive}}}";
            return string.Format(
                "{{UserId:{0}, Username:'{1}', Email:'{2}', FullName:'{3}', IsActive:{4}}}",
                user.UserId,
                user.Username ?? string.Empty,
                user.Email ?? string.Empty,
                user.FullName ?? string.Empty,
                user.IsActive
                );
        }

        /// <summary>
        /// ثبت لاگ در دیتابیس
        /// </summary>
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
                    IpAddress = GetUserIP(),
                    UserAgent = GetUserAgent(),
                    ActionDate = DateTime.Now
                };

                _auditRepo.InsertLog(log);
            }
            catch (Exception ex)
            {
                // خطای لاگ نباید جلوی عملیات اصلی رو بگیره
                LogError("LogAudit", ex);
            }
        }

        /// <summary>
        /// ثبت خطا
        /// </summary>
        private void LogError(string methodName, Exception ex)
        {
            // می‌تونید به فایل یا Event Log بنویسید
            //System.Diagnostics.Debug.WriteLine($"[ERROR in {methodName}]: {ex.Message}");
            System.Diagnostics.Debug.WriteLine(string.Format("[ERROR in {0}]: {1}"), methodName, ex.Message);
        }

        /// <summary>
        /// دریافت IP کاربر
        /// </summary>
        private string GetUserIP()
        {
            try
            {
                if (HttpContext.Current != null)
                {
                    string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (string.IsNullOrEmpty(ip))
                        ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    return ip;
                }
            }
            catch { }
            
            return "Unknown";
        }

        /// <summary>
        /// دریافت User Agent
        /// </summary>
        private string GetUserAgent()
        {
            try
            {
                if (HttpContext.Current != null)
                    return HttpContext.Current.Request.UserAgent;
            }
            catch { }
            
            return "Unknown";
        }

        #endregion
    }
}