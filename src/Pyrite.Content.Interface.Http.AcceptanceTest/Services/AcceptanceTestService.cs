using Flurl.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Pyrite.Content.Interface.Http.Sample;

namespace Pyrite.Content.Interface.Http.AcceptanceTest.Services
{
    public class AcceptanceTestService : FlurlClient
    {
        public AcceptanceTestService() : base(new WebApplicationFactory<Startup>().CreateClient())
        {
            this.AllowAnyHttpStatus();
        }
    }
}