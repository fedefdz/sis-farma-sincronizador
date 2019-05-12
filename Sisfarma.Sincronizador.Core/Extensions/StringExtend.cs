using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Sisfarma.Sincronizador.Core.Extensions
{
    public static class StringExtension
    {
        public static string Strip(this string word) => word != null
                ? Regex.Replace(word.Trim(), @"[.',\-\\]", string.Empty)
                : string.Empty;
        
        public static int ToIntegerOrDefault(this string @this, int @default = 0)
        {
            if (string.IsNullOrWhiteSpace(@this))
                return @default;

            if (int.TryParse(@this, out var integer))
                return integer;

            return @default;
        }

        public static long ToLongOrDefault(this string @this, int @default = 0)
        {
            if (string.IsNullOrWhiteSpace(@this))
                return @default;

            if (long.TryParse(@this, out var number))
                return number;

            return @default;
        }

        public static DateTime ToDateTimeOrDefault(this string @this, string format)
        {
            if (string.IsNullOrWhiteSpace(@this))
                return default(DateTime);

            if (DateTime.TryParseExact(@this, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))            
                return fecha;

            return default(DateTime);
        }        
    }
}
