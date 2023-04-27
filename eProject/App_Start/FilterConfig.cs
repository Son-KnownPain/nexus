using System.Web;
using System.Web.Mvc;
using eProject.Filters;

namespace eProject
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            // Auto re-sign in
            filters.Add(new AutoSignIn());
        }
    }
}
