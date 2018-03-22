using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
//using System.Web.Mvc;

namespace APM.WebApi.MultiCulture
{
    public class LocalizationAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        private string _DefaultLanguage = "en";

        public LocalizationAttribute(string defaultLanguage)
        {
            _DefaultLanguage = defaultLanguage;
        }
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            string lang = (string)actionContext.ControllerContext.RouteData.Values["lang"];

            if (string.IsNullOrWhiteSpace(lang))
                return;

            //string lang = actionExecutedContext.Request.RequestUri.Query;//..Values["lang"] ?? _DefaultLanguage;
            if (lang != _DefaultLanguage)
            {
                try
                {
                    Thread.CurrentThread.CurrentCulture =
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
                }
                catch (Exception)
                {
                    throw new NotSupportedException(String.Format("ERROR: Invalid language code '{0}'.", lang));
                }
            }
        }
        //public override void OnActionExecuted(System.Web.Http.Filters.HttpActionExecutedContext actionExecutedContext)
        //{

        //}
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    string lang = (string)filterContext.RouteData.Values["lang"] ?? _DefaultLanguage;
        //    if (lang != _DefaultLanguage)
        //    {
        //        try
        //        {
        //            Thread.CurrentThread.CurrentCulture =
        //            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
        //        }
        //        catch (Exception)
        //        {
        //            throw new NotSupportedException(String.Format("ERROR: Invalid language code '{0}'.", lang));
        //        }
        //    }
        //}
    }
}