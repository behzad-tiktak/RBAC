<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PermissionManagement.aspx.cs" Inherits="Pages_Admin_PermissionManagement" %>

<!DOCTYPE html>
<html dir="rtl" lang="fa">
<head runat="server">
    <title>مدیریت دسترسی‌ها</title>
    <link href="../../css/fontstyle.css" rel="stylesheet" />
    <meta charset="utf-8" />
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        
        body {
            font-family: Tahoma, Arial, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            padding: 20px;
            min-height: 100vh;
        }
        
        .container {
            max-width: 1400px;
            margin: 0 auto;
            background: white;
            border-radius: 15px;
            box-shadow: 0 10px 40px rgba(0,0,0,0.2);
        }
        
        .header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 30px;
            text-align: center;
            border-radius: 15px 15px 0 0;
        }
        
        .header h1 { font-size: 28px; margin-bottom: 10px; }
        
        .content { padding: 30px; }
        
        .section {
            margin-bottom: 30px;
            padding: 25px;
            background: #f8f9fa;
            border-radius: 10px;
            border: 2px solid #e0e0e0;
        }
        
        .section-title {
            font-size: 20px;
            font-weight: bold;
            color: #667eea;
            margin-bottom: 20px;
            padding-bottom: 10px;
            border-bottom: 3px solid #667eea;
        }
        
        .form-row {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 20px;
            margin-bottom: 20px;
        }
        
        .form-group {
            margin-bottom: 15px;
        }
        
        .form-group label {
            display: block;
            margin-bottom: 8px;
            font-weight: bold;
            color: #333;
        }
        
        .form-control {
            width: 100%;
            padding: 12px;
            border: 2px solid #ddd;
            border-radius: 8px;
            font-size: 14px;
            transition: border-color 0.3s;
        }
        
        .form-control:focus {
            outline: none;
            border-color: #667eea;
        }
        
        .btn {
            padding: 12px 30px;
            border: none;
            border-radius: 8px;
            font-size: 14px;
            cursor: pointer;
            font-weight: bold;
            margin-left: 10px;
            transition: all 0.3s;
        }
        
        .btn-primary {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
        }
        
        .btn-danger {
            background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
            color: white;
        }
        
        .btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(0,0,0,0.3);
        }
        
        .alert {
            padding: 15px;
            border-radius: 8px;
            margin-bottom: 20px;
        }
        
        .alert-success {
            background: #d4edda;
            border: 2px solid #c3e6cb;
            color: #155724;
        }
        
        .alert-error {
            background: #f8d7da;
            border: 2px solid #f5c6cb;
            color: #721c24;
        }
        
        .info-box {
            background: #e7f3ff;
            border: 2px solid #b3d9ff;
            border-radius: 8px;
            padding: 15px;
            margin-bottom: 20px;
        }
        
        .info-box h4 {
            color: #004085;
            margin-bottom: 10px;
        }
        
        .info-box ul {
            margin-right: 20px;
            color: #004085;
        }
        
        .info-box li {
            margin-bottom: 5px;
        }
        
        .GridView {
            width: 100%;
            border-collapse: collapse;
            background: white;
        }
        
        .GridView th {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 15px;
            text-align: center;
        }
        
        .GridView td {
            padding: 12px;
            border-bottom: 1px solid #ddd;
            text-align: center;
        }
        
        .GridView tr:hover {
            background: #f8f9fa;
        }
        
        .category-badge {
            display: inline-block;
            padding: 5px 15px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: bold;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
        }
        
        .filter-bar {
            background: white;
            padding: 15px;
            border-radius: 8px;
            margin-bottom: 20px;
            display: flex;
            gap: 15px;
            align-items: end;
        }
        
        .filter-bar .form-group {
            flex: 1;
            margin-bottom: 0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header">
                <h1>🔐 مدیریت دسترسی‌ها</h1>
                <p>ایجاد، ویرایش و حذف دسترسی‌های سیستم</p>
            </div>

            <div class="content">
                <!-- پیام -->
                <asp:Panel ID="pnlMessage" runat="server" Visible="false">
                    <asp:Label ID="lblMessage" runat="server" CssClass="alert"></asp:Label>
                </asp:Panel>

                <!-- راهنما -->
                <div class="info-box">
                    <h4>📌 نکات مهم:</h4>
                    <ul>
                        <li><strong>فرمت نام دسترسی:</strong> Category.Action (مثلاً: Users.View، Roles.Manage)</li>
                        <li><strong>Category:</strong> دسته‌بندی دسترسی (Users, Roles, Groups, Reports و...)</li>
                        <li><strong>ResourceType:</strong> Page (صفحه)، Action (عملیات)، Feature (قابلیت)</li>
                        <li><strong>نام یکتا:</strong> هر دسترسی باید نام یکتا داشته باشد</li>
                    </ul>
                </div>

                <!-- بخش 1: ایجاد/ویرایش دسترسی -->
                <div class="section">
                    <div class="section-title">📝 ایجاد/ویرایش دسترسی</div>
                    
                    <div class="form-row">
                        <div class="form-group">
                            <label>نام دسترسی (PermissionName): *</label>
                            <asp:TextBox ID="txtPermissionName" runat="server" CssClass="form-control" 
                                         placeholder="مثلاً: Users.View" MaxLength="100"></asp:TextBox>
                            <small style="color: #666;">فرمت: Category.Action</small>
                        </div>

                        <div class="form-group">
                            <label>نام نمایشی (فارسی): *</label>
                            <asp:TextBox ID="txtDisplayName" runat="server" CssClass="form-control" 
                                         placeholder="مثلاً: مشاهده کاربران" MaxLength="200"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label>دسته‌بندی (Category):</label>
                            <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">-- انتخاب یا تایپ کنید --</asp:ListItem>
                                <asp:ListItem Value="Users">Users - کاربران</asp:ListItem>
                                <asp:ListItem Value="Roles">Roles - نقش‌ها</asp:ListItem>
                                <asp:ListItem Value="Groups">Groups - گروه‌ها</asp:ListItem>
                                <asp:ListItem Value="Permissions">Permissions - دسترسی‌ها</asp:ListItem>
                                <asp:ListItem Value="Reports">Reports - گزارشات</asp:ListItem>
                                <asp:ListItem Value="Settings">Settings - تنظیمات</asp:ListItem>
                                <asp:ListItem Value="AuditLog">AuditLog - لاگ‌ها</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-group">
                            <label>توضیحات:</label>
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" 
                                         placeholder="توضیحات کامل دسترسی" TextMode="MultiLine" Rows="2"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label>نوع منبع (ResourceType):</label>
                            <asp:DropDownList ID="ddlResourceType" runat="server" CssClass="form-control">
                                <asp:ListItem Value="Page">Page - صفحه</asp:ListItem>
                                <asp:ListItem Value="Action">Action - عملیات</asp:ListItem>
                                <asp:ListItem Value="Feature">Feature - قابلیت</asp:ListItem>
                                <asp:ListItem Value="API">API - سرویس</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <label>مسیر منبع (ResourcePath):</label>
                            <asp:TextBox ID="txtResourcePath" runat="server" CssClass="form-control" 
                                         placeholder="مثلاً: ~/Admin/Users.aspx" MaxLength="500"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:CheckBox ID="chkIsActive" runat="server" Text="  فعال" Checked="true" />
                    </div>

                    <div style="margin-top: 20px;">
                        <asp:Button ID="btnSavePermission" runat="server" Text="💾 ذخیره دسترسی" 
                                    CssClass="btn btn-primary" OnClick="btnSavePermission_Click" />
                        
                        <asp:Button ID="btnCancelEdit" runat="server" Text="❌ انصراف" 
                                    CssClass="btn btn-danger" OnClick="btnCancelEdit_Click" Visible="false" />
                    </div>
                    
                    <asp:HiddenField ID="hfEditPermissionId" runat="server" Value="0" />
                </div>

                <!-- بخش 2: فیلتر -->
                <div class="filter-bar">
                    <div class="form-group">
                        <label>فیلتر بر اساس دسته:</label>
                        <asp:DropDownList ID="ddlFilterCategory" runat="server" CssClass="form-control" 
                                          AutoPostBack="true" OnSelectedIndexChanged="ddlFilterCategory_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>

                    <div class="form-group">
                        <label>وضعیت:</label>
                        <asp:DropDownList ID="ddlFilterStatus" runat="server" CssClass="form-control" 
                                          AutoPostBack="true" OnSelectedIndexChanged="ddlFilterCategory_SelectedIndexChanged">
                            <asp:ListItem Value="all">همه</asp:ListItem>
                            <asp:ListItem Value="active" Selected="True">فعال</asp:ListItem>
                            <asp:ListItem Value="inactive">غیرفعال</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <!-- بخش 3: لیست دسترسی‌ها -->
                <div class="section">
                    <div class="section-title">📋 لیست دسترسی‌ها</div>
                    
                    <asp:GridView ID="gvPermissions" runat="server" AutoGenerateColumns="False" 
                                  CssClass="GridView" OnRowCommand="gvPermissions_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="PermissionId" HeaderText="شناسه" />
                            <asp:BoundField DataField="PermissionName" HeaderText="نام دسترسی" />
                            <asp:BoundField DataField="DisplayName" HeaderText="نام نمایشی" />
                            <asp:TemplateField HeaderText="دسته">
                                <ItemTemplate>
                                    <span class="category-badge"><%# Eval("Category") %></span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="ResourceType" HeaderText="نوع منبع" />
                            <asp:CheckBoxField DataField="IsActive" HeaderText="فعال" />
                            <asp:TemplateField HeaderText="عملیات">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEdit" runat="server" 
                                                    CommandName="EditPermission" 
                                                    CommandArgument='<%# Eval("PermissionId") %>'
                                                    Text="✏️ ویرایش" 
                                                    CssClass="btn btn-primary" 
                                                    style="font-size: 12px; padding: 8px 15px;" />
                                    
                                    <asp:LinkButton ID="btnDelete" runat="server" 
                                                    CommandName="DeletePermission" 
                                                    CommandArgument='<%# Eval("PermissionId") %>'
                                                    Text="🗑️ حذف" 
                                                    CssClass="btn btn-danger" 
                                                    style="font-size: 12px; padding: 8px 15px;"
                                                    OnClientClick="return confirm('آیا مطمئن هستید؟ این دسترسی از تمام نقش‌ها حذف می‌شود!');" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
