using System;
using System.Collections.Generic;

namespace RBACSystem.Models.RBAC
{
    // ====================================================================================================
    // 6. RolePermission Model - مدل رابطه نقش و دسترسی
    // ====================================================================================================

    /// <summary>
    /// مدل رابطه بین نقش و دسترسی
    /// نمایانگر تخصیص یک دسترسی به یک نقش
    /// </summary>
    public class RolePermission
    {
        /// <summary>
        /// شناسه یکتای رابطه
        /// </summary>
        public int RolePermissionId { get; set; }

        /// <summary>
        /// شناسه نقش
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// شناسه دسترسی
        /// </summary>
        public int PermissionId { get; set; }

        /// <summary>
        /// حذف منطقی
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// شناسه کاربری که این دسترسی را تخصیص داده
        /// </summary>
        public int? AssignedBy { get; set; }

        /// <summary>
        /// تاریخ تخصیص
        /// </summary>
        public DateTime AssignedDate { get; set; }

        // Properties اضافی

        /// <summary>
        /// نام نقش
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// نام دسترسی
        /// </summary>
        public string PermissionName { get; set; }

        /// <summary>
        /// سازنده پیش‌فرض
        /// </summary>
        public RolePermission()
        {
            IsDeleted = false;
            AssignedDate = DateTime.Now;
        }
    }
}