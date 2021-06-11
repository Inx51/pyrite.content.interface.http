using Microsoft.AspNetCore.Http;
using Pyrite.Content.Interface.Http.Interfaces.Factories;
using System.Threading.Tasks;

namespace Pyrite.Content.Interface.Http
{
    public class PyriteHttpMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly string _route;

        public PyriteHttpMiddleware
        (
            RequestDelegate next,
            string route = "/"
        )
        {
            this._next = next;
            this._route = route;
        }

        public virtual async Task InvokeAsync
        (
            HttpContext context,
            IHttpMethodStrategyFactory httpMethodStrategyFactory
        )
        {
            if (!context.Request.Path.ToString().StartsWith(this._route))
            {
                await this._next(context);
                return;
            }

            context.Response.StatusCode = 405;

            var httpMethodStrategy = httpMethodStrategyFactory.Create(context.Request.Method);
            if (httpMethodStrategy != null)
                await httpMethodStrategy.ExecuteAsync(context);

            return;
        }
    }
}