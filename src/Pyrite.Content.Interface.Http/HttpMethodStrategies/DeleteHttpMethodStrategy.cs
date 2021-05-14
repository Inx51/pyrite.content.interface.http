using Microsoft.AspNetCore.Http;
using Pyrite.Content.Abstractions.Interfaces.Repositories;
using Pyrite.Content.Interface.Http.Interfaces;
using System.Threading.Tasks;

namespace Pyrite.Content.Interface.Http.HttpMethodStrategies
{
    public class DeleteHttpMethodStrategy : IHttpMethodStrategy
    {
        private readonly IResourceRepository _resourceRepository;

        public DeleteHttpMethodStrategy(IResourceRepository resourceRepository)
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

            await this._resourceRepository.DeleteAsync(httpContext.Request.Path);

            httpContext.Response.StatusCode = 204;
        }
    }
}