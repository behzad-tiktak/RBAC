using System;
using System.Collections.Generic;

namespace RBACSystem.Models.RBAC
{
    // ====================================================================================================
    // 4. UserGroup Model - مدل گروه کاربری
    // ====================================================================================================

    /// <summary>
    /// مدل گروه کاربری
    /// نمایانگر یک گروه (مانند بخش فنی، بخش مالی)
    /// </summary>
    public class UserGroup
    {
        /// <summary>
        /// شناسه یکتای گروه
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// نام گروه (یکتا)
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// توضیحات گروه
        /// </summary>
        public string Description { get; set; }

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
        /// تعداد اعضای گروه
        /// </summary>
        public int MemberCount { get; set; }

        /// <summary>
        /// سازنده پیش‌فرض
        /// </summary>
        public UserGroup()
        {
            IsActive = true;
            IsDeleted = false;
            CreatedDate = DateTime.Now;
        }
    }
}