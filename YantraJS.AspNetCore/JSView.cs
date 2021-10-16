using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YantraJS.AspNetCore.Modules;
using YantraJS.Core;
using YantraJS.Core.Clr;

namespace YantraJS.AspNetCore
{
    public class JSView : IView
    {

        // Module name
        static KeyString AspNetCore = nameof(AspNetCore);


        static KeyString model = nameof(model);
        static KeyString render = nameof(render);
        static KeyString services = nameof(services);
        static KeyString viewContext = nameof(viewContext);

        readonly string filePath;
        readonly string folder;

        public JSView(string folder, string filePath)
        {
            this.folder = folder;
            var path = filePath;
            if (path.StartsWith(folder))
            {
                path = "./" + path.Substring(folder.Length);
            }
            else
            {

            }
            this.filePath = path;
        }

        public string Path => this.filePath;

        public async Task RenderAsync(ViewContext context)
        {
            var s = new SynchronizationContext();

            // Create new JavaScript context
            using (var yc = new YantraContext(this.folder, s))
            {

                // Wrap aspNetCore module
                // which will export View class                
                var aspNetCore = typeof(JSAspNetCoreModule).Marshal() as JSObject;
                yc.RegisterModule(AspNetCore, aspNetCore);

                // Wrap services
                var ys = new YantraServiceResolver(context.HttpContext.RequestServices).Marshal();

                // Expose services on context
                yc[services] = ys;

                // Load JavaScript module at given location and execute it
                // it will return module exports
                var exports = await yc.RunAsync(folder, "./" + filePath);

                // load `default`
                var jsViewClass = exports[KeyStrings.@default];

                // Create new instance of default export
                // pass the context
                var view = jsViewClass.CreateInstance(context.Marshal());

                // Invoke render method of the view
                var data = view.InvokeMethod(render);

                // if returned data was string, write it
                if (data.IsString)
                {
                    await context.Writer.WriteLineAsync(data.ToString());
                } else
                {
                    // if it is a promise, you can await on it
                    if(data is JSPromise promise)
                    {
                        data = await promise.Task;
                    }
                    await context.Writer.WriteLineAsync(data.ToString());
                }
            }
        }
    }

    public class YantraServiceResolver 
    {
        private readonly IServiceProvider services;

        public YantraServiceResolver(IServiceProvider services)
        {
            this.services = services;
        }

        public JSValue Resolve(in Arguments a)
        {
            var f = a[0] ?? throw new ArgumentNullException();
            if(!f.ConvertTo<Type>(out var type))
            {
                if (!f.IsString)
                    throw new ArgumentOutOfRangeException();
                var typeName = f.ToString();
                type = Type.GetType(typeName) ?? throw new TypeAccessException($"Could not load type `{typeName}`");
                
            }
            return services.GetRequiredService(type).Marshal();
        }


    }
}
