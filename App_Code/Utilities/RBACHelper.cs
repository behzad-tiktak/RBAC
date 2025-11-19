// ====================================================================================================
// RBACHelper - کلاس کمکی برای عملیات RBAC
// مسیر: App_Code/Utilities/RBACHelper.cs
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using RBACSystem.BusinessLogic.RBAC;
using RBACSystem.Models.RBAC;

namespace RBACSystem.Utilities
{
    /// <summary>
    /// کلاس کمکی برای عملیات RBAC در UI
    /// </summary>
    public static class RBACHelper
    {
        #region Bind Controls

        /// <summary>
        /// Bind کردن لیست کاربران به DropDownList
        /// </summary>
        public static void BindUsersToDropDown(DropDownList ddl, bool includeInactive = false, bool addEmptyItem = true)
        {
            try
            {
                var rbacManager = new RBACManager();
                List<User> users = rbacManager.GetAllUsers(includeInactive);

                ddl.DataSource = users;
                ddl.DataTextField = "FullName";
                ddl.DataValueField = "UserId";
                ddl.DataBind();

                if (addEmptyItem)
                {
                    ddl.Items.Insert(0, new ListItem("-- انتخاب کاربر --", "0"));
                }
            }
            catch
            {
                // در صورت خطا
            }
        }

        /// <summary>
        /// Bind کردن لیست نقش‌ها به DropDownList
        /// </summary>
        public static void BindRolesToDropDown(DropDownList ddl, bool includeInactive = false, bool addEmptyItem = true)
        {
            try
            {
                var rbacManager = new RBACManager();
                List<Role> roles = rbacManager.GetAllRoles(includeInactive);

                ddl.DataSource = roles;
                ddl.DataTextField = "RoleName";
                ddl.DataValueField = "RoleId";
                ddl.DataBind();

                if (addEmptyItem)
                {
                    ddl.Items.Insert(0, new ListItem("-- انتخاب نقش --", "0"));
                }
            }
            catch
            {
                // در صورت خطا
            }
        }

        /// <summary>
        /// Bind کردن لیست گروه‌ها به DropDownList
        /// </summary>
        public static void BindGroupsToDropDown(DropDownList ddl, bool includeInactive = false, bool addEmptyItem = true)
        {
            try
            {
                var rbacManager = new RBACManager();
                List<UserGroup> groups = rbacManager.GetAllGroups(includeInactive);

                ddl.DataSource = groups;
                ddl.DataTextField = "GroupName";
                ddl.DataValueField = "GroupId";
                ddl.DataBind();

                if (addEmptyItem)
                {
                    ddl.Items.Insert(0, new ListItem("-- انتخاب گروه --", "0"));
                }
            }
            catch
            {
                // در صورت خطا
            }
        }

        /// <summary>
        /// Bind کردن دسترسی‌ها به CheckBoxList به صورت گروه‌بندی شده
        /// </summary>
        public static void BindPermissionsToCheckBoxList(CheckBoxList cbl, string category = null)
        {
            try
            {
                var rbacManager = new RBACManager();
                List<Permission> permissions;

                if (string.IsNullOrEmpty(category))
                {
                    permissions = rbacManager.GetAllPermissions(false);
                }
                else
                {
                    var grouped = rbacManager.GetPermissionsGroupedByCategory(false);
                    permissions = grouped.ContainsKey(category) ? grouped[category] : new List<Permission>();
                }

                cbl.DataSource = permissions;
                cbl.DataTextField = "DisplayName";
                cbl.DataValueField = "PermissionId";
                cbl.DataBind();
            }
            catch
            {
                // در صورت خطا
            }
        }

        #endregion

        #region CheckBox Selection Helpers

        /// <summary>
        /// انتخاب آیتم‌های مشخص در CheckBoxList
        /// </summary>
        public static void SelectCheckBoxListItems(CheckBoxList cbl, List<int> selectedIds)
        {
            if (cbl == null || selectedIds == null)
                return;

            foreach (ListItem item in cbl.Items)
            {
                int itemValue;
                if (int.TryParse(item.Value, out itemValue))
                {
                    item.Selected = selectedIds.Contains(itemValue);
                }
            }
        }

        /// <summary>
        /// دریافت لیست آیتم‌های انتخاب شده از CheckBoxList
        /// </summary>
        public static List<int> GetSelectedCheckBoxListItems(CheckBoxList cbl)
        {
            var selectedIds = new List<int>();

            if (cbl == null)
                return selectedIds;

            foreach (ListItem item in cbl.Items)
            {
                if (item.Selected)
                {
                    int itemValue;
                    if (int.TryParse(item.Value, out itemValue))
                    {
                        selectedIds.Add(itemValue);
                    }
                }
            }

            return selectedIds;
        }

        /// <summary>
        /// انتخاب/عدم انتخاب تمام آیتم‌های CheckBoxList
        /// </summary>
        public static void SelectAllCheckBoxListItems(CheckBoxList cbl, bool select)
        {
            if (cbl == null)
                return;

            foreach (ListItem item in cbl.Items)
            {
                item.Selected = select;
            }
        }

        #endregion

        #region Authorization Checks

        /// <summary>
        /// بررسی دسترسی کاربر فعلی
        /// </summary>
        public static bool CurrentUserHasPermission(string permissionName)
        {
            try
            {
                int userId = SecurityHelper.GetCurrentUserId();
                
                if (userId <= 0)
                    return false;

                var rbacManager = new RBACManager();
                return rbacManager.UserHasPermission(userId, permissionName);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// بررسی دسترسی کاربر فعلی به یکی از دسترسی‌ها
        /// </summary>
        public static bool CurrentUserHasAnyPermission(params string[] permissionNames)
        {
            try
            {
                int userId = SecurityHelper.GetCurrentUserId();
                
                if (userId <= 0)
                    return false;

                var rbacManager = new RBACManager();
                return rbacManager.UserHasAnyPermission(userId, permissionNames);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// بررسی اینکه آیا کاربر فعلی SuperAdmin است
        /// </summary>
        public static bool IsCurrentUserSuperAdmin()
        {
            try
            {
                int userId = SecurityHelper.GetCurrentUserId();
                
                if (userId <= 0)
                    return false;

                var rbacManager = new RBACManager();
                List<Role> userRoles = rbacManager.GetUserRoles(userId);
                
                foreach (Role role in userRoles)
                {
                    if (role.RoleName == "SuperAdmin" || role.Priority >= 100)
                        return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Redirect به صفحه Access Denied اگر دسترسی نداشته باشد
        /// </summary>
        public static void RequirePermission(string permissionName, string redirectUrl = "~/AccessDenied.aspx")
        {
            if (!CurrentUserHasPermission(permissionName))
            {
                HttpContext.Current.Response.Redirect(redirectUrl);
            }
        }

        #endregion

        #region UI Helpers

        /// <summary>
        /// غیرفعال کردن کنترل‌ها بر اساس دسترسی
        /// </summary>
        public static void DisableControlIfNoPermission(System.Web.UI.Control control, string permissionName)
        {
            if (control == null)
                return;

            if (!CurrentUserHasPermission(permissionName))
            {
                if (control is Button)
                    ((Button)control).Enabled = false;
                else if (control is LinkButton)
                    ((LinkButton)control).Enabled = false;
                else if (control is TextBox)
                    ((TextBox)control).Enabled = false;
                else if (control is DropDownList)
                    ((DropDownList)control).Enabled = false;
                else if (control is CheckBox)
                    ((CheckBox)control).Enabled = false;

                control.Visible = false; // یا می‌توانید فقط غیرفعال کنید
            }
        }

        /// <summary>
        /// نمایش پیام خطای دسترسی
        /// </summary>
        public static string GetAccessDeniedMessage()
        {
            return "شما دسترسی به این عملیات را ندارید. لطفاً با مدیر سیستم تماس بگیرید.";
        }

        #endregion

        #region Formatting Helpers

        /// <summary>
        /// فرمت کردن لیست نقش‌ها برای نمایش
        /// </summary>
        public static string FormatRolesList(List<Role> roles)
        {
            if (roles == null || roles.Count == 0)
                return "بدون نقش";

            var roleNames = new List<string>();
            foreach (Role role in roles)
            {
                roleNames.Add(role.RoleName);
            }

            return string.Join(", ", roleNames.ToArray());
        }

        /// <summary>
        /// فرمت کردن لیست دسترسی‌ها برای نمایش
        /// </summary>
        public static string FormatPermissionsList(List<Permission> permissions, int maxCount = 5)
        {
            if (permissions == null || permissions.Count == 0)
                return "بدون دسترسی";

            if (permissions.Count <= maxCount)
            {
                var permNames = new List<string>();
                foreach (Permission perm in permissions)
                {
                    permNames.Add(perm.DisplayName);
                }
                return string.Join(", ", permNames.ToArray());
            }
            else
            {
                return string.Format("{0} دسترسی", permissions.Count);
            }
        }

        /// <summary>
        /// فرمت کردن تاریخ برای نمایش فارسی
        /// </summary>
        public static string FormatPersianDate(DateTime? date)
        {
            if (!date.HasValue)
                return "-";

            try
            {
                System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
                int year = pc.GetYear(date.Value);
                int month = pc.GetMonth(date.Value);
                int day = pc.GetDayOfMonth(date.Value);
                
                return string.Format("{0:0000}/{1:00}/{2:00}", year, month, day);
            }
            catch
            {
                return date.Value.ToString("yyyy/MM/dd");
            }
        }

        #endregion

        #region Validation Helpers

        /// <summary>
        /// Validation نام کاربری
        /// </summary>
        public static bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            if (username.Length < 3 || username.Length > 50)
                return false;

            // فقط حروف، اعداد و underscore
            foreach (char c in username)
            {
                if (!char.IsLetterOrDigit(c) && c != '_')
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Validation نام نقش
        /// </summary>
        public static bool IsValidRoleName(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return false;

            if (roleName.Length < 2 || roleName.Length > 50)
                return false;

            return true;
        }

        #endregion
    }
}