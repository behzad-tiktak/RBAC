// ====================================================================================================
// SecurityHelper - کلاس کمکی امنیتی
// نسخه سازگار با .NET Framework 4.8.1 / C# 5.0
// مسیر: App_Code/Utilities/SecurityHelper.cs
// ====================================================================================================

using System;
using System.Linq;
using System.Web;
using RBACSystem.Models.RBAC;
using RBACSystem.DataAccess.RBAC;

namespace RBACSystem.Utilities
{
    /// <summary>
    /// کلاس کمکی برای عملیات امنیتی
    /// </summary>
    public static class SecurityHelper
    {
        #region Hash کردن رمز عبور

        /// <summary>
        /// Hash کردن رمز عبور با SHA256
        /// </summary>
        /// <param name="password">رمز عبور Plain Text</param>
        /// <returns>رمز Hash شده</returns>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("رمز عبور نمی‌تواند خالی باشد");

            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                // تبدیل به رشته Hex
                var builder = new System.Text.StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }

        /// <summary>
        /// تولید رمز عبور تصادفی
        /// </summary>
        /// <param name="length">طول رمز عبور (پیش‌فرض 8)</param>
        /// <returns>رمز عبور تصادفی</returns>
        public static string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            var result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }

            return new string(result);
        }

        #endregion

        #region مدیریت Session

        /// <summary>
        /// دریافت کاربر فعلی از Session
        /// </summary>
        /// <returns>شیء User یا null</returns>
        public static User GetCurrentUser()
        {
            try
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    return HttpContext.Current.Session["CurrentUser"] as User;
                }
            }
            catch
            {
                // در صورت خطا null برگردون
            }

            return null;
        }

        /// <summary>
        /// دریافت شناسه کاربر فعلی
        /// </summary>
        /// <returns>شناسه کاربر یا 0</returns>
        public static int GetCurrentUserId()
        {
            var user = GetCurrentUser();

            // ✅ نسخه سازگار (بدون ?.)
            if (user != null)
                return user.UserId;
            else
                return 0;
        }

        /// <summary>
        /// دریافت نام کاربری فعلی
        /// </summary>
        /// <returns>نام کاربری یا رشته خالی</returns>
        public static string GetCurrentUsername()
        {
            var user = GetCurrentUser();

            if (user != null)
                return user.Username;
            else
                return string.Empty;
        }

        /// <summary>
        /// بررسی اینکه آیا کاربر لاگین کرده
        /// </summary>
        /// <returns>true/false</returns>
        public static bool IsAuthenticated()
        {
            return GetCurrentUser() != null;
        }

        /// <summary>
        /// ذخیره اطلاعات کاربر در Session
        /// </summary>
        /// <param name="user">شیء کاربر</param>
        public static void SetCurrentUser(User user)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session["CurrentUser"] = user;
                HttpContext.Current.Session["UserId"] = user.UserId;
                HttpContext.Current.Session["Username"] = user.Username;
                HttpContext.Current.Session.Timeout = 60; // 60 دقیقه
            }
        }

        /// <summary>
        /// خروج از سیستم (پاک کردن Session)
        /// </summary>
        public static void Logout()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                int userId = GetCurrentUserId();

                // ثبت لاگ خروج
                if (userId > 0)
                {
                    try
                    {
                        var auditRepo = new AuditRepository();
                        auditRepo.InsertLog(new AuditLog
                        {
                            UserId = userId,
                            Action = "Logout",
                            TableName = "Users",
                            RecordId = userId,
                            NewValue = "خروج از سیستم",
                            IpAddress = GetUserIP(),
                            ActionDate = DateTime.Now
                        });
                    }
                    catch
                    {
                        // خطای لاگ نباید مانع Logout بشه
                    }
                }

                // پاک کردن Session
                HttpContext.Current.Session.Clear();
                HttpContext.Current.Session.Abandon();
            }
        }

        #endregion

        #region اطلاعات درخواست HTTP

        /// <summary>
        /// دریافت IP کاربر
        /// </summary>
        /// <returns>آدرس IP</returns>
        public static string GetUserIP()
        {
            try
            {
                if (HttpContext.Current != null && HttpContext.Current.Request != null)
                {
                    string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                    if (string.IsNullOrEmpty(ip))
                        ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    if (string.IsNullOrEmpty(ip))
                        ip = HttpContext.Current.Request.UserHostAddress;

                    return ip;
                }
            }
            catch
            {
                // در صورت خطا
            }

            return "Unknown";
        }

        /// <summary>
        /// دریافت User Agent (مرورگر)
        /// </summary>
        /// <returns>User Agent</returns>
        public static string GetUserAgent()
        {
            try
            {
                if (HttpContext.Current != null && HttpContext.Current.Request != null)
                {
                    return HttpContext.Current.Request.UserAgent;
                }
            }
            catch
            {
                // در صورت خطا
            }

            return "Unknown";
        }

        /// <summary>
        /// دریافت URL فعلی
        /// </summary>
        /// <returns>URL صفحه</returns>
        public static string GetCurrentUrl()
        {
            try
            {
                if (HttpContext.Current != null && HttpContext.Current.Request != null)
                {
                    return HttpContext.Current.Request.Url.ToString();
                }
            }
            catch
            {
                // در صورت خطا
            }

            return string.Empty;
        }

        #endregion

        #region Validation

        /// <summary>
        /// اعتبارسنجی فرمت ایمیل
        /// </summary>
        /// <param name="email">آدرس ایمیل</param>
        /// <returns>معتبر/نامعتبر</returns>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

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
        /// بررسی قدرت رمز عبور
        /// </summary>
        /// <param name="password">رمز عبور</param>
        /// <returns>ضعیف، متوسط، قوی</returns>
        public static string CheckPasswordStrength(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return "خالی";

            int score = 0;

            // طول
            if (password.Length >= 8) score++;
            if (password.Length >= 12) score++;

            // حروف بزرگ
            if (password.Any(char.IsUpper)) score++;

            // حروف کوچک
            if (password.Any(char.IsLower)) score++;

            // اعداد
            if (password.Any(char.IsDigit)) score++;

            // کاراکترهای خاص
            if (password.Any(c => !char.IsLetterOrDigit(c))) score++;

            if (score <= 2)
                return "ضعیف";
            else if (score <= 4)
                return "متوسط";
            else
                return "قوی";
        }

        #endregion
    }
}

// ====================================================================================================
// نکات مهم:
// ====================================================================================================
//
// ✅ سازگار با .NET Framework 4.8.1
// ✅ سازگار با C# 5.0
// ✅ بدون استفاده از:
//    - nameof (C# 6.0)
//    - String Interpolation $ (C# 6.0)
//    - Null-Conditional ?. (C# 6.0)
//    - Expression-bodied members (C# 6.0)
//
// ====================================================================================================