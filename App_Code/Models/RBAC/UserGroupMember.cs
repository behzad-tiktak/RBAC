using System;
using System.Collections.Generic;

namespace RBACSystem.Models.RBAC
{
    // ====================================================================================================
    // 7. UserGroupMember Model - مدل عضویت در گروه
    // ====================================================================================================

    /// <summary>
    /// مدل عضویت کاربر در گروه
    /// </summary>
    public class UserGroupMember
    {
        /// <summary>
        /// شناسه یکتای عضویت
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// شناسه گروه
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// حذف منطقی
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// شناسه کاربری که عضویت را ثبت کرده
        /// </summary>
        public int? AssignedBy { get; set; }

        /// <summary>
        /// تاریخ عضویت
        /// </summary>
        public DateTime AssignedDate { get; set; }

        // Properties اضافی

        /// <summary>
        /// نام کاربر
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// نام کامل کاربر
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// نام گروه
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// سازنده پیش‌فرض
        /// </summary>
        public UserGroupMember()
        {
            IsDeleted = false;
            AssignedDate = DateTime.Now;
        }
    }
}