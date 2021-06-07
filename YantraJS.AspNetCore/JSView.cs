using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Linq;
using System.Threading.Tasks;
using YantraJS.Core;

namespace YantraJS.AspNetCore
{
    public class JSView : IView
    {
        static KeyString model = "model";
        static KeyString render = "render";

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
                var text = await yc.RunAsync(folder, "./" + filePath);
                var view = text[KeyStrings.@default].CreateInstance();
                view[model] = context.ViewData.Model.Marshal();
                var data = view.InvokeMethod(render).ToString();
                context.Writer.WriteLine(data);
            }
        }
    }
}
