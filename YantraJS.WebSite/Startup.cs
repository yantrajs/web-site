using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NeuroSpeech;
using YantraJS.AspNetCore;
using YantraJS.Emit;

namespace YantraJS.WebSite
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.RegisterAssembly(typeof(Startup).Assembly);
            services.AddRouting();
            services.AddMvc(c => { 
                
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
            .AddViewOptions(o => {
                o.ViewEngines.Insert(0, new JSViewEngine());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            DictionaryCodeCache.Current = new AssemblyCodeCache();

            // if (env.IsDevelopment())
            // {
                app.UseDeveloperExceptionPage();
            // }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapFallback((r) => {
                    r.Response.Redirect("/index.html");
                    return Task.CompletedTask;
                });
            });
        }
    }
}
