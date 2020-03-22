using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ExtensionMethods
{
    /// <summary>
    /// Át lehetne nevezni DatabaseExtensions-re
    /// </summary>
    public static class GisExtensions
    {
        public static int DbObjectToInteger(this object Val)
        {
            if (Val != null && Val != DBNull.Value)
            {
                return Convert.ToInt32(Val);
            }
            else
            {
                return 0;
            }
        }
        public static string DbObjectToString(this object Val)
        {
            if (Val != null && Val != DBNull.Value)
            {
                return Val as string;
            }
            else
            {
                return string.Empty;
            }
        }
        public static decimal DbObjectToDecimal(this object Val)
        {
            if (Val != null && Val != DBNull.Value)
            {
                string vv = Val.ToString();
                var numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "," };
                return decimal.Parse(vv, numberFormatInfo);
            }
            else
            {
                return 0;
            }
        }
        public static bool DbObjectToBoolean(this object Val)
        {
            if (Val != null && Val != DBNull.Value)
            {
                return Convert.ToBoolean(Val);
            }
            else
            {
                return false;
            }
        }
        public static IEnumerable<T> TryAddToCollection<T>(this IEnumerable<T> Collection, IEnumerable<T> Addition)
        {
            if (Addition.Count() != 0)
            {
                Collection.ToList().AddRange(Addition);
                return Collection;
            }
            else
            {
                return Collection;
            }
        }
    }
}
