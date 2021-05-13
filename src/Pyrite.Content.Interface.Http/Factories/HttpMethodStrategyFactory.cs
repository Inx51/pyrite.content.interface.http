using Pyrite.Content.Abstractions.Interfaces.Repositories;
using Pyrite.Content.Interface.Http.HttpMethodStrategies;
using Pyrite.Content.Interface.Http.Interfaces;
using Pyrite.Content.Interface.Http.Interfaces.Factories;
using System.Collections.Generic;

namespace Pyrite.Content.Interface.Http.Factories
{
    public class HttpMethodStrategyFactory : IHttpMethodStrategyFactory
    {
        private Dictionary<string, IHttpMethodStrategy> _httpMethodStraegyContainer;

        public HttpMethodStrategyFactory(IResourceRepository resourceRepository)
        {
            this._httpMethodStraegyContainer = new Dictionary<string, IHttpMethodStrategy>
            {
                ["GET"] = new GetHttpMethodStrategy(resourceRepository),
                ["HEAD"] = new HeadHttpMethodStrategy(resourceRepository),
                ["POST"] = new PostHttpMethodStrategy(resourceRepository),
                ["PUT"] = new PutHttpMethodStrategy(resourceRepository),
                ["DELETE"] = new DeleteHttpMethodStrategy(resourceRepository)
            };
        }

        public IHttpMethodStrategy Create(string method)
        {
            if (!this._httpMethodStraegyContainer.ContainsKey(method))
                return null;

            return this._httpMethodStraegyContainer[method];
        }
    }
}