using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for JCGSQLInsert
/// </summary>
public class JCGSQLInsert : JCGSQLCommandBase
{
    public JCGSQLInsert(string query, Dictionary<string, object> parameters, bool requestValue)
    {
        Execute(query, parameters, requestValue);
    }

    public JCGSQLInsert(string query, bool requestValue)
    {
        ExecuteLegacy(query, requestValue);
    }
}

