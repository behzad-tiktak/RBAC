using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;

public class JCGSQLSelect : IDisposable
{
    private SqlDataReader reader;
    private DataTable dataTable;
    private SqlConnection connection;
    private bool useDataTable;
    private bool useDataReader;
    private bool flag;
    /// <summary>
    /// Query Include of User Input
    /// </summary>
    /// <param name="query">SQL Query With Parameters</param>
    /// <param name="parameters">DIctionary<string,object></param>
    /// <param name="useDataTable">If Returen DataTable Result</param>
    /// <param name="useDataReader">If Returen DaraReader Result</param>
    public JCGSQLSelect(string query, Dictionary<string, object> parameters, bool useDataTable, bool useDataReader)
    {
        if (useDataTable == useDataReader)
            throw new ArgumentException("یکی از useDataTable یا useDataReader باید True باشد");

        this.useDataTable = useDataTable;
        this.useDataReader = useDataReader;

        ExecuteSelect(query, parameters);
    }
    /// <summary>
    ///  Query not Use Any Input of User Input
    /// </summary>
    /// <param name="query">SQL Query With Parameters</param>    
    /// <param name="useDataTable">If Returen DataTable Result</param>
    /// <param name="useDataReader">If Returen DaraReader Result</param>
    public JCGSQLSelect(string query, bool useDataAdapter, bool useDataReader)
    {
        if (useDataAdapter == useDataReader)
            throw new ArgumentException("یکی از useDataAdapter یا useDataReader باید True باشد");

        this.useDataTable = useDataAdapter;
        this.useDataReader = useDataReader;

        ExecuteSelectLegacy(query);
    }

    private void ExecuteSelect(string query, Dictionary<string, object> parameters)
    {
        try
        {

            //using (connection = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            //using (SqlCommand cmd = new SqlCommand(query, connection))
            //{
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.CommandType = CommandType.Text;
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        object value = param.Value;

                        // اگر مقدار عربی در رشته‌ای بود، اصلاح کن
                        if (value is string)
                        {
                            value = StandardCharacter.Convert((string)value);
                        }                        
                        cmd.Parameters.AddWithValue(param.Key, value ?? DBNull.Value);
                    }
                }

                connection.Open();

                if (useDataTable)
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    flag = dataTable.Rows.Count > 0;
                    connection.Close();
                }
                else if (useDataReader)
                {
                    reader = cmd.ExecuteReader();
                    flag = reader.HasRows;
                    if (flag) reader.Read();
                }
            //}

        }
        catch
        {
            flag = false;
            // For Fial Log
            // LogError("Select", ex); 
            if (connection != null && connection.State == ConnectionState.Open)
                connection.Close();
        }
    }

    private void ExecuteSelectLegacy(string query)
    {
        try
        {
            string command = StandardCharacter.Convert(query);
            //using (connection = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            //{
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
                if (useDataTable)
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command, connection);
                    dataTable = new DataTable();
                    connection.Open();
                    adapter.Fill(dataTable);
                    flag = dataTable.Rows.Count > 0;
                    connection.Close();
                }
                else if (useDataReader)
                {
                    SqlCommand cmd = new SqlCommand(command, connection);
                    connection.Open();
                    reader = cmd.ExecuteReader();
                    flag = reader.HasRows;
                    if (flag) reader.Read();
                }
            //}
        }
        catch
        {
            flag = false;
            // For Fial Log
            // LogError("Select", ex); 
            if (connection != null && connection.State == ConnectionState.Open)
                connection.Close();
        }
    }

    public bool Status()
    {
        return flag;
    }

    public SqlDataReader GetDataReader()
    {
        if (!useDataReader)
            throw new InvalidOperationException("این شیء برای DataTable ساخته شده است");
        return reader;
    }

    public DataTable GetDataTable()
    {
        if (!useDataTable)
            throw new InvalidOperationException("این شیء برای DataReader ساخته شده است");
        return dataTable;
    }

    public void CloseConnection()
    {
        if (reader != null && !reader.IsClosed)
            reader.Close();

        if (connection != null && connection.State == ConnectionState.Open)
            connection.Close();
    }

    public void Dispose()
    {
        if (reader != null)
            reader.Dispose();

        if (dataTable != null)
            dataTable.Dispose();

        if (connection != null)
            connection.Dispose();
    }
}
