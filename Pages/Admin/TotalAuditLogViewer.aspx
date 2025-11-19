<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TotalAuditLogViewer.aspx.cs" Inherits="Pages_Admin_TotalAuditLogViewer" %>


<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>مشاهده لاگ‌های سیستم</title>
    <link href="../../css/all.css" rel="stylesheet" />
    <link href="../../css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../css/bootstrap.min.rtl.css" rel="stylesheet" />
    <link href="../../css/fontstyle.css" rel="stylesheet" />
    <script src="../../js/jquery-3.2.1.min.js"></script>
    <script src="../../js/bootstrap.min.js"></script>
    <!-- Bootstrap 3 CSS -->
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" />

    <style>
        body {
            font-family: Tahoma, Arial;
            direction: rtl;
            text-align: right;
            padding: 20px;
            background: #f5f5f5;
        }
        
        .page-header {
            border-bottom: 3px solid #337ab7;
            margin-bottom: 20px;
        }
        
        .filter-panel {
            background: #fff;
            padding: 20px;
            border-radius: 4px;
            margin-bottom: 20px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.1);
        }
        
        .filter-row {
            margin-bottom: 15px;
        }
        
        .filter-row label {
            font-weight: bold;
            margin-left: 10px;
        }
        
        /* GridView Styles */
        .table-responsive {
            background: white;
            padding: 15px;
            border-radius: 4px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.1);
        }
        
        .audit-grid {
            margin-bottom: 0;
        }
        
        .audit-grid thead {
            background: #337ab7;
            color: white;
        }
        
        .audit-grid thead th {
            text-align: center;
            vertical-align: middle;
        }
        
        .audit-grid tbody td {
            text-align: center;
            vertical-align: middle;
        }
        
        .action-insert {
            color: #5cb85c;
            font-weight: bold;
        }
        
        .action-update {
            color: #5bc0de;
            font-weight: bold;
        }
        
        .action-delete {
            color: #d9534f;
            font-weight: bold;
        }
        
        .action-login {
            color: #f0ad4e;
            font-weight: bold;
        }
        
        /* Modal Customization */
        .modal-header {
            background: #337ab7;
            color: white;
            border-radius: 6px 6px 0 0;
        }
        
        .modal-header .close {
            color: white;
            opacity: 0.8;
        }
        
        .modal-header .close:hover {
            opacity: 1;
        }
        
        .modal-title {
            font-weight: bold;
        }
        
        .detail-group {
            margin-bottom: 20px;
        }
        
        .detail-group .panel-heading {
            background: #f5f5f5;
            font-weight: bold;
            color: #333;
        }
        
        .detail-table {
            margin-bottom: 0;
        }
        
        .detail-table td {
            padding: 10px;
            border-top: 1px solid #ddd;
        }
        
        .detail-label {
            font-weight: bold;
            width: 180px;
            background: #f9f9f9;
            color: #555;
        }
        
        .detail-value {
            color: #333;
        }
        
        .code-block {
            background: #f5f5f5;
            border: 1px solid #ddd;
            border-radius: 4px;
            padding: 15px;
            max-height: 300px;
            overflow-y: auto;
            direction: ltr;
            text-align: left;
            font-family: 'Courier New', monospace;
            font-size: 12px;
            white-space: pre-wrap;
            word-wrap: break-word;
        }
        
        .badge-action {
            font-size: 12px;
            padding: 5px 10px;
        }
        
        .no-data {
            text-align: center;
            padding: 30px;
            color: #999;
            font-size: 16px;
        }
        
        .icon-prefix {
            margin-left: 5px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server" />
        
        <div class="container-fluid">
            <div class="page-header">
                <h2><span class="fa fa-list-alt"></span> مشاهده لاگ‌های سیستم</h2>
            </div>
            
            <!-- Filter Panel -->
            <div class="filter-panel">
                <h4 class="text-primary">
                    <span class="fa fa-filter"></span> فیلترها
                </h4>
                <hr />
                
                <div class="row filter-row">
                    <div class="col-md-3">
                        <label>نام کاربری:</label>
                        <asp:TextBox ID="txtSearchUsername" runat="server" CssClass="form-control" 
                            placeholder="نام کاربری را وارد کنید..." />
                    </div>
                    
                    <div class="col-md-3">
                        <label>عملیات:</label>
                        <asp:DropDownList ID="ddlAction" runat="server" CssClass="form-control">
                            <asp:ListItem Value="All" Text="همه"></asp:ListItem>
                            <asp:ListItem Value="Insert" Text="افزودن"></asp:ListItem>
                            <asp:ListItem Value="Update" Text="ویرایش"></asp:ListItem>
                            <asp:ListItem Value="Delete" Text="حذف"></asp:ListItem>
                            <asp:ListItem Value="Login" Text="ورود"></asp:ListItem>
                            <asp:ListItem Value="Logout" Text="خروج"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    
                    <div class="col-md-2">
                        <label>از تاریخ:</label>
                        <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" CssClass="form-control" />
                    </div>
                    
                    <div class="col-md-2">
                        <label>تا تاریخ:</label>
                        <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" CssClass="form-control" />
                    </div>
                    
                    <div class="col-md-2" style="padding-top: 25px;">
                        <asp:Button ID="btnSearch" runat="server" Text="جستجو" 
                            CssClass="btn btn-primary btn-block" OnClick="btnSearch_Click">
                            <%--<span class="fa fa-search"></span>--%>
                        </asp:Button>
                    </div>
                </div>
                
                <div class="row">
                    <div class="col-md-12">
                        <asp:Button ID="btnClearSearch" runat="server" Text="پاک کردن فیلترها" 
                            CssClass="btn btn-default" OnClick="btnClearSearch_Click" />
                    </div>
                </div>
            </div>
            
            <!-- GridView -->
            <div class="table-responsive">
                <asp:GridView ID="gvAuditLogs" runat="server" 
                    CssClass="table table-striped table-bordered table-hover audit-grid"
                    AutoGenerateColumns="False" 
                    DataKeyNames="LogId"
                    AllowPaging="True" 
                    PageSize="20"
                    OnRowDataBound="gvAuditLogs_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="LogId" HeaderText="شناسه" />
                        <asp:BoundField DataField="Username" HeaderText="کاربر" />
                        <asp:BoundField DataField="Action" HeaderText="عملیات" />
                        <asp:BoundField DataField="TableName" HeaderText="جدول" />
                        <asp:BoundField DataField="RecordId" HeaderText="شناسه رکورد" />
                        <asp:BoundField DataField="ActionDate" HeaderText="تاریخ و زمان" 
                            DataFormatString="{0:yyyy/MM/dd HH:mm:ss}" />
                        <asp:BoundField DataField="IpAddress" HeaderText="IP" />
                        <asp:TemplateField HeaderText="عملیات">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnDetails" runat="server" 
                                    CssClass="btn btn-info btn-sm"
                                    OnClick="btnDetails_Click">
                                    <%--<span class="fa fa-eye-open"></span>--%> جزئیات
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="no-data">
                            <span class="fa fa-info-sign" style="font-size: 48px;"></span>
                            <p>هیچ لاگی یافت نشد</p>
                        </div>
                    </EmptyDataTemplate>
                    <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                </asp:GridView>
            </div>
        </div>
        
        <!-- Bootstrap Modal -->
        <div class="modal fade" id="detailsModal" tabindex="-1" role="dialog" aria-labelledby="detailsModalLabel">
            <div class="modal-dialog modal-lg" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                        <h4 class="modal-title" id="detailsModalLabel">
                            <span class="fa fa-zoom-in icon-prefix"></span>
                            جزئیات کامل لاگ
                        </h4>
                    </div>
                    
                    <div class="modal-body">
                        <!-- اطلاعات اصلی -->
                        <div class="detail-group">
                            <div class="panel panel-primary">
                                <div class="panel-heading">
                                    <span class="fa fa-info-sign icon-prefix"></span>
                                    اطلاعات اصلی
                                </div>
                                <div class="panel-body" style="padding: 0;">
                                    <table class="table detail-table">
                                        <tr>
                                            <td class="detail-label">
                                                <span class="fa fa-tag icon-prefix"></span>
                                                شناسه لاگ:
                                            </td>
                                            <td class="detail-value">
                                                <asp:Label ID="lblModalLogId" runat="server" CssClass="label label-default" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="detail-label">
                                                <span class="fa fa-user icon-prefix"></span>
                                                کاربر:
                                            </td>
                                            <td class="detail-value">
                                                <asp:Label ID="lblModalUsername" runat="server" CssClass="label label-info" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="detail-label">
                                                <span class="fa fa-flash icon-prefix"></span>
                                                عملیات:
                                            </td>
                                            <td class="detail-value">
                                                <asp:Label ID="lblModalAction" runat="server" CssClass="label label-primary badge-action" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="detail-label">
                                                <span class="fa fa-th icon-prefix"></span>
                                                جدول:
                                            </td>
                                            <td class="detail-value">
                                                <asp:Label ID="lblModalTable" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="detail-label">
                                                <span class="fa fa-barcode icon-prefix"></span>
                                                شناسه رکورد:
                                            </td>
                                            <td class="detail-value">
                                                <asp:Label ID="lblModalRecordId" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="detail-label">
                                                <span class="fa fa-calendar icon-prefix"></span>
                                                تاریخ و زمان:
                                            </td>
                                            <td class="detail-value">
                                                <asp:Label ID="lblModalDate" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="detail-label">
                                                <span class="fa fa-globe icon-prefix"></span>
                                                آدرس IP:
                                            </td>
                                            <td class="detail-value">
                                                <asp:Label ID="lblModalIp" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="detail-label">
                                                <span class="fa fa-phone icon-prefix"></span>
                                                مرورگر (User Agent):
                                            </td>
                                            <td class="detail-value">
                                                <small><asp:Label ID="lblModalUserAgent" runat="server" /></small>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </div>
                        
                        <!-- مقدار قبلی -->
                        <div class="detail-group">
                            <div class="panel panel-warning">
                                <div class="panel-heading">
                                    <span class="fa fa-backward icon-prefix"></span>
                                    مقدار قبلی (Old Value)
                                </div>
                                <div class="panel-body">
                                    <div class="code-block">
                                        <asp:Label ID="lblOldValue" runat="server" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        
                        <!-- مقدار جدید -->
                        <div class="detail-group">
                            <div class="panel panel-success">
                                <div class="panel-heading">
                                    <span class="fa fa-forward icon-prefix"></span>
                                    مقدار جدید (New Value)
                                </div>
                                <div class="panel-body">
                                    <div class="code-block">
                                        <asp:Label ID="lblNewValue" runat="server" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal">
                            <span class="fa fa-remove"></span> بستن
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </form>
    
    <!-- jQuery -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
    <!-- Bootstrap 3 JS -->
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
    
    <script type="text/javascript">
        function showDetailsModal() {
            $('#detailsModal').modal('show');
        }
    </script>
</body>
</html>