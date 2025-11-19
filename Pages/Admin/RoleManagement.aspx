<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RoleManagement.aspx.cs" Inherits="Pages_Admin_RoleManagement" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>مدیریت نقش‌ها</title>
    <link href="../../css/fontstyle.css" rel="stylesheet" />
    <meta charset="utf-8" />
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: Tahoma, Arial, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            direction: rtl;
            padding: 20px;
        }

        .container {
            max-width: 1200px;
            margin: 0 auto;
            background: white;
            border-radius: 15px;
            box-shadow: 0 10px 40px rgba(0,0,0,0.2);
            overflow: hidden;
        }

        .header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 30px;
            text-align: center;
        }

        .header h1 {
            font-size: 28px;
            margin-bottom: 10px;
        }

        .content {
            padding: 30px;
        }

        .section {
            margin-bottom: 30px;
            padding: 20px;
            background: #f8f9fa;
            border-radius: 10px;
        }

        .section-title {
            font-size: 20px;
            font-weight: bold;
            color: #667eea;
            margin-bottom: 15px;
            padding-bottom: 10px;
            border-bottom: 2px solid #667eea;
        }

        .form-group {
            margin-bottom: 15px;
        }

        .form-group label {
            display: block;
            margin-bottom: 5px;
            font-weight: bold;
            color: #333;
        }

        .form-control {
            width: 100%;
            padding: 10px;
            border: 2px solid #ddd;
            border-radius: 5px;
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
            border-radius: 5px;
            font-size: 14px;
            cursor: pointer;
            transition: all 0.3s;
            font-weight: bold;
        }

        .btn-primary {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
        }

        .btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(102, 126, 234, 0.4);
        }

        .btn-danger {
            background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
            color: white;
        }

        .btn-danger:hover {
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(245, 87, 108, 0.4);
        }

        .btn-success {
            background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
            color: white;
        }

        .alert {
            padding: 15px;
            border-radius: 5px;
            margin-bottom: 20px;
        }

        .alert-success {
            background: #d4edda;
            border: 1px solid #c3e6cb;
            color: #155724;
        }

        .alert-error {
            background: #f8d7da;
            border: 1px solid #f5c6cb;
            color: #721c24;
        }

        .permissions-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
            gap: 15px;
            margin-top: 15px;
        }

        .permission-category {
            background: white;
            padding: 15px;
            border-radius: 8px;
            border: 2px solid #e0e0e0;
        }

        .permission-category h4 {
            color: #667eea;
            margin-bottom: 10px;
            font-size: 16px;
        }

        .permission-item {
            padding: 8px;
            margin: 5px 0;
        }

        .permission-item input[type="checkbox"] {
            margin-left: 8px;
        }

        .roles-list {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
            gap: 15px;
            margin-top: 15px;
        }

        .role-card {
            background: white;
            padding: 20px;
            border-radius: 10px;
            border: 2px solid #e0e0e0;
            transition: all 0.3s;
        }

        .role-card:hover {
            border-color: #667eea;
            box-shadow: 0 5px 15px rgba(102, 126, 234, 0.2);
        }

        .role-card h3 {
            color: #667eea;
            margin-bottom: 10px;
        }

        .role-card p {
            color: #666;
            margin-bottom: 15px;
        }

        .role-card .role-actions {
            display: flex;
            gap: 10px;
        }

        .GridView {
            width: 100%;
            border-collapse: collapse;
            margin-top: 15px;
        }

        .GridView th {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 12px;
            text-align: center;
        }

        .GridView td {
            padding: 10px;
            border-bottom: 1px solid #ddd;
            text-align: center;
        }

        .GridView tr:hover {
            background: #f8f9fa;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header">
                <h1>🎭 مدیریت نقش‌ها و دسترسی‌ها</h1>
                <p>ایجاد، ویرایش و تخصیص دسترسی به نقش‌ها</p>
            </div>
            <p><a href="../AdminTest.aspx" >صفحه مدیریت مرکزی</a></p>
            <div class="content">
                <!-- پیام‌ها -->
                <asp:Panel ID="pnlMessage" runat="server" Visible="false">
                    <asp:Label ID="lblMessage" runat="server" CssClass="alert"></asp:Label>
                </asp:Panel>

                <!-- بخش 1: ایجاد/ویرایش نقش -->
                <div class="section">
                    <div class="section-title">📝 ایجاد/ویرایش نقش</div>
                    
                    <div class="form-group">
                        <label>نام نقش (انگلیسی):</label>
                        <asp:TextBox ID="txtRoleName" runat="server" CssClass="form-control" 
                                     placeholder="مثلاً: Manager" MaxLength="50"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label>توضیحات:</label>
                        <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" 
                                     placeholder="توضیحات نقش" TextMode="MultiLine" Rows="3"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label>اولویت (عدد بزرگتر = قدرت بیشتر):</label>
                        <asp:TextBox ID="txtPriority" runat="server" CssClass="form-control" 
                                     placeholder="مثلاً: 50" Text="0"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <asp:CheckBox ID="chkIsActive" runat="server" Text="  فعال" Checked="true" />
                    </div>

                    <asp:Button ID="btnSaveRole" runat="server" Text="💾 ذخیره نقش" 
                                CssClass="btn btn-primary" OnClick="btnSaveRole_Click" />
                    
                    <asp:Button ID="btnCancelEdit" runat="server" Text="❌ انصراف" 
                                CssClass="btn btn-danger" OnClick="btnCancelEdit_Click" Visible="false" />
                    
                    <asp:HiddenField ID="hfEditRoleId" runat="server" Value="0" />
                </div>

                <!-- بخش 2: تخصیص دسترسی به نقش -->
                <div class="section">
                    <div class="section-title">🔐 تخصیص دسترسی به نقش</div>
                    
                    <div class="form-group">
                        <label>انتخاب نقش:</label>
                        <asp:DropDownList ID="ddlRoles" runat="server" CssClass="form-control" 
                                          AutoPostBack="true" OnSelectedIndexChanged="ddlRoles_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>

                    <div class="permissions-grid">
                        <asp:Repeater ID="rptPermissionsByCategory" runat="server">
                            <ItemTemplate>
                                <div class="permission-category">
                                    <h4><%# Eval("Key") %></h4>
                                    <asp:CheckBoxList ID="cblPermissions" runat="server" 
                                                      DataSource='<%# Eval("Value") %>'
                                                      DataTextField="DisplayName"
                                                      DataValueField="PermissionId"
                                                      CssClass="permission-list">
                                    </asp:CheckBoxList>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>

                    <div style="margin-top: 20px;">
                        <asp:Button ID="btnSavePermissions" runat="server" Text="💾 ذخیره دسترسی‌ها" 
                                    CssClass="btn btn-success" OnClick="btnSavePermissions_Click" />
                    </div>
                </div>

                <!-- بخش 3: لیست نقش‌ها -->
                <div class="section">
                    <div class="section-title">📋 لیست نقش‌ها</div>
                    
                    <asp:GridView ID="gvRoles" runat="server" AutoGenerateColumns="False" 
                                  CssClass="GridView" OnRowCommand="gvRoles_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="RoleId" HeaderText="شناسه" />
                            <asp:BoundField DataField="RoleName" HeaderText="نام نقش" />
                            <asp:BoundField DataField="Description" HeaderText="توضیحات" />
                            <asp:BoundField DataField="Priority" HeaderText="اولویت" />
                            <asp:BoundField DataField="UserCount" HeaderText="تعداد کاربران" />
                            <asp:CheckBoxField DataField="IsActive" HeaderText="فعال" />
                            <asp:TemplateField HeaderText="عملیات">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEdit" runat="server" 
                                                    CommandName="EditRole" 
                                                    CommandArgument='<%# Eval("RoleId") %>'
                                                    Text="✏️ ویرایش" CssClass="btn btn-primary" />
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