using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core;
using YantraJS.Core.Clr;

namespace YantraJS.AspNetCore.Modules
{
    public class JSAspNetCoreModule
    {

        [JSName("View")]
        public static Type View => typeof(JSViewBase);

    }

    public class JSViewBase
    {
        public readonly ViewContext? ViewContext;
        public readonly object Model;
        public readonly ModelStateDictionary ModelState;
        public readonly JSValue Services;

        public JSViewBase(in Arguments a)
        {
            this.ViewContext = a.Get1().ForceConvert(typeof(ViewContext)) as ViewContext;
            this.Model = ViewContext!.ViewData.Model;
            this.ModelState = ViewContext.ViewData.ModelState;
            var ys = new YantraServiceResolver(ViewContext.HttpContext.RequestServices).Marshal();
            this.Services = ys;
        }
    }
}
