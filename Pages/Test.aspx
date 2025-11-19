<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Test.aspx.cs" Inherits="Pages_Test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" dir="rtl" lang="fa">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>تست سیستم RBAC</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: Tahoma, Arial, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            padding: 20px;
        }

        .container {
            max-width: 1200px;
            margin: 0 auto;
        }

        .header {
            background: white;
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.2);
            margin-bottom: 30px;
            text-align: center;
        }

        .header h1 {
            color: #667eea;
            font-size: 32px;
            margin-bottom: 10px;
        }

        .header p {
            color: #666;
            font-size: 14px;
        }

        .test-section {
            background: white;
            padding: 25px;
            border-radius: 10px;
            box-shadow: 0 5px 15px rgba(0,0,0,0.1);
            margin-bottom: 20px;
        }

        .test-section h2 {
            color: #667eea;
            font-size: 20px;
            margin-bottom: 15px;
            padding-bottom: 10px;
            border-bottom: 2px solid #f0f0f0;
        }

        .test-button {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border: none;
            padding: 12px 30px;
            border-radius: 5px;
            font-size: 14px;
            cursor: pointer;
            margin: 5px;
            transition: all 0.3s;
        }

        .test-button:hover {
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(102, 126, 234, 0.4);
        }

        .test-button:active {
            transform: translateY(0);
        }

        .result-box {
            background: #f8f9fa;
            border: 1px solid #e0e0e0;
            border-radius: 5px;
            padding: 15px;
            margin-top: 15px;
            min-height: 100px;
            max-height: 400px;
            overflow-y: auto;
            font-family: 'Courier New', monospace;
            font-size: 13px;
            white-space: pre-wrap;
            word-wrap: break-word;
        }

        .success {
            color: #28a745;
            font-weight: bold;
        }

        .error {
            color: #dc3545;
            font-weight: bold;
        }

        .info {
            color: #17a2b8;
        }

        .warning {
            color: #ffc107;
        }

        .stats-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 15px;
            margin-top: 15px;
        }

        .stat-card {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 20px;
            border-radius: 8px;
            text-align: center;
        }

        .stat-card h3 {
            font-size: 32px;
            margin-bottom: 5px;
        }

        .stat-card p {
            font-size: 14px;
            opacity: 0.9;
        }

        .button-group {
            margin: 10px 0;
        }

        .divider {
            height: 1px;
            background: #e0e0e0;
            margin: 20px 0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <!-- Header -->
            <div class="header">
                <h1>🔐 تست سیستم RBAC</h1>
                <p>نسخه سازگار با .NET Framework 4.8.1 | Visual Studio 2013</p>
            </div>

            <!-- Test 1: Database Connection -->
            <div class="test-section">
                <h2>1️⃣ تست اتصال به دیتابیس</h2>
                <div class="button-group">
                    <asp:Button ID="btnTestConnection" runat="server" Text="تست اتصال" 
                        CssClass="test-button" OnClick="btnTestConnection_Click" />
                </div>
                <asp:Literal ID="litConnectionResult" runat="server"></asp:Literal>
            </div>

            <!-- Test 2: User Operations -->
            <div class="test-section">
                <h2>2️⃣ تست عملیات کاربران</h2>
                <div class="button-group">
                    <asp:Button ID="btnTestCreateUser" runat="server" Text="ایجاد کاربر تستی" 
                        CssClass="test-button" OnClick="btnTestCreateUser_Click" />
                    <asp:Button ID="btnTestGetUsers" runat="server" Text="دریافت کاربران" 
                        CssClass="test-button" OnClick="btnTestGetUsers_Click" />
                    <asp:Button ID="btnTestLogin" runat="server" Text="تست Login" 
                        CssClass="test-button" OnClick="btnTestLogin_Click" />
                </div>
                <asp:Literal ID="litUserResult" runat="server"></asp:Literal>
            </div>

            <!-- Test 3: Role Operations -->
            <div class="test-section">
                <h2>3️⃣ تست عملیات نقش‌ها</h2>
                <div class="button-group">
                    <asp:Button ID="btnTestGetRoles" runat="server" Text="دریافت نقش‌ها" 
                        CssClass="test-button" OnClick="btnTestGetRoles_Click" />
                    <asp:Button ID="btnTestAssignRole" runat="server" Text="تخصیص نقش به کاربر" 
                        CssClass="test-button" OnClick="btnTestAssignRole_Click" />
                    <asp:Button ID="btnTestGetUserRoles" runat="server" Text="نقش‌های کاربر" 
                        CssClass="test-button" OnClick="btnTestGetUserRoles_Click" />
                </div>
                <asp:Literal ID="litRoleResult" runat="server"></asp:Literal>
            </div>

            <!-- Test 4: Permission Operations -->
            <div class="test-section">
                <h2>4️⃣ تست عملیات دسترسی‌ها</h2>
                <div class="button-group">
                    <asp:Button ID="btnTestGetPermissions" runat="server" Text="دریافت دسترسی‌ها" 
                        CssClass="test-button" OnClick="btnTestGetPermissions_Click" />
                    <asp:Button ID="btnTestUserPermissions" runat="server" Text="دسترسی‌های کاربر" 
                        CssClass="test-button" OnClick="btnTestUserPermissions_Click" />
                    <asp:Button ID="btnTestCheckPermission" runat="server" Text="بررسی دسترسی خاص" 
                        CssClass="test-button" OnClick="btnTestCheckPermission_Click" />
                </div>
                <asp:Literal ID="litPermissionResult" runat="server"></asp:Literal>
            </div>

            <!-- Test 5: Cache Test -->
            <div class="test-section">
                <h2>5️⃣ تست Cache و Performance</h2>
                <div class="button-group">
                    <asp:Button ID="btnTestCache" runat="server" Text="تست Cache" 
                        CssClass="test-button" OnClick="btnTestCache_Click" />
                    <asp:Button ID="btnClearCache" runat="server" Text="پاک کردن Cache" 
                        CssClass="test-button" OnClick="btnClearCache_Click" />
                </div>
                <asp:Literal ID="litCacheResult" runat="server"></asp:Literal>
            </div>

            <!-- Test 6: Security Helper -->
            <div class="test-section">
                <h2>6️⃣ تست SecurityHelper</h2>
                <div class="button-group">
                    <asp:Button ID="btnTestHash" runat="server" Text="تست Hash کردن" 
                        CssClass="test-button" OnClick="btnTestHash_Click" />
                    <asp:Button ID="btnTestSession" runat="server" Text="تست Session" 
                        CssClass="test-button" OnClick="btnTestSession_Click" />
                    <asp:Button ID="btnTestValidation" runat="server" Text="تست Validation" 
                        CssClass="test-button" OnClick="btnTestValidation_Click" />
                </div>
                <asp:Literal ID="litSecurityResult" runat="server"></asp:Literal>
            </div>

            <!-- Statistics -->
            <div class="test-section">
                <h2>📊 آمار سیستم</h2>
                <asp:Button ID="btnGetStats" runat="server" Text="دریافت آمار" 
                    CssClass="test-button" OnClick="btnGetStats_Click" />
                <asp:Literal ID="litStats" runat="server"></asp:Literal>
            </div>

            <!-- Run All Tests -->
            <div class="test-section" style="text-align: center;">
                <h2>🚀 اجرای تمام تست‌ها</h2>
                <asp:Button ID="btnRunAllTests" runat="server" Text="اجرای تمام تست‌ها (توصیه می‌شود)" 
                    CssClass="test-button" OnClick="btnRunAllTests_Click" 
                    style="font-size: 16px; padding: 15px 40px;" />
            </div>
        </div>
    </form>
</body>
</html>
