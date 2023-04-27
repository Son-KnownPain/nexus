using eProject.Auth;
using eProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace eProject.Filters
{
    public class AutoSignIn : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Customer
            HttpCookie authCustomerCookie = filterContext.HttpContext.Request.Cookies.Get(AuthManager.Chef.AUTH_CUSTOMER_DATA_KEY);
            HttpCookie authCustomer = filterContext.HttpContext.Request.Cookies.Get(AuthManager.Chef.AUTH_CUSTOMER_HASHED_KEY);

            if (authCustomerCookie != null && authCustomer != null && !AuthManager.IsCustomerAuthenticated)
            {
                string customerID = authCustomerCookie.Value;
                string hashedCustomer = authCustomer.Value;

                bool isValidAuth = Crypto.VerifyHashedPassword(hashedCustomer, AuthManager.Chef.GetMixing(customerID));
                if (isValidAuth)
                {
                    NexusEntities context = new NexusEntities();
                    Customer customer = context.Customers.FirstOrDefault(c => c.CustomerID == Convert.ToInt32(customerID));
                    if (customer != null)
                    {
                        AuthManager.CurrentCustomer.Update(customer);
                    }
                }
            }

            // Employee
            HttpCookie authEmployeeCookie = filterContext.HttpContext.Request.Cookies.Get(AuthManager.Chef.AUTH_EMPLOYEE_DATA_KEY);
            HttpCookie authEmployee = filterContext.HttpContext.Request.Cookies.Get(AuthManager.Chef.AUTH_EMPLOYEE_HASHED_KEY);

            if (authEmployeeCookie != null && authEmployee != null && !AuthManager.IsCustomerAuthenticated)
            {
                string employeeID = authEmployeeCookie.Value;
                string hashedEmployee = authEmployee.Value;

                bool isValidAuth = Crypto.VerifyHashedPassword(hashedEmployee, AuthManager.Chef.GetMixing(employeeID));
                if (isValidAuth)
                {
                    NexusEntities context = new NexusEntities();
                    Employee employee = context.Employees.FirstOrDefault(e => e.EmployeeID == Convert.ToInt32(employeeID));
                    if (employee != null)
                    {
                        AuthManager.CurrentEmployee.Update(employee);
                    }
                }
            }
        }
    }
}