using System;
using System.Collections.Generic;

namespace RBACSystem.Models.RBAC
{
    // ====================================================================================================
    // 8. RoleHierarchy Model - مدل سلسله مراتب نقش‌ها
    // ====================================================================================================

    /// <summary>
    /// مدل سلسله مراتب نقش‌ها
    /// نمایانگر رابطه والد-فرزند بین نقش‌ها
    /// </summary>
    public class RoleHierarchy
    {
        /// <summary>
        /// شناسه یکتای سلسله مراتب
        /// </summary>
        public int HierarchyId { get; set; }

        /// <summary>
        /// شناسه نقش والد (نقش با قدرت بیشتر)
        /// </summary>
        public int ParentRoleId { get; set; }

        /// <summary>
        /// شناسه نقش فرزند (نقش با قدرت کمتر)
        /// </summary>
        public int ChildRoleId { get; set; }

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

        // Properties اضافی

        /// <summary>
        /// نام نقش والد
        /// </summary>
        public string ParentRoleName { get; set; }

        /// <summary>
        /// نام نقش فرزند
        /// </summary>
        public string ChildRoleName { get; set; }

        /// <summary>
        /// سازنده پیش‌فرض
        /// </summary>
        public RoleHierarchy()
        {
            IsDeleted = false;
            CreatedDate = DateTime.Now;
        }
    }
}