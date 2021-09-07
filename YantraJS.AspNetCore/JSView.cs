using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YantraJS.Core;
using YantraJS.Core.Clr;

namespace YantraJS.AspNetCore
{
    public class JSView : IView
    {
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
            using (var yc = new YantraContext(this.folder))
            {
                var ys = new YantraServiceResolver(context.HttpContext.RequestServices).Marshal();
                yc[services] = ys;
                var text = await yc.RunAsync(folder, "./" + filePath);
                var view = text[KeyStrings.@default].CreateInstance();
                view[model] = context.ViewData.Model.Marshal();
                view[services] = ys;
                view[viewContext] = context.Marshal();
                var data = view.InvokeMethod(render).ToString();
                context.Writer.WriteLine(data);
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
