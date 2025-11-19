<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserManagement.aspx.cs" Inherits="Pages_Admin_UserManagement" %>

<!DOCTYPE html>
<html dir="rtl" lang="fa">
<head runat="server">
    <title>مدیریت کاربران</title>
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
        
        .content { padding: 30px; }
        
        .section {
            margin-bottom: 30px;
            padding: 25px;
            background: #f8f9fa;
            border-radius: 10px;
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
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
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
        
        .checkbox-list {
            max-height: 200px;
            overflow-y: auto;
            padding: 15px;
            background: white;
            border: 2px solid #ddd;
            border-radius: 8px;
        }
        .checkbox-list label{
            display: inline-block;            
            margin:8px 4px 8px 59px;
            font-weight: bold;
            color: #333;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header">
                <h1>👤 مدیریت کاربران</h1>
                <p>ایجاد، ویرایش و مدیریت نقش‌های کاربران</p>
            </div>
            <p><a href="../AdminTest.aspx" >صفحه مدیریت مرکزی</a></p>
            <div class="content">
                <asp:Panel ID="pnlMessage" runat="server" Visible="false">
                    <asp:Label ID="lblMessage" runat="server" CssClass="alert"></asp:Label>
                </asp:Panel>

                <!-- بخش 1: ایجاد/ویرایش کاربر -->
                <div class="section">
                    <div class="section-title"><h3>📝 ایجاد/ویرایش کاربر</h3></div>
                    
                    <div class="form-row">
                        <div class="form-group">
                            <label>نام کاربری:</label>
                            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" 
                                         placeholder="نام کاربری (انگلیسی)" MaxLength="50"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label>نام کامل:</label>
                            <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" 
                                         placeholder="نام و نام خانوادگی"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label>ایمیل:</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" 
                                         placeholder="example@domain.com" TextMode="Email"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-group">
                            <label>رمز عبور:</label>
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" 
                                         placeholder="حداقل 6 کاراکتر" TextMode="Password"></asp:TextBox>
                            <small style="color: #666;">در ویرایش، خالی بگذارید تا تغییر نکند</small>
                        </div>

                        <div class="form-group">
                            <label>تکرار رمز عبور:</label>
                            <asp:TextBox ID="txtPasswordConfirm" runat="server" CssClass="form-control" 
                                         placeholder="تکرار رمز عبور" TextMode="Password"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:CheckBox ID="chkIsActive" runat="server" Text="  کاربر فعال است" Checked="true" />
                    </div>

                    <div style="margin-top: 20px;">
                        <asp:Button ID="btnSaveUser" runat="server" Text="💾 ذخیره کاربر" 
                                    CssClass="btn btn-primary" OnClick="btnSaveUser_Click" />
                        
                        <asp:Button ID="btnCancelEdit" runat="server" Text="❌ انصراف" 
                                    CssClass="btn btn-danger" OnClick="btnCancelEdit_Click" Visible="false" />
                    </div>
                    
                    <asp:HiddenField ID="hfEditUserId" runat="server" Value="0" />
                </div>

                <!-- بخش 2: تخصیص نقش‌ها -->
                <div class="section">
                    <div class="section-title"><h3>🎭 تخصیص نقش‌ها به کاربر</h3></div>
                    
                    <div class="form-group">
                        <label>انتخاب کاربر:</label>
                        <asp:DropDownList ID="ddlUsersForRoles" runat="server" CssClass="form-control" 
                                          AutoPostBack="true" OnSelectedIndexChanged="ddlUsersForRoles_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>

                    <div class="form-group">
                        <label>نقش های کاربر:</label>
                        <asp:CheckBoxList ID="cblUserRoles" runat="server" CssClass="checkbox-list" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
                    </div>

                    <div style="margin-top: 15px;">
                        <asp:Button ID="btnSaveRoles" runat="server" Text="💾 ذخیره نقش‌ها" 
                                    CssClass="btn btn-primary" OnClick="btnSaveRoles_Click" />
                    </div>
                </div>

                <!-- بخش 3: لیست کاربران -->
                <div class="section">
                    <div class="section-title"><h3>📋 لیست کاربران</h3></div>
                    
                    <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False" 
                                  CssClass="GridView" OnRowCommand="gvUsers_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="UserId" HeaderText="شناسه" />
                            <asp:BoundField DataField="Username" HeaderText="نام کاربری" />
                            <asp:BoundField DataField="FullName" HeaderText="نام کامل" />
                            <asp:BoundField DataField="Email" HeaderText="ایمیل" />
                            <asp:CheckBoxField DataField="IsActive" HeaderText="فعال" />
                            <asp:TemplateField HeaderText="عملیات">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEdit" runat="server" 
                                                    CommandName="EditUser" 
                                                    CommandArgument='<%# Eval("UserId") %>'
                                                    Text="✏️ ویرایش" 
                                                    CssClass="btn btn-primary" 
                                                    style="font-size: 12px; padding: 8px 15px;" />
                                    
                                    <asp:LinkButton ID="btnDelete" runat="server" 
                                                    CommandName="DeleteUser" 
                                                    CommandArgument='<%# Eval("UserId") %>'
                                                    Text="🗑️ حذف" 
                                                    CssClass="btn btn-danger" 
                                                    style="font-size: 12px; padding: 8px 15px;"
                                                    OnClientClick="return confirm('آیا مطمئن هستید؟');" />
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