using System;
using System.Collections.Generic;

namespace RBACSystem.Models.RBAC
{
    // ====================================================================================================
    // 9. AuditLog Model - مدل لاگ تغییرات
    // ====================================================================================================

    /// <summary>
    /// مدل ثبت تاریخچه تغییرات
    /// برای ردیابی تمام عملیات کاربران
    /// </summary>
    public class AuditLog
    {
        /// <summary>
        /// شناسه یکتای لاگ
        /// </summary>
        public long LogId { get; set; }

        /// <summary>
        /// شناسه کاربری که عملیات را انجام داده
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// نوع عملیات (Insert, Update, Delete, Login, Logout)
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// نام جدول تغییر یافته
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// شناسه رکورد تغییر یافته
        /// </summary>
        public int? RecordId { get; set; }

        /// <summary>
        /// مقدار قبلی (JSON)
        /// </summary>
        public string OldValue { get; set; }

        /// <summary>
        /// مقدار جدید (JSON)
        /// </summary>
        public string NewValue { get; set; }

        /// <summary>
        /// آدرس IP کاربر
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// مرورگر کاربر (User Agent)
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// تاریخ و زمان عملیات
        /// </summary>
        public DateTime ActionDate { get; set; }

        // Properties اضافی

        /// <summary>
        /// نام کاربر
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// سازنده پیش‌فرض
        /// </summary>
        public AuditLog()
        {
            ActionDate = DateTime.Now;
        }
    }
}