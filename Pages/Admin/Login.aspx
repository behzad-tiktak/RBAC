<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Pages_Admin_Login" %>

<!DOCTYPE html>
<html dir="rtl" lang="fa">
<head runat="server">
    <title>ورود به سیستم</title>
    <link href="../../css/fontstyle.css" rel="stylesheet" />
    <meta charset="utf-8" />
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        
        body {
            font-family: Tahoma, Arial, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 20px;
        }
        
        .login-container {
            background: white;
            border-radius: 20px;
            box-shadow: 0 20px 60px rgba(0,0,0,0.3);
            padding: 50px;
            width: 100%;
            max-width: 450px;
            animation: fadeIn 0.5s;
        }
        
        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(-30px); }
            to { opacity: 1; transform: translateY(0); }
        }
        
        .logo {
            text-align: center;
            font-size: 60px;
            margin-bottom: 10px;
        }
        
        h1 {
            text-align: center;
            color: #333;
            margin-bottom: 10px;
            font-size: 28px;
        }
        
        .subtitle {
            text-align: center;
            color: #666;
            margin-bottom: 30px;
            font-size: 14px;
        }
        
        .form-group {
            margin-bottom: 25px;
        }
        
        .form-group label {
            display: block;
            margin-bottom: 8px;
            color: #333;
            font-weight: bold;
            font-size: 14px;
        }
        
        .input-wrapper {
            position: relative;
        }
        
        .input-wrapper span {
            position: absolute;
            right: 15px;
            top: 50%;
            transform: translateY(-50%);
            font-size: 18px;
        }
        
        .form-control {
            width: 100%;
            padding: 15px 50px 15px 15px;
            border: 2px solid #e0e0e0;
            border-radius: 10px;
            font-size: 14px;
            transition: all 0.3s;
        }
        
        .form-control:focus {
            outline: none;
            border-color: #667eea;
        }
        
        .btn-login {
            width: 100%;
            padding: 15px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border: none;
            border-radius: 10px;
            font-size: 16px;
            font-weight: bold;
            cursor: pointer;
            transition: all 0.3s;
        }
        
        .btn-login:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 30px rgba(102, 126, 234, 0.4);
        }
        
        .alert {
            padding: 15px;
            border-radius: 10px;
            margin-bottom: 20px;
            text-align: center;
        }
        
        .alert-error {
            background: #fee;
            color: #c33;
            border: 1px solid #fcc;
        }
        
        .alert-success {
            background: #efe;
            color: #3c3;
            border: 1px solid #cfc;
        }
        
        .remember-me {
            margin-top: 15px;
            display: flex;
            align-items: center;
            gap: 8px;
        }
        
        .footer {
            margin-top: 30px;
            text-align: center;
            color: #999;
            font-size: 12px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container">
            <div class="logo">🔐</div>
            <h1>ورود به سیستم</h1>
            <p class="subtitle">سیستم مدیریت دسترسی RBAC</p>
            
            <asp:Panel ID="pnlMessage" runat="server" Visible="false">
                <asp:Label ID="lblMessage" runat="server" CssClass="alert"></asp:Label>
            </asp:Panel>
            
            <div class="form-group">
                <label>نام کاربری:</label>
                <div class="input-wrapper">
                    <span>👤</span>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" 
                                 placeholder="نام کاربری خود را وارد کنید"></asp:TextBox>
                </div>
            </div>
            
            <div class="form-group">
                <label>رمز عبور:</label>
                <div class="input-wrapper">
                    <span>🔒</span>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" 
                                 TextMode="Password" placeholder="رمز عبور خود را وارد کنید"></asp:TextBox>
                </div>
            </div>
            
            <div class="remember-me">
                <asp:CheckBox ID="chkRememberMe" runat="server" />
                <label for="chkRememberMe">مرا به خاطر بسپار</label>
            </div>
            
            <div class="form-group" style="margin-top: 30px;">
                <asp:Button ID="btnLogin" runat="server" Text="ورود" 
                            CssClass="btn-login" OnClick="btnLogin_Click" />
            </div>
            
            <div class="footer">
                سیستم RBAC - نسخه 1.0
            </div>
        </div>
    </form>
</body>
</html>