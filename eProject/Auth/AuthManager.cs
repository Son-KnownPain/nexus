using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Security;
using System.Web.SessionState;
using eProject.Models;

namespace eProject.Auth
{
    public static class AuthManager
    {
        public static bool IsCustomerAuthenticated
        {
            get
            {
                return HttpContext.Current.Session["CustomerLogin"] != null;
            }
        }
        public static bool IsEmployeeAuthenticated
        {
            get
            {
                return HttpContext.Current.Session["EmployeeLogin"] != null;
            }
        }
        public static class Chef
        {
            public static string SECRET_KEY
            {
                get => "hs0226";
            }
            public static string AUTH_CUSTOMER_DATA_KEY
            {
                get => "auth_customer_data";
            }
            public static string AUTH_CUSTOMER_HASHED_KEY
            {
                get => "auth_customer_hashed";
            }
            public static string AUTH_EMPLOYEE_DATA_KEY
            {
                get => "auth_employee_data";
            }
            public static string AUTH_EMPLOYEE_HASHED_KEY
            {
                get => "auth_employee_hashed";
            }
            public static string GetMixing(string data)
            {
                return data + SECRET_KEY;
            }
            public static bool PreservationCustomerCookies(Customer accountData)
            {
                HttpCookie authCustomerCookie = new HttpCookie(
                    AUTH_CUSTOMER_DATA_KEY,
                    accountData.CustomerID + ""
                );
                HttpContext.Current.Response.Cookies.Add(authCustomerCookie);

                HttpCookie authCustomerHash = new HttpCookie(
                    AUTH_CUSTOMER_HASHED_KEY,
                    Crypto.HashPassword(AuthManager.Chef.GetMixing(accountData.CustomerID + ""))
                );
                HttpContext.Current.Response.Cookies.Add(authCustomerHash);

                AuthManager.CurrentCustomer.Update(accountData);

                return true;
            }
            public static bool SpoiledCustomer()
            {
                HttpCookie authUsernameCookie = new HttpCookie(AUTH_CUSTOMER_DATA_KEY);
                HttpCookie authHash = new HttpCookie(AUTH_CUSTOMER_HASHED_KEY);

                authUsernameCookie.Expires = DateTime.Now.AddDays(-1);
                authHash.Expires = DateTime.Now.AddDays(-1);

                HttpContext.Current.Response.Cookies.Add(authHash);
                HttpContext.Current.Response.Cookies.Add(authUsernameCookie);

                AuthManager.CurrentCustomer.Destroy();

                return true;
            }
            public static bool PreservationEmployeeCookies(Employee accountData)
            {
                HttpCookie authUsernameCookie = new HttpCookie(
                    AUTH_EMPLOYEE_DATA_KEY,
                    accountData.EmployeeID + ""
                );
                HttpContext.Current.Response.Cookies.Add(authUsernameCookie);

                HttpCookie authHash = new HttpCookie(
                    AUTH_EMPLOYEE_HASHED_KEY,
                    Crypto.HashPassword(AuthManager.Chef.GetMixing(accountData.EmployeeID + ""))
                );
                HttpContext.Current.Response.Cookies.Add(authHash);

                AuthManager.CurrentEmployee.Update(accountData);

                return true;
            }
            public static bool SpoiledEmployee()
            {
                HttpCookie authUsernameCookie = new HttpCookie(AUTH_EMPLOYEE_DATA_KEY);
                HttpCookie authHash = new HttpCookie(AUTH_EMPLOYEE_HASHED_KEY);

                authUsernameCookie.Expires = DateTime.Now.AddDays(-1);
                authHash.Expires = DateTime.Now.AddDays(-1);

                HttpContext.Current.Response.Cookies.Add(authHash);
                HttpContext.Current.Response.Cookies.Add(authUsernameCookie);

                AuthManager.CurrentEmployee.Destroy();

                return true;
            }
        }
        public static class CurrentCustomer
        {
            public static Customer GetCustomer
            {
                get
                {
                    if (HttpContext.Current.Session["CustomerLogin"] != null)
                    {
                        return HttpContext.Current.Session["CustomerLogin"] as Customer;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            public static void Update(Customer newAccount)
            {
                // object is your user class model
                HttpContext.Current.Session["CustomerLogin"] = newAccount;
            }
            public static void Destroy()
            {
                if (HttpContext.Current.Session["CustomerLogin"] != null)
                {
                    HttpContext.Current.Session.Remove("CustomerLogin");
                }
            }
        }
        public static class CurrentEmployee
        {
            public static bool IsRetailStoreEmployeeRole
            {
                get => GetEmployee.Role == EmployeeRoles.RetailStoreEmployeeRole;
            }
            public static bool IsAdmin
            {
                get => GetEmployee.Role == EmployeeRoles.AdminRole;
            }
            public static Employee GetEmployee
            {
                get
                {
                    if (HttpContext.Current.Session["EmployeeLogin"] != null)
                    {
                        return HttpContext.Current.Session["EmployeeLogin"] as Employee;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            public static void Update(Employee newAccount)
            {
                // object is your user class model
                HttpContext.Current.Session["EmployeeLogin"] = newAccount;
            }
            public static void Destroy()
            {
                if (HttpContext.Current.Session["EmployeeLogin"] != null)
                {
                    HttpContext.Current.Session.Remove("EmployeeLogin");
                }
            }
        }
        public static class EmployeeRoles
        {
            public static int AdminRole
            {
                get => 10;
            }
            public static int RetailStoreEmployeeRole
            {
                get => 5;
            }
        }
    }
}