using System;
using System.Configuration;

/// <summary>
/// کلاس مدیریت ارتباط با بانک اطلاعاتی
/// </summary>
public static class BankLink
{
    /// <summary>
    /// دریافت Connection String از Web.config
    /// IMPORTANT: هرگز رمز عبور را در کد ننویسید!
    /// </summary>
    public static string GetConnectionString()
    {
        try
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myConnectionString"];
            
            if (connectionString == null || string.IsNullOrEmpty(connectionString.ConnectionString))
            {
                throw new InvalidOperationException("Connection string 'myConnectionString' not found in configuration.");
            }
            
            return connectionString.ConnectionString;
        }
        catch (Exception ex)
        {
            // لاگ کردن خطا
            LogError("خطا در دریافت Connection String", ex);
            throw;
        }
    }

    /// <summary>
    /// متد برای لاگ کردن خطاها
    /// می‌توانید این متد را به Event Log، File، یا سیستم لاگ خود متصل کنید
    /// </summary>
    private static void LogError(string message, Exception ex)
    {
        // TODO: اتصال به سیستم لاگ
        // مثال: File.AppendAllText("errors.log", $"{DateTime.Now}: {message} - {ex.Message}\n");
        System.Diagnostics.Debug.WriteLine("[ERROR] {message}: {ex.Message}");
    }
}

/*
 * نکات مهم برای Web.config:
 * 
 * <connectionStrings>
 *   <add name="myConnectionString" 
 *        connectionString="Data Source=.;Initial Catalog=myDB;Integrated Security=True;" 
 *        providerName="System.Data.SqlClient" />
 * </connectionStrings>
 * 
 * برای Production از Integrated Security یا Azure Key Vault استفاده کنید
 * هرگز رمز عبور را در کد ننویسید!
 */