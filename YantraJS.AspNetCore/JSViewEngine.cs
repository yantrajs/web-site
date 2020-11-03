using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace YantraJS.AspNetCore
{
    public class JSView : IView
    {
        private string filePath;
        readonly string folder;

        public JSView(string folder, string filePath)
        {
            this.folder = folder;
            this.filePath = filePath;
        }

        public string Path => this.filePath;

        public Task RenderAsync(ViewContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class JSViewEngine : IViewEngine
    {
        readonly string folder;
        public JSViewEngine(string? folder = null)
        {
            this.folder = folder 
                ?? AppDomain.CurrentDomain.BaseDirectory
                ?? throw new NotImplementedException();
        }

        private static string[] _viewLocationFormats =
            new string[] { 
                "Views/{1}/{0}.js",
                "Views/Shared/{1}/{0}.js"
            };

        public ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage)
        {
            if (context.ActionDescriptor.RouteValues.TryGetValue("controller", out var controllerName))
            {
                var checkedLocations = new List<string>();

                foreach (var locationFormat in _viewLocationFormats)
                {
                    var possibleViewLocation = this.folder + "/" + string.Format(locationFormat, viewName, controllerName);

                    if (File.Exists(possibleViewLocation))
                    {
                        return ViewEngineResult.Found(viewName, new JSView(this.folder, possibleViewLocation));
                    }
                    checkedLocations.Add(possibleViewLocation);
                }

                return ViewEngineResult.NotFound(viewName, checkedLocations);
            }
            throw new Exception("Controller route value not found.");
        }

        public ViewEngineResult GetView(string executingFilePath, string viewPath, bool isMainPage)
        {
            if (string.IsNullOrEmpty(viewPath) || !viewPath.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
            {
                return ViewEngineResult.NotFound(viewPath, Enumerable.Empty<string>());
            }

            var appRelativePath = GetAbsolutePath(executingFilePath, viewPath);

            if (File.Exists(appRelativePath))
            {
                return ViewEngineResult.Found(viewPath, new JSView(appRelativePath));
            }

            return ViewEngineResult.NotFound(viewPath, new List<string> { appRelativePath });
        }
    }
}
