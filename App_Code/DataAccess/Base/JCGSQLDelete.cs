using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for JCGSQLDelete
/// </summary>
public class JCGSQLDelete : JCGSQLCommandBase
{
    public JCGSQLDelete(string query, Dictionary<string, object> parameters, bool requestValue)
    {
        Execute(query, parameters, requestValue);
    }

    public JCGSQLDelete(string query, bool requestValue)
    {
        ExecuteLegacy(query, requestValue);
    }
}

