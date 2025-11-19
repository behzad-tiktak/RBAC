using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for StandardCharacter
/// </summary>
public static class StandardCharacter
{
    public static string Convert(string inputString)
    {
        inputString = inputString.Replace("۰", "0");
        inputString = inputString.Replace("۱", "1");
        inputString = inputString.Replace("۲", "2");
        inputString = inputString.Replace("۳", "3");
        inputString = inputString.Replace("۴", "4");
        inputString = inputString.Replace("۵", "5");
        inputString = inputString.Replace("۶", "6");
        inputString = inputString.Replace("۷", "7");
        inputString = inputString.Replace("۸", "8");
        inputString = inputString.Replace("۹", "9");
        inputString = inputString.Replace("ي", "ی");
        return inputString;
    }
}