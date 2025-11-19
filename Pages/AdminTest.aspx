<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AdminTest.aspx.cs" Inherits="Pages_AdminTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" dir="rtl" lang="fa">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>مدیریت جامع RBAC</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: vazirmatn,Tahoma, Arial, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            padding: 20px;
        }

        .container {
            max-width: 1400px;
            margin: 0 auto;
        }

        .header {
            background: white;
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.2);
            margin-bottom: 20px;
            text-align: center;
        }

            .header h1 {
                color: #667eea;
                font-size: 28px;
                margin-bottom: 10px;
            }

        .main-grid {
            display: grid;
            grid-template-columns: 350px 1fr;
            gap: 20px;
        }

        .sidebar {
            background: white;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 5px 15px rgba(0,0,0,0.1);
            height: fit-content;
            position: sticky;
            top: 20px;
        }

        .content-area {
            background: white;
            border-radius: 10px;
            padding: 25px;
            box-shadow: 0 5px 15px rgba(0,0,0,0.1);
        }

        .section {
            margin-bottom: 25px;
            padding-bottom: 20px;
            border-bottom: 2px solid #f0f0f0;
        }

            .section:last-child {
                border-bottom: none;
            }

        .section-title {
            color: #667eea;
            font-size: 18px;
            margin-bottom: 15px;
            padding-bottom: 10px;
            border-bottom: 2px solid #f0f0f0;
            font-weight: bold;
        }

        .user-card {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 20px;
            border-radius: 8px;
            margin-bottom: 20px;
            text-align: center;
        }

            .user-card h3 {
                font-size: 20px;
                margin-bottom: 10px;
            }

            .user-card p {
                font-size: 13px;
                opacity: 0.9;
                margin: 5px 0;
            }

        .form-group {
            margin-bottom: 15px;
        }

            .form-group label {
                /*display: block;*/
                margin-bottom: 5px;
                color: #333;
                font-weight: bold;
                font-size: 13px;
            }

        .form-control {
            width: 100%;
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 5px;
            font-size: 14px;
            font-family: Tahoma, Arial;
        }

            .form-control:focus {
                outline: none;
                border-color: #667eea;
                box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
            }

        .btn {
            padding: 10px 20px;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
            font-family: Vazirmatn, Tahoma;
            transition: all 0.3s;
            margin: 3px;
        }

        .btn-primary {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
        }

            .btn-primary:hover {
                transform: translateY(-2px);
                box-shadow: 0 5px 15px rgba(102, 126, 234, 0.4);
            }

        .btn-success {
            background: #28a745;
            color: white;
        }

        .btn-danger {
            background: #dc3545;
            color: white;
        }

        .btn-warning {
            background: #ffc107;
            color: #333;
        }

        .btn-info {
            background: #17a2b8;
            color: white;
        }

        .btn-block {
            width: 100%;
            display: block;
        }

        .checklist {
            max-height: 300px;
            overflow-y: auto;
            border: 1px solid #ddd;
            border-radius: 5px;
            padding: 10px;
            background: #f9f9f9;
        }

        .checklist-item {
            padding: 8px;
            margin: 5px 0;
            background: white;
            border-radius: 3px;
            /*display: flex;*/
            align-items: center;
        }

            .checklist-item input[type="checkbox"] {
                margin-left: 10px;
                width: 18px;
                height: 18px;
                cursor: pointer;
            }

            .checklist-item label {
                cursor: pointer;
                /*flex: 1;*/
                margin: 0;
                font-weight: normal;
            }

        .info-box {
            background: #e3f2fd;
            border-right: 4px solid #2196f3;
            padding: 15px;
            border-radius: 5px;
            margin: 15px 0;
        }

        .success-box {
            background: #e8f5e9;
            border-right: 4px solid #4caf50;
            padding: 15px;
            border-radius: 5px;
            margin: 15px 0;
        }

        .error-box {
            background: #ffebee;
            border-right: 4px solid #f44336;
            padding: 15px;
            border-radius: 5px;
            margin: 15px 0;
        }

        .stats-grid {
            display: grid;
            grid-template-columns: repeat(2, 1fr);
            gap: 10px;
            margin: 15px 0;
        }

        .stat-item {
            background: #f5f5f5;
            padding: 15px;
            border-radius: 5px;
            text-align: center;
        }

            .stat-item h4 {
                font-size: 24px;
                color: #667eea;
                margin-bottom: 5px;
            }

            .stat-item p {
                font-size: 12px;
                color: #666;
            }

        .result-panel {
            background: #f8f9fa;
            border: 1px solid #ddd;
            border-radius: 5px;
            padding: 15px;
            margin: 15px 0;
            max-height: 400px;
            overflow-y: auto;
            font-size: 13px;
        }

        .badge {
            display: inline-block;
            padding: 3px 8px;
            border-radius: 3px;
            font-size: 11px;
            font-weight: bold;
            margin: 2px;
        }

        .badge-success {
            background: #28a745;
            color: white;
        }

        .badge-danger {
            background: #dc3545;
            color: white;
        }

        .badge-info {
            background: #17a2b8;
            color: white;
        }

        .badge-warning {
            background: #ffc107;
            color: #333;
        }

        .action-buttons {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            margin-top: 10px;
        }

        .divider {
            height: 1px;
            background: #e0e0e0;
            margin: 20px 0;
        }

        .loading {
            text-align: center;
            padding: 20px;
            color: #667eea;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true"></asp:ScriptManager>

        <div class="container">
            <!-- Header -->
            <div class="header">
                <h1>🎛️ پنل مدیریت جامع RBAC</h1>
                <p>مدیریت کاربران، نقش‌ها و دسترسی‌ها با Session و Cache</p>
            </div>

            <div class="main-grid">
                <!-- Sidebar -->
                <div class="sidebar">
                    <div>
                        <ul>
                            <li><a href="Admin/GroupManagement.aspx">مدیریت گروه ها</a></li>
                            <li><a href="Admin/RoleManagement.aspx">مدیریت نقش ها</a></li>
                            <li><a href="Admin/UserManagement.aspx">مدیریت کاربر ها</a></li>
                        </ul>
                    </div>
                    <asp:UpdatePanel ID="upSidebar" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <!-- Current User Card -->
                            <div class="user-card">
                                <h3>👤 کاربر فعلی</h3>
                                <asp:Literal ID="litCurrentUser" runat="server"></asp:Literal>
                            </div>

                            <!-- User Selection -->
                            <div class="section">
                                <div class="section-title">انتخاب کاربر</div>
                                <div class="form-group">
                                    <label>کاربر:</label>
                                    <asp:DropDownList ID="ddlUsers" runat="server" CssClass="form-control"
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlUsers_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>
                                <asp:Button ID="btnLoadUser" runat="server" Text="بارگذاری در Session"
                                    CssClass="btn btn-primary btn-block" OnClick="btnLoadUser_Click" />
                                <asp:Button ID="btnLogout" runat="server" Text="خروج از Session"
                                    CssClass="btn btn-danger btn-block" OnClick="btnLogout_Click" />
                            </div>

                            <!-- Quick Stats -->
                            <div class="section">
                                <div class="section-title">📊 آمار سریع</div>
                                <div class="stats-grid">
                                    <div class="stat-item">
                                        <h4>
                                            <asp:Literal ID="litTotalUsers" runat="server"></asp:Literal></h4>
                                        <p>کاربران</p>
                                    </div>
                                    <div class="stat-item">
                                        <h4>
                                            <asp:Literal ID="litTotalRoles" runat="server"></asp:Literal></h4>
                                        <p>نقش‌ها</p>
                                    </div>
                                </div>
                                <asp:Button ID="btnRefreshStats" runat="server" Text="🔄 بروزرسانی"
                                    CssClass="btn btn-info btn-block" OnClick="btnRefreshStats_Click" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>

                <!-- Content Area -->
                <div class="content-area">
                    <asp:UpdatePanel ID="upContent" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>

                            <!-- Section 1: User Management -->
                            <div class="section">
                                <div class="section-title">👤 مدیریت کاربر</div>

                                <div class="action-buttons">
                                    <asp:Button ID="btnShowUserInfo" runat="server" Text="📋 اطلاعات کامل"
                                        CssClass="btn btn-info" OnClick="btnShowUserInfo_Click" />
                                    <asp:Button ID="btnShowUserRoles" runat="server" Text="🎭 نقش‌های کاربر"
                                        CssClass="btn btn-info" OnClick="btnShowUserRoles_Click" />
                                    <asp:Button ID="btnShowUserPermissions" runat="server" Text="🔐 دسترسی‌های کاربر"
                                        CssClass="btn btn-info" OnClick="btnShowUserPermissions_Click" />
                                    <asp:Button ID="btnTestLogin" runat="server" Text="🔑 تست Login"
                                        CssClass="btn btn-warning" OnClick="btnTestLogin_Click" />
                                </div>

                                <asp:Panel ID="pnlUserResult" runat="server" CssClass="result-panel" Visible="false">
                                    <asp:Literal ID="litUserResult" runat="server"></asp:Literal>
                                </asp:Panel>
                            </div>

                            <!-- Section 2: Role Management -->
                            <div class="section">
                                <div class="section-title">🎭 مدیریت نقش‌ها</div>

                                <div class="form-group">
                                    <h3>نقش های موجود:</h3>
                                    <div class="checklist">
                                        <asp:CheckBoxList ID="cblRoles" runat="server" CssClass="checklist"></asp:CheckBoxList>
                                    </div>
                                </div>

                                <div class="action-buttons">
                                    <asp:Button ID="btnAssignRoles" runat="server" Text="✅ تخصیص نقش‌های انتخابی"
                                        CssClass="btn btn-success" OnClick="btnAssignRoles_Click" />
                                    <asp:Button ID="btnRemoveRoles" runat="server" Text="❌ حذف نقش‌های انتخابی"
                                        CssClass="btn btn-danger" OnClick="btnRemoveRoles_Click" />
                                    <asp:Button ID="btnLoadRoles" runat="server" Text="🔄 بارگذاری نقش‌ها"
                                        CssClass="btn btn-info" OnClick="btnLoadRoles_Click" />
                                </div>

                                <asp:Panel ID="pnlRoleResult" runat="server" CssClass="result-panel" Visible="false">
                                    <asp:Literal ID="litRoleResult" runat="server"></asp:Literal>
                                </asp:Panel>
                            </div>

                            <!-- Section 3: Permission Management -->
                            <div class="section">
                                <div class="section-title">🔐 مدیریت دسترسی‌ها</div>

                                <div class="form-group">
                                    <label>انتخاب نقش برای مدیریت دسترسی:</label>
                                    <asp:DropDownList ID="ddlRolesForPermission" runat="server" CssClass="form-control"
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlRolesForPermission_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>

                                <div class="form-group">
                                    <label>دسترسی‌های موجود:</label>
                                    <div class="checklist">
                                        <asp:CheckBoxList ID="cblPermissions" runat="server" CssClass="checklist"></asp:CheckBoxList>
                                    </div>
                                </div>

                                <div class="action-buttons">
                                    <asp:Button ID="btnAssignPermissions" runat="server" Text="✅ تخصیص دسترسی‌ها"
                                        CssClass="btn btn-success" OnClick="btnAssignPermissions_Click" />
                                    <asp:Button ID="btnRemovePermissions" runat="server" Text="❌ حذف دسترسی‌ها"
                                        CssClass="btn btn-danger" OnClick="btnRemovePermissions_Click" />
                                    <asp:Button ID="btnShowRolePermissions" runat="server" Text="📋 نمایش دسترسی‌های نقش"
                                        CssClass="btn btn-info" OnClick="btnShowRolePermissions_Click" />
                                </div>

                                <asp:Panel ID="pnlPermissionResult" runat="server" CssClass="result-panel" Visible="false">
                                    <asp:Literal ID="litPermissionResult" runat="server"></asp:Literal>
                                </asp:Panel>
                            </div>

                            <!-- Section 4: Cache Management -->
                            <div class="section">
                                <div class="section-title">⚡ مدیریت Cache</div>

                                <div class="action-buttons">
                                    <asp:Button ID="btnTestCache" runat="server" Text="🧪 تست Performance Cache"
                                        CssClass="btn btn-info" OnClick="btnTestCache_Click" />
                                    <asp:Button ID="btnClearUserCache" runat="server" Text="🗑️ پاک کردن Cache کاربر"
                                        CssClass="btn btn-warning" OnClick="btnClearUserCache_Click" />
                                    <asp:Button ID="btnClearAllCache" runat="server" Text="💣 پاک کردن تمام Cache"
                                        CssClass="btn btn-danger" OnClick="btnClearAllCache_Click" />
                                </div>

                                <asp:Panel ID="pnlCacheResult" runat="server" CssClass="result-panel" Visible="false">
                                    <asp:Literal ID="litCacheResult" runat="server"></asp:Literal>
                                </asp:Panel>
                            </div>

                            <!-- Section 5: CRUD Operations -->
                            <div class="section">
                                <div class="section-title">⚙️ عملیات CRUD</div>

                                <div class="form-group">
                                    <label>نام کاربری جدید:</label>
                                    <asp:TextBox ID="txtNewUsername" runat="server" CssClass="form-control"
                                        placeholder="نام کاربری"></asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <label>ایمیل:</label>
                                    <asp:TextBox ID="txtNewEmail" runat="server" CssClass="form-control"
                                        placeholder="example@domain.com"></asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <label>نام کامل:</label>
                                    <asp:TextBox ID="txtNewFullName" runat="server" CssClass="form-control"
                                        placeholder="نام و نام خانوادگی"></asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <label>رمز عبور:</label>
                                    <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control"
                                        TextMode="Password" placeholder="حداقل 6 کاراکتر"></asp:TextBox>
                                </div>

                                <div class="action-buttons">
                                    <asp:Button ID="btnCreateUser" runat="server" Text="➕ ایجاد کاربر"
                                        CssClass="btn btn-success" OnClick="btnCreateUser_Click" />
                                    <asp:Button ID="btnUpdateUser" runat="server" Text="✏️ بروزرسانی کاربر فعلی"
                                        CssClass="btn btn-warning" OnClick="btnUpdateUser_Click" />
                                    <asp:Button ID="btnDeleteUser" runat="server" Text="🗑️ حذف کاربر فعلی"
                                        CssClass="btn btn-danger" OnClick="btnDeleteUser_Click"
                                        OnClientClick="return confirm('آیا مطمئن هستید؟');" />
                                </div>

                                <asp:Panel ID="pnlCrudResult" runat="server" CssClass="result-panel" Visible="false">
                                    <asp:Literal ID="litCrudResult" runat="server"></asp:Literal>
                                </asp:Panel>
                            </div>

                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
