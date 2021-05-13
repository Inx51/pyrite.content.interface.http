using Microsoft.AspNetCore.Http;
using Pyrite.Content.Abstractions.Interfaces.Repositories;
using Pyrite.Content.Interface.Http.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pyrite.Content.Interface.Http.HttpMethodStrategies
{
    public class HeadHttpMethodStrategy : IHttpMethodStrategy
    {
        private readonly IResourceRepository _resourceRepository;

        public HeadHttpMethodStrategy(IResourceRepository resourceRepository)
        {
            this._resourceRepository = resourceRepository;
        }

        public async Task ExecuteAsync(HttpContext httpContext)
        {
            if (!await this._resourceRepository.ExistsAsync(httpContext.Request.Path))
            {
                httpContext.Response.StatusCode = 404;
                return;
            }

            var resource = await this._resourceRepository.GetAsync
            (
                httpContext.Request.Path,
                false
            );

            httpContext.Response.StatusCode = 204;

            if (resource.Headers != null)
            {
                foreach (var header in resource.Headers)
                {
                    httpContext.Response.Headers.TryAdd(header.Key, header.Value);
                }
            }
        }
    }
}