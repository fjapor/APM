﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
//using System.Web.Mvc;

namespace APM.WebApi.MultiCulture
{
    /// <summary>
    /// Attribute created in order to check what language/culture the API wish to request. 
    /// It is used to handle multi-currencies, but can be extended to handle multi-languages in the future as needed.
    /// </summary>
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
    }
}