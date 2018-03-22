using APM.WebApi.MultiCulture;
using System.Web.Mvc;

namespace APM.WebApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new LocalizationAttribute("en"));
        }
    }
}
