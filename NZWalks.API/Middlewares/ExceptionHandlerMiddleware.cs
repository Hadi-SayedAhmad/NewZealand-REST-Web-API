using System.Net;

namespace NZWalks.API.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger1, RequestDelegate next)
        {
            Logger1 = logger1;
            Next = next;
        }

        public ILogger<ExceptionHandlerMiddleware> Logger1 { get; }
        public RequestDelegate Next { get; }


        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await Next(httpContext);
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid();
                //log the exception
                Logger1.LogError(ex, $"{errorId} : {ex.Message}");
                //return a custom error response
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";
                var error = new
                {
                    Id = errorId,
                    ErrorMessage = "Something went wrong!"
                };

                await httpContext.Response.WriteAsJsonAsync(error);
            }
        }
    }
}
