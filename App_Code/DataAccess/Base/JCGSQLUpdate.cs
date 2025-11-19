using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for JCGSQLUpdate
/// </summary>
public class JCGSQLUpdate : JCGSQLCommandBase
{
    public JCGSQLUpdate(string query, Dictionary<string, object> parameters, bool requestValue)
    {
        Execute(query, parameters, requestValue);
    }

    public JCGSQLUpdate(string query, bool requestValue)
    {
        ExecuteLegacy(query, requestValue);
    }
}

