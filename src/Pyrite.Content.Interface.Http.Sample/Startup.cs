using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pyrite.Content.Interface.Http.Builder;
using Pyrite.Content.Interface.Http.Extensions.DependencyInjection;
using Pyrite.Content.Repository.Memory.Extensions.DependencyInjection;

namespace Pyrite.Content.Interface.Http.Sample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddPyriteContentHttpInterface();
            services.AddPyriteContentMemoryRepository();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UsePyriteContentHttpInterface();
        }
    }
}