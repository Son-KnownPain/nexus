using eProject.Auth;
using eProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc.Filters;
using System.Web.Mvc;
using System.Web.Routing;

namespace eProject.Filters
{
    public class EmployeeAuthorization : ActionFilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            HttpCookie authEmployeeCookie = filterContext.HttpContext.Request.Cookies.Get(AuthManager.Chef.AUTH_CUSTOMER_DATA_KEY);
            HttpCookie authEmployeeHash = filterContext.HttpContext.Request.Cookies.Get(AuthManager.Chef.AUTH_CUSTOMER_HASHED_KEY);

            bool isNullCookies = authEmployeeCookie == null && authEmployeeHash == null;
            if (isNullCookies)
            {
                AuthManager.CurrentCustomer.Update(null);
                filterContext.Result = new HttpUnauthorizedResult();
            }
            else
            {
                string employeeID = authEmployeeCookie.Value;
                string codeHashed = authEmployeeHash.Value;

                bool isValidAuth = Crypto.VerifyHashedPassword(codeHashed, AuthManager.Chef.GetMixing(employeeID));

                if (!isValidAuth)
                {
                    AuthManager.CurrentEmployee.Update(null);
                    filterContext.Result = new HttpUnauthorizedResult();
                }
                else if (!AuthManager.IsEmployeeAuthenticated)
                {
                    NexusEntities context = new NexusEntities();
                    Employee employee = context.Employees.FirstOrDefault(c => c.EmployeeID == Convert.ToInt32(employeeID));
                    if (employee != null)
                    {
                        AuthManager.CurrentEmployee.Update(employee);
                    }
                    else
                    {
                        AuthManager.CurrentEmployee.Update(null);
                        filterContext.Result = new HttpUnauthorizedResult();
                    }
                }
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "area", "Admin" },
                    { "controller", "Employee" },
                    { "action", "SignIn" }
                });
            }
        }
    }
}