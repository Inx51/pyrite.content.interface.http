using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Pyrite.Content.Interface.Http.Interfaces
{
    public interface IHttpMethodStrategy
    {
        public Task ExecuteAsync(HttpContext httpContext);
    }
}