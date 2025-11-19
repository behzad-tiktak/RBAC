<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AuditLogViewer.aspx.cs" Inherits="Pages_Admin_AuditLogViewer" %>

<!DOCTYPE html>
<html dir="rtl" lang="fa">
<head runat="server">
    <title>گزارش لاگ‌های سیستم</title>
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
            max-width: 1600px;
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
        
        .filters {
            background: #f8f9fa;
            padding: 20px;
            border-radius: 10px;
            margin-bottom: 30px;
        }
        
        .filter-row {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 15px;
            margin-bottom: 15px;
        }
        
        .form-group label {
            display: block;
            margin-bottom: 5px;
            font-weight: bold;
            font-size: 13px;
        }
        
        .form-control {
            width: 100%;
            padding: 10px;
            border: 2px solid #ddd;
            border-radius: 5px;
        }
        
        .btn {
            padding: 10px 25px;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-weight: bold;
            margin-left: 10px;
        }
        
        .btn-primary {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
        }
        
        .stats {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }
        
        .stat-card {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 20px;
            border-radius: 10px;
            text-align: center;
        }
        
        .stat-number {
            font-size: 36px;
            font-weight: bold;
            margin: 10px 0;
        }
        
        .GridView {
            width: 100%;
            border-collapse: collapse;
            background: white;
            font-size: 13px;
        }
        
        .GridView th {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 12px;
            text-align: center;
            position: sticky;
            top: 0;
        }
        
        .GridView td {
            padding: 10px;
            border-bottom: 1px solid #ddd;
            text-align: center;
        }
        
        .GridView tr:hover {
            background: #f8f9fa;
        }
        
        .action-insert { background: #d4edda; }
        .action-update { background: #fff3cd; }
        .action-delete { background: #f8d7da; }
        .action-login { background: #d1ecf1; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header">
                <h1>📊 گزارش لاگ‌های سیستم</h1>
                <p>مشاهده تمام فعالیت‌های کاربران</p>
            </div>

            <div class="content">
                <!-- آمار -->
                <div class="stats">
                    <div class="stat-card">
                        <div>📝 تعداد کل لاگ‌ها</div>
                        <div class="stat-number">
                            <asp:Label ID="lblTotalLogs" runat="server" Text="0"></asp:Label>
                        </div>
                    </div>
                    <div class="stat-card">
                        <div>👤 کاربران فعال</div>
                        <div class="stat-number">
                            <asp:Label ID="lblActiveUsers" runat="server" Text="0"></asp:Label>
                        </div>
                    </div>
                    <div class="stat-card">
                        <div>📅 امروز</div>
                        <div class="stat-number">
                            <asp:Label ID="lblTodayLogs" runat="server" Text="0"></asp:Label>
                        </div>
                    </div>
                </div>

                <!-- فیلتر -->
                <div class="filters">
                    <div class="filter-row">
                        <div class="form-group">
                            <label>کاربر:</label>
                            <asp:DropDownList ID="ddlUsers" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <label>نوع عملیات:</label>
                            <asp:DropDownList ID="ddlAction" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">همه</asp:ListItem>
                                <asp:ListItem Value="Login">Login</asp:ListItem>
                                <asp:ListItem Value="Logout">Logout</asp:ListItem>
                                <asp:ListItem Value="Insert">Insert</asp:ListItem>
                                <asp:ListItem Value="Update">Update</asp:ListItem>
                                <asp:ListItem Value="Delete">Delete</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <label>جدول:</label>
                            <asp:DropDownList ID="ddlTable" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <label>تعداد نمایش:</label>
                            <asp:DropDownList ID="ddlCount" runat="server" CssClass="form-control">
                                <asp:ListItem Value="50">50 مورد</asp:ListItem>
                                <asp:ListItem Value="100" Selected="True">100 مورد</asp:ListItem>
                                <asp:ListItem Value="200">200 مورد</asp:ListItem>
                                <asp:ListItem Value="500">500 مورد</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div style="margin-top: 15px;">
                        <asp:Button ID="btnFilter" runat="server" Text="🔍 اعمال فیلتر" 
                                    CssClass="btn btn-primary" OnClick="btnFilter_Click" />
                        
                        <asp:Button ID="btnReset" runat="server" Text="🔄 بازنشانی" 
                                    CssClass="btn btn-primary" OnClick="btnReset_Click" />
                    </div>
                </div>

                <!-- جدول لاگ‌ها -->
                <div style="max-height: 600px; overflow-y: auto;">
                    <asp:GridView ID="gvLogs" runat="server" AutoGenerateColumns="False" CssClass="GridView">
                        <Columns>
                            <asp:BoundField DataField="LogId" HeaderText="شناسه" />
                            <asp:BoundField DataField="Username" HeaderText="کاربر" />
                            <asp:BoundField DataField="Action" HeaderText="عملیات" />
                            <asp:BoundField DataField="TableName" HeaderText="جدول" />
                            <asp:BoundField DataField="RecordId" HeaderText="شناسه رکورد" />
                            <asp:BoundField DataField="ActionDate" HeaderText="تاریخ" DataFormatString="{0:yyyy/MM/dd HH:mm:ss}" />
                            <asp:BoundField DataField="IpAddress" HeaderText="IP" />
                            <asp:TemplateField HeaderText="جزئیات">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDetails" runat="server" 
                                                    CommandName="ViewDetails" 
                                                    CommandArgument='<%# Eval("LogId") %>'
                                                    Text="👁️ مشاهده" 
                                                    style="color: #667eea; font-weight: bold;" />
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
