using EStockManager.Middlewares;

namespace EStockManager.MiddleWares
{
    public static class ErrorHandlingExtensions
    {
        // uzantı metotları static olmalı
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
