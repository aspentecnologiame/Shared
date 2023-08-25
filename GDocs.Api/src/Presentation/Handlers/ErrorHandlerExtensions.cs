namespace Microsoft.AspNetCore.Builder
{
    internal static class ErrorHandlerExtensions
    {
        public static IApplicationBuilder UseCustomErrorHandler(this IApplicationBuilder app) =>
            app.UseMiddleware<ErrorHandlerMiddleware>();
    }
}
