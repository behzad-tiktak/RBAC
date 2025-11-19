using System;
using System.Collections.Generic;

namespace RBACSystem.Models.RBAC
{
    // ====================================================================================================
    // 5. UserRole Model - مدل رابطه کاربر و نقش
    // ====================================================================================================

    /// <summary>
    /// مدل رابطه بین کاربر و نقش
    /// نمایانگر تخصیص یک نقش به یک کاربر
    /// </summary>
    public class UserRole
    {
        /// <summary>
        /// شناسه یکتای رابطه
        /// </summary>
        public int UserRoleId { get; set; }

        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// شناسه نقش
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// حذف منطقی
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// شناسه کاربری که این نقش را تخصیص داده
        /// </summary>
        public int? AssignedBy { get; set; }

        /// <summary>
        /// تاریخ تخصیص
        /// </summary>
        public DateTime AssignedDate { get; set; }

        // Properties اضافی برای نمایش

        /// <summary>
        /// نام کاربر
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// نام نقش
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// نام کاربری که تخصیص داده
        /// </summary>
        public string AssignedByUsername { get; set; }

        /// <summary>
        /// سازنده پیش‌فرض
        /// </summary>
        public UserRole()
        {
            IsDeleted = false;
            AssignedDate = DateTime.Now;
        }
    }
}