using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;

public abstract class JCGSQLCommandBase
{
    protected string command;
    protected bool flag;
    protected string returnValue;

    public string ReturnValue
    {
        get { return returnValue; }
    }

    public bool Status()
    {
        return flag;
    }

    protected void Execute(string query, Dictionary<string, object> parameters, bool requestValue)
    {
        command = query;

        using (SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
        using (SqlCommand com = new SqlCommand(command, connect))
        {
            com.CommandType = CommandType.Text;

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    object value = param.Value;
                    if (value is string)
                        value = StandardCharacter.Convert((string)value);

                    com.Parameters.AddWithValue(param.Key, value ?? DBNull.Value);
                }
            }

            try
            {
                connect.Open();
                if (requestValue)
                    returnValue = Convert.ToString(com.ExecuteScalar());
                else
                    com.ExecuteNonQuery();

                flag = true;
            }
            catch
            {
                flag = false;
            }
        }
    }

    protected void ExecuteLegacy(string query, bool requestValue)
    {
        command = StandardCharacter.Convert(query);

        using (SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
        using (SqlCommand com = new SqlCommand(command, connect))
        {
            try
            {
                connect.Open();
                if (requestValue)
                    returnValue = Convert.ToString(com.ExecuteScalar());
                else
                    com.ExecuteNonQuery();

                flag = true;
            }
            catch
            {
                flag = false;
            }
        }
    }
}
