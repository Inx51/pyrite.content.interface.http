using Microsoft.Extensions.DependencyInjection;
using Pyrite.Content.Interface.Http.Factories;
using Pyrite.Content.Interface.Http.Interfaces.Factories;

namespace Pyrite.Content.Interface.Http.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static void AddPyriteContentHttpInterface(this IServiceCollection services)
        {
            services.AddTransient<IHttpMethodStrategyFactory, HttpMethodStrategyFactory>();
        }
    }
}