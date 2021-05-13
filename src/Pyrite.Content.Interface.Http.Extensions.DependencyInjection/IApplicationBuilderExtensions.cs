using Microsoft.AspNetCore.Builder;

namespace Pyrite.Content.Interface.Http.Extensions.DependencyInjection
{
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Uses the Pyrite Content HTTP interface.
        /// </summary>
        /// <param name="path">By setting the path, you can restrict Pyrite Content to only "listen" to paths below the set path. Default is set to "listen" for all paths</param>
        public static void UsePyriteContentHttpInterface
        (
            this IApplicationBuilder app,
            string path = "/"
        )
        {
            app.UseMiddleware<PyriteHttpMiddleware>(path);
        }
    }
}