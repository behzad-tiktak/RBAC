<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GroupManagement.aspx.cs" Inherits="Pages_Admin_GroupManagement" %>

<!DOCTYPE html>
<html dir="rtl" lang="fa">
<head runat="server">
    <title>مدیریت گروه‌های کاربری</title>
    <link href="../../css/fontstyle.css" rel="stylesheet" />
    
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
            font-size: 14px;
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
            transition: all 0.3s;
            font-weight: bold;
            margin-left: 10px;
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
            background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
            color: white;
        }
        
        .btn-success:hover {
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(79, 172, 254, 0.4);
        }
        
        .btn-danger {
            background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
            color: white;
        }
        
        .btn-danger:hover {
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(245, 87, 108, 0.4);
        }
        
        .alert {
            padding: 15px;
            border-radius: 8px;
            margin-bottom: 20px;
            font-size: 14px;
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
            margin-top: 15px;
            background: white;
        }
        
        .GridView th {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 15px;
            text-align: center;
            font-weight: bold;
        }
        
        .GridView td {
            padding: 12px;
            border-bottom: 1px solid #ddd;
            text-align: center;
        }
        
        .GridView tr:hover {
            background: #f8f9fa;
        }
        
        .members-box {
            border: 2px solid #ddd;
            border-radius: 8px;
            padding: 15px;
            max-height: 400px;
            overflow-y: auto;
            background: white;
        }
        
        .member-item {
            padding: 10px;
            border-bottom: 1px solid #eee;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        
        .member-item:last-child {
            border-bottom: none;
        }
        
        .member-info {
            display: flex;
            align-items: center;
            gap: 10px;
        }
        
        .member-icon {
            width: 40px;
            height: 40px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
            font-weight: bold;
        }
        
        .checkbox-list {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
            gap: 10px;
            max-height: 300px;
            overflow-y: auto;
            padding: 15px;
            background: white;
            border: 2px solid #ddd;
            border-radius: 8px;
        }
        
        .checkbox-item {
            padding: 10px;
            border: 1px solid #eee;
            border-radius: 5px;
            transition: all 0.3s;
        }
        
        .checkbox-item:hover {
            background: #f8f9fa;
            border-color: #667eea;
        }
        
        .empty-state {
            text-align: center;
            padding: 40px;
            color: #999;
        }
        
        .empty-state-icon {
            font-size: 60px;
            margin-bottom: 15px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header">
                <h1>👥 مدیریت گروه‌های کاربری</h1>
                <p>ایجاد، ویرایش و مدیریت اعضای گروه‌ها</p>
            </div>
            <p><a href="../AdminTest.aspx" >صفحه مدیریت مرکزی</a></p>
            <div class="content">
                <!-- پیام‌ها -->
                <asp:Panel ID="pnlMessage" runat="server" Visible="false">
                    <asp:Label ID="lblMessage" runat="server" CssClass="alert"></asp:Label>
                </asp:Panel>

                <!-- بخش 1: ایجاد/ویرایش گروه -->
                <div class="section">
                    <div class="section-title">📝 ایجاد/ویرایش گروه</div>
                    
                    <div class="form-row">
                        <div class="form-group">
                            <label>نام گروه:</label>
                            <asp:TextBox ID="txtGroupName" runat="server" CssClass="form-control" 
                                         placeholder="مثلاً: بخش فنی" MaxLength="100"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label>توضیحات:</label>
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" 
                                         placeholder="توضیحات گروه" TextMode="MultiLine" Rows="1"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:CheckBox ID="chkIsActive" runat="server" Text="  فعال" Checked="true" />
                    </div>

                    <div style="margin-top: 20px;">
                        <asp:Button ID="btnSaveGroup" runat="server" Text="💾 ذخیره گروه" 
                                    CssClass="btn btn-primary" OnClick="btnSaveGroup_Click" />
                        
                        <asp:Button ID="btnCancelEdit" runat="server" Text="❌ انصراف" 
                                    CssClass="btn btn-danger" OnClick="btnCancelEdit_Click" Visible="false" />
                    </div>
                    
                    <asp:HiddenField ID="hfEditGroupId" runat="server" Value="0" />
                </div>

                <!-- بخش 2: مدیریت اعضای گروه -->
                <div class="section">
                    <div class="section-title">👥 مدیریت اعضای گروه</div>
                    
                    <div class="form-group">
                        <label>انتخاب گروه:</label>
                        <asp:DropDownList ID="ddlGroupsForMembers" runat="server" CssClass="form-control" 
                                          AutoPostBack="true" OnSelectedIndexChanged="ddlGroupsForMembers_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>

                    <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 20px; margin-top: 20px;">
                        <!-- لیست کاربران موجود -->
                        <div>
                            <label style="font-weight: bold; margin-bottom: 10px; display: block;">
                                کاربران موجود (برای افزودن انتخاب کنید):
                            </label>
                            <asp:CheckBoxList ID="cblAvailableUsers" runat="server" CssClass="checkbox-list">
                            </asp:CheckBoxList>
                            
                            <div style="margin-top: 15px;">
                                <asp:Button ID="btnAddMembers" runat="server" Text="➡️ افزودن به گروه" 
                                            CssClass="btn btn-success" OnClick="btnAddMembers_Click" />
                            </div>
                        </div>

                        <!-- اعضای فعلی گروه -->
                        <div>
                            <label style="font-weight: bold; margin-bottom: 10px; display: block;">
                                اعضای فعلی گروه:
                            </label>
                            <div class="members-box">
                                <asp:Repeater ID="rptGroupMembers" runat="server" OnItemCommand="rptGroupMembers_ItemCommand">
                                    <ItemTemplate>
                                        <div class="member-item">
                                            <div class="member-info">
                                                <div class="member-icon">
                                                    <%# Eval("FullName").ToString().Substring(0, 1) %>
                                                </div>
                                                <div>
                                                    <strong><%# Eval("FullName") %></strong><br />
                                                    <small style="color: #666;"><%# Eval("Username") %></small>
                                                </div>
                                            </div>
                                            <asp:LinkButton ID="btnRemoveMember" runat="server" 
                                                            CommandName="RemoveMember" 
                                                            CommandArgument='<%# Eval("UserId") %>'
                                                            Text="🗑️ حذف" 
                                                            OnClientClick="return confirm('آیا مطمئن هستید؟');"
                                                            style="color: #f5576c; font-weight: bold; text-decoration: none;" />
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                                
                                <asp:Panel ID="pnlNoMembers" runat="server" Visible="false" CssClass="empty-state">
                                    <div class="empty-state-icon">📭</div>
                                    <p>این گروه هنوز عضوی ندارد</p>
                                </asp:Panel>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- بخش 3: تخصیص آبشاری نقش -->
                <div class="section">
                    <div class="section-title">🔄 تخصیص آبشاری نقش به اعضا</div>
                    
                    <div class="form-row">
                        <div class="form-group">
                            <label>انتخاب گروه:</label>
                            <asp:DropDownList ID="ddlGroupsForRole" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <label>انتخاب نقش:</label>
                            <asp:DropDownList ID="ddlRolesForGroup" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div style="margin-top: 20px;">
                        <asp:Button ID="btnAssignRoleToGroup" runat="server" 
                                    Text="✅ تخصیص نقش به تمام اعضا" 
                                    CssClass="btn btn-success" 
                                    OnClick="btnAssignRoleToGroup_Click" />
                        
                        <asp:Button ID="btnRemoveRoleFromGroup" runat="server" 
                                    Text="❌ حذف نقش از تمام اعضا" 
                                    CssClass="btn btn-danger" 
                                    OnClick="btnRemoveRoleFromGroup_Click"
                                    OnClientClick="return confirm('آیا مطمئن هستید؟');" />
                    </div>
                </div>

                <!-- بخش 4: لیست گروه‌ها -->
                <div class="section">
                    <div class="section-title">📋 لیست گروه‌ها</div>
                    
                    <asp:GridView ID="gvGroups" runat="server" AutoGenerateColumns="False" 
                                  CssClass="GridView" OnRowCommand="gvGroups_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="GroupId" HeaderText="شناسه" />
                            <asp:BoundField DataField="GroupName" HeaderText="نام گروه" />
                            <asp:BoundField DataField="Description" HeaderText="توضیحات" />
                            <asp:BoundField DataField="MemberCount" HeaderText="تعداد اعضا" />
                            <asp:CheckBoxField DataField="IsActive" HeaderText="فعال" />
                            <asp:TemplateField HeaderText="عملیات">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEdit" runat="server" 
                                                    CommandName="EditGroup" 
                                                    CommandArgument='<%# Eval("GroupId") %>'
                                                    Text="✏️ ویرایش" 
                                                    CssClass="btn btn-primary" 
                                                    style="font-size: 12px; padding: 8px 15px;" />
                                    
                                    <asp:LinkButton ID="btnDelete" runat="server" 
                                                    CommandName="DeleteGroup" 
                                                    CommandArgument='<%# Eval("GroupId") %>'
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
