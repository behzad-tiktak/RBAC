using System;
using System.Collections.Generic;

namespace RBACSystem.Models.RBAC
{
    // ====================================================================================================
    // 3. Permission Model - مدل دسترسی
    // ====================================================================================================

    /// <summary>
    /// مدل دسترسی سیستم
    /// نمایانگر یک دسترسی (مانند Users.View، Reports.Edit)
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// شناسه یکتای دسترسی
        /// </summary>
        public int PermissionId { get; set; }

        /// <summary>
        /// نام سیستمی دسترسی (یکتا)
        /// فرمت: Category.Action (مثلاً Users.View)
        /// </summary>
        public string PermissionName { get; set; }

        /// <summary>
        /// نام نمایشی فارسی
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// توضیحات دسترسی
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// دسته‌بندی (مثلاً Users, Reports, Settings)
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// نوع منبع (Page, Action, Feature)
        /// </summary>
        public string ResourceType { get; set; }

        /// <summary>
        /// مسیر منبع (مثلاً ~/Admin/Users.aspx)
        /// </summary>
        public string ResourcePath { get; set; }

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

        /// <summary>
        /// سازنده پیش‌فرض
        /// </summary>
        public Permission()
        {
            IsActive = true;
            IsDeleted = false;
            CreatedDate = DateTime.Now;
        }
    }
}