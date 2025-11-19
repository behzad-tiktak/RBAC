<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Pages_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../css/all.css" rel="stylesheet" />
    <link href="../css/bootstrap.min.css" rel="stylesheet" />
    <link href="../css/bootstrap.min.rtl.css" rel="stylesheet" />
    <link href="../css/main.css" rel="stylesheet" />
    <link href="../css/fontstyle.css" rel="stylesheet" />
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="row">
                <div class="col-xs-12">
                    <asp:Label runat="server" ID="lbltest"></asp:Label>
            <h1>صفحه مدیریتی</h1>
            <table class="table table-responsive table-condensed table-hover table-bordered">
                <thead>
                    <tr>
                        <th class="text-center">
                            <p>ردیف</p>
                        </th>
                        <th class="text-center">
                            <p>نام کاربری</p>
                        </th>
                        <th class="text-center">
                            <p>نام کامل</p>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="repListUsers" runat="server">
                        
                        <ItemTemplate>
                            <tr class="text-center">
                                <td>
                                    <p>
                                        <asp:Label runat="server" Text='<%# Container.ItemIndex+1 %>'></asp:Label></p>
                                </td>
                                <td>
                                    <p>
                                        <asp:Label runat="server" Text='<%# Eval("Username").ToString() %>'></asp:Label></p>
                                </td>
                                <td>
                                    <p>
                                        <asp:Label runat="server" Text='<%# Eval("FullName").ToString() %>'></asp:Label></p>
                                </td>
                                <td>
                                    <asp:Button ID="btnDeleteUser" runat="server" Text='<%# bool.Parse(Eval("IsActive").ToString())==true ? "غیر فعال شود" : "فعال سازی" %>'    CssClass='<%#  bool.Parse(Eval("IsActive").ToString())==true ? "btn btn-danger" : "btn btn-success" %>'  OnClick="btnDeleteUser_Click" CommandArgument='<%# Eval("UserId") %>' CommandName='<%# Eval("IsActive") %>'  />
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
            <asp:Label ID="lblUser" runat="server"></asp:Label>
                </div>
                </div>
        </div>
    </form>
</body>
</html>
