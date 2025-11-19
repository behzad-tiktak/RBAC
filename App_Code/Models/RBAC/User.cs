using System;
using System.Collections.Generic;

namespace RBACSystem.Models.RBAC
{
    // ====================================================================================================
    // 1. User Model - مدل کاربر
    // ====================================================================================================

    /// <summary>
    /// مدل کاربر سیستم
    /// نمایانگر اطلاعات یک کاربر در سیستم
    /// </summary>
    public class User
    {
        /// <summary>
        /// شناسه یکتای کاربر
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// نام کاربری (یکتا)
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// رمز عبور Hash شده
        /// نکته: هرگز رمز عبور Plain Text ذخیره نشود!
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// آدرس ایمیل (یکتا)
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// نام کامل کاربر
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// وضعیت فعال/غیرفعال
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// حذف منطقی (Soft Delete)
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// شناسه کاربری که این رکورد را ایجاد کرده
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// شناسه کاربری که این رکورد را ویرایش کرده
        /// </summary>
        public int? ModifiedBy { get; set; }

        /// <summary>
        /// تاریخ آخرین ویرایش
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// تاریخ آخرین ورود به سیستم
        /// </summary>
        public DateTime? LastLoginDate { get; set; }

        // Properties اضافی برای کار راحت‌تر (از DB نمی‌آیند)

        /// <summary>
        /// لیست نقش‌های کاربر (برای استفاده در UI)
        /// </summary>
        public List<Role> Roles { get; set; }

        /// <summary>
        /// لیست دسترسی‌های کاربر (برای Cache)
        /// </summary>
        public List<string> Permissions { get; set; }

        /// <summary>
        /// سازنده پیش‌فرض
        /// </summary>
        public User()
        {
            IsActive = true;
            IsDeleted = false;
            CreatedDate = DateTime.Now;
            Roles = new List<Role>();
            Permissions = new List<string>();
        }
    }
}