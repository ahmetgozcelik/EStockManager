using System.Net;
using System.Text.Json;

namespace EStockManager.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        // private readonly ILogger<ErrorHandlingMiddleware> _logger; // Gelişmiş uygulamada loglama eklenir

        public ErrorHandlingMiddleware(RequestDelegate next /*, ILogger<ErrorHandlingMiddleware> logger */)
        {
            _next = next;
            // _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                // İstek, işlem hattında bir sonraki adıma iletilir.
                await _next(context);
            }
            catch (Exception ex)
            {
                // Bir hata yakalandığında, işlemi sonlandırır ve hata yanıtını hazırlar.
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Varsayılan olarak 500 Internal Server Error (Sunucu İç Hatası) döndürülür.
            var code = HttpStatusCode.InternalServerError;


            // Hata mesajını ve durumu içeren standart bir anonim JSON yanıtı oluşturulur.
            var result = JsonSerializer.Serialize(new
            {
                Error = exception.Message,
                StatusCode = (int)code
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            // Yanıtı istemciye yaz ve işlemi sonlandır.
            return context.Response.WriteAsync(result);
        }
    }
}