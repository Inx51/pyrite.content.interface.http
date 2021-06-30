using Microsoft.AspNetCore.Http;
using Pyrite.Content.Abstractions.Interfaces.Repositories;
using Pyrite.Content.Domain;
using Pyrite.Content.Interface.Http.Constants;
using Pyrite.Content.Interface.Http.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Pyrite.Content.Interface.Http.HttpMethodStrategies
{
    public class PutHttpMethodStrategy : IHttpMethodStrategy
    {
        private readonly IResourceRepository _resourceRepository;

        public PutHttpMethodStrategy(IResourceRepository resourceRepository)
        {
            this._resourceRepository = resourceRepository;
        }

        public async Task ExecuteAsync(HttpContext httpContext)
        {
            if (await this._resourceRepository.ExistsAsync(httpContext.Request.Path))
            {
                httpContext.Response.StatusCode = 204;
                await this._resourceRepository.DeleteAsync(httpContext.Request.Path);
            }
            else
            {
                httpContext.Response.StatusCode = 201;
                httpContext.Response.Headers.Add("Location", $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}");
            }

            var headers = httpContext.Request.Headers.Where
            (
                header =>
                !HttpHeaders.RequestHeaders.Contains(header.Key)
            );

            await this._resourceRepository.CreateAsync
            (
                new Resource
                (
                    httpContext.Request.Path,
                    headers,
                    httpContext.Request.Body
                )
            );
        }
    }
}