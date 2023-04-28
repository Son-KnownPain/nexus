using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Filters;
using System.Web.Mvc;
using eProject.Auth;
using System.Web.Helpers;
using System.Web.Routing;
using eProject.Models;

namespace eProject.Filters
{
    public class CustomerAuthorization : ActionFilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            HttpCookie authCustomerCookie = filterContext.HttpContext.Request.Cookies.Get(AuthManager.Chef.AUTH_CUSTOMER_DATA_KEY);
            HttpCookie authCustomerHash = filterContext.HttpContext.Request.Cookies.Get(AuthManager.Chef.AUTH_CUSTOMER_HASHED_KEY);

            bool isNullCookies = authCustomerCookie == null && authCustomerHash == null;
            if (isNullCookies)
            {
                AuthManager.CurrentCustomer.Update(null);
                filterContext.Result = new HttpUnauthorizedResult();
            }
            else
            {
                string customerID = authCustomerCookie.Value;
                string codeHashed = authCustomerHash.Value;

                bool isValidAuth = Crypto.VerifyHashedPassword(codeHashed, AuthManager.Chef.GetMixing(customerID));

                if (!isValidAuth)
                {
                    AuthManager.CurrentCustomer.Update(null);
                    filterContext.Result = new HttpUnauthorizedResult();
                }
                else
                {
                    NexusEntities context = new NexusEntities();
                    int customerIDInt = Convert.ToInt32(customerID);
                    Customer customer = context.Customers.FirstOrDefault(c => c.CustomerID == customerIDInt);
                    if (customer != null)
                    {
                        AuthManager.CurrentCustomer.Update(customer);
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
                    { "controller", "Customer" },
                    { "action", "Login" }
                });
            }
        }
    }
}