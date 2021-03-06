﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YantraJS.AspNetCore
{
    public class JSViewEngine : IViewEngine
    {
        static readonly string[] _viewLocationFormats =
            new string[] {
                "dist/Area/{2}/Views/{1}/{0}.js",
                "dist/Area/{2}/Views/Shared/{1}/{0}.js",
                "dist/Views/{1}/{0}.js",
                "dist/Views/Shared/{1}/{0}.js",
                "Area/{2}/Views/{1}/{0}.js",
                "Area/{2}/Views/Shared/{1}/{0}.js",
                "Views/{1}/{0}.js",
                "Views/Shared/{1}/{0}.js"
            };

        readonly ViewEngineResult NotFound;
        IMemoryCache? cache;
        IHostingEnvironment? hostingEnvironment;
        public JSViewEngine()
        {
            this.NotFound = ViewEngineResult.NotFound("", Enumerable.Empty<string>());
        }


        public ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage)
        {
            var services =  context.HttpContext.RequestServices;
            cache ??= (services.GetRequiredService<IMemoryCache>());
            hostingEnvironment ??= (services.GetRequiredService<IHostingEnvironment>());
            context.ActionDescriptor.RouteValues.TryGetValue("area", out var area);
            if (context.ActionDescriptor.RouteValues.TryGetValue("controller", out var controllerName))
            {

                string key = $"__ViewPath__{area}_{controllerName}_{viewName}";
                if (cache.TryGetValue<ViewEngineResult>(key, out var viewResult))
                    return viewResult;

                var checkedLocations = new List<string>();

                foreach (var locationFormat in _viewLocationFormats)
                {
                    var possibleViewLocation = string.Format(locationFormat, viewName, controllerName, area);

                    if (File.Exists(possibleViewLocation))
                    {
                        var view = new JSView(hostingEnvironment.ContentRootPath, possibleViewLocation);
                        viewResult = ViewEngineResult.Found(viewName, view);
                        cache.Set(key, viewResult, new MemoryCacheEntryOptions { 
                            SlidingExpiration = TimeSpan.FromMinutes(1) 
                        });
                        return viewResult;
                    }
                    checkedLocations.Add(possibleViewLocation);
                }

                return ViewEngineResult.NotFound(viewName, checkedLocations);
            }
            throw new Exception("Controller route value not found.");
        }



        public ViewEngineResult GetView(string executingFilePath, string viewPath, bool isMainPage)
        {
            // if (string.IsNullOrEmpty(viewPath))
            // {
                return NotFound;
            // }

            //var appRelativePath = GetAbsolutePath(executingFilePath, viewPath);

            //if (File.Exists(appRelativePath))
            //{
            //    return ViewEngineResult.Found(viewPath, new JSView(this.folder, appRelativePath));
            //}

            //return ViewEngineResult.NotFound(viewPath, new List<string> { appRelativePath });
        }

        public string GetAbsolutePath(string executingFilePath, string pagePath)
        {
            if (string.IsNullOrEmpty(pagePath))
            {
                // Path is not valid; no change required.
                return pagePath;
            }

            if (IsApplicationRelativePath(pagePath))
            {
                // An absolute path already; no change required.
                return pagePath;
            }

            if (!IsRelativePath(pagePath))
            {
                // A page name; no change required.
                return pagePath;
            }

            if (string.IsNullOrEmpty(executingFilePath))
            {
                // Given a relative path i.e. not yet application-relative (starting with "~/" or "/"), interpret
                // path relative to currently-executing view, if any.
                // Not yet executing a view. Start in app root.
                var absolutePath = "/" + pagePath;
                return ViewEnginePath.ResolvePath(absolutePath);
            }

            return ViewEnginePath.CombinePath(executingFilePath, pagePath);
        }


        private static bool IsApplicationRelativePath(string name)
        {
            return name[0] == '~' || name[0] == '/';
        }

        private static bool IsRelativePath(string name)
        {
            // Though ./ViewName looks like a relative path, framework searches for that view using view locations.
            return name.EndsWith(".js", StringComparison.OrdinalIgnoreCase) 
                || name.EndsWith(".jsx", StringComparison.OrdinalIgnoreCase);
        }
    }
}
