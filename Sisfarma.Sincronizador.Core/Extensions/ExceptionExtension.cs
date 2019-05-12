using System;

namespace Sisfarma.Sincronizador.Core.Extensions
{
    public static class ExceptionExtension
    {
        public static string ToLogErrorMessage(this Exception @this) 
            => $"Message: {@this.Message}{Environment.NewLine}StackTrace: {@this.StackTrace}";

        //public static string ToLogErrorMessage(this RestClientException @this) 
        //    => $"Request: {@this.Request.ToString()}{Environment.NewLine}Response: {@this.Response.ToString()}";
    }
}
