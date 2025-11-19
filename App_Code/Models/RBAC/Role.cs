using System;
using System.Collections.Generic;

namespace RBACSystem.Models.RBAC
{
    // ====================================================================================================
    // 2. Role Model - مدل نقش
    // ====================================================================================================

    /// <summary>
    /// مدل نقش سیستم
    /// نمایانگر یک نقش (مانند Admin، Manager، User)
    /// </summary>
    public class Role
    {
        /// <summary>
        /// شناسه یکتای نقش
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// نام نقش (یکتا) - مثلاً: Admin, Manager
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// توضیحات نقش
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// اولویت نقش برای Role Hierarchy
        /// عدد بزرگتر = قدرت بیشتر
        /// مثلاً: SuperAdmin=100, Admin=80, Manager=60
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// وضعیت فعال/غیرفعال
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// حذف منطقی
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// شناسه کاربر ایجادکننده
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// شناسه کاربر ویرایش‌کننده
        /// </summary>
        public int? ModifiedBy { get; set; }

        /// <summary>
        /// تاریخ آخرین ویرایش
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        // Properties اضافی

        /// <summary>
        /// لیست دسترسی‌های این نقش
        /// </summary>
        public List<Permission> Permissions { get; set; }

        /// <summary>
        /// تعداد کاربرانی که این نقش را دارند
        /// </summary>
        public int UserCount { get; set; }

        /// <summary>
        /// سازنده پیش‌فرض
        /// </summary>
        public Role()
        {
            IsActive = true;
            IsDeleted = false;
            CreatedDate = DateTime.Now;
            Priority = 0;
            Permissions = new List<Permission>();
        }
    }
}