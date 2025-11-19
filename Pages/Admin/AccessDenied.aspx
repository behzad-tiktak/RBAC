<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AccessDenied.aspx.cs" Inherits="Pages_Admin_AccessDenied" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>عدم دسترسی</title>
    <meta charset="utf-8" />
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: Tahoma, Arial, sans-serif;
            background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
            direction: rtl;
            display: flex;
            align-items: center;
            justify-content: center;
            min-height: 100vh;
            padding: 20px;
        }

        .container {
            background: white;
            border-radius: 20px;
            box-shadow: 0 20px 60px rgba(0,0,0,0.3);
            padding: 50px;
            text-align: center;
            max-width: 600px;
            animation: fadeIn 0.5s;
        }

        @keyframes fadeIn {
            from {
                opacity: 0;
                transform: translateY(-30px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        .icon {
            font-size: 120px;
            margin-bottom: 20px;
            animation: bounce 2s infinite;
        }

        @keyframes bounce {
            0%, 100% {
                transform: translateY(0);
            }
            50% {
                transform: translateY(-20px);
            }
        }

        h1 {
            color: #f5576c;
            font-size: 32px;
            margin-bottom: 20px;
        }

        p {
            color: #666;
            font-size: 18px;
            line-height: 1.8;
            margin-bottom: 30px;
        }

        .btn {
            display: inline-block;
            padding: 15px 40px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            text-decoration: none;
            border-radius: 50px;
            font-size: 16px;
            font-weight: bold;
            transition: all 0.3s;
        }

        .btn:hover {
            transform: translateY(-3px);
            box-shadow: 0 10px 30px rgba(102, 126, 234, 0.4);
        }

        .details {
            background: #f8f9fa;
            padding: 20px;
            border-radius: 10px;
            margin: 20px 0;
            text-align: right;
        }

        .details p {
            margin: 10px 0;
            font-size: 14px;
            color: #333;
        }

        .details strong {
            color: #667eea;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="icon">🚫</div>
            <h1>عدم دسترسی</h1>
            <p>
                متأسفانه شما دسترسی به این صفحه یا عملیات را ندارید.<br/>
                لطفاً با مدیر سیستم تماس بگیرید.
            </p>

            <div class="details">
                <p><strong>کاربر:</strong> <%= GetCurrentUsername() %></p>
                <p><strong>صفحه درخواستی:</strong> <%= GetRequestedPage() %></p>
                <p><strong>زمان:</strong> <%= DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") %></p>
            </div>

            <a href="<%= GetHomePageUrl() %>" class="btn">🏠 بازگشت به صفحه اصلی</a>
        </div>
    </form>
    <script runat="server">
        protected string GetCurrentUsername()
        {
            try
            {
                if (Session["Username"] != null)
                    return Session["Username"].ToString();
                    
                return "مهمان";
            }
            catch
            {
                return "نامشخص";
            }
        }

        protected string GetRequestedPage()
        {
            try
            {
                if (Request.UrlReferrer != null)
                    return Request.UrlReferrer.PathAndQuery;
                    
                return "نامشخص";
            }
            catch
            {
                return "نامشخص";
            }
        }

        protected string GetHomePageUrl()
        {
            // می‌توانید بر اساس نقش کاربر، صفحه اصلی را تغییر دهید
            return ResolveUrl("~/Pages/Admin/Login.aspx");
        }
    </script>
</body>
</html>
