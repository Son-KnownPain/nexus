using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Security;
using System.Web.SessionState;

namespace eProject.Auth
{
    public static class AuthManager
    {
        public static bool IsAuthenticated
        {
            get
            {
                return HttpContext.Current.Session["UserLogin"] != null;
            }
        }
        public static class Chef
        {
            public static string SECRET_KEY
            {
                get => "hs0226";
            }
            public static string AUTH_USER_DATA_KEY
            {
                get => "auth_user_data";
            }
            public static string AUTH_HASHED_KEY
            {
                get => "auth_hashed";
            }
            public static string GetMixing(string data)
            {
                return data + SECRET_KEY;
            }
            public static bool PreservationUserCookies(object userData)
            {
                HttpCookie authUsernameCookie = new HttpCookie(
                    AUTH_USER_DATA_KEY,
                    "data of user is unique"
                );
                HttpContext.Current.Response.Cookies.Add(authUsernameCookie);

                HttpCookie authHash = new HttpCookie(
                    AUTH_HASHED_KEY,
                    Crypto.HashPassword(AuthManager.Chef.GetMixing("data of user is unique"))
                );
                HttpContext.Current.Response.Cookies.Add(authHash);

                AuthManager.CurrentUser.Update(userData);

                return true;
            }
            public static bool Spoiled()
            {
                HttpCookie authUsernameCookie = new HttpCookie(AUTH_USER_DATA_KEY);
                HttpCookie authHash = new HttpCookie(AUTH_HASHED_KEY);

                authUsernameCookie.Expires = DateTime.Now.AddDays(-1);
                authHash.Expires = DateTime.Now.AddDays(-1);

                HttpContext.Current.Response.Cookies.Add(authHash);
                HttpContext.Current.Response.Cookies.Add(authUsernameCookie);

                AuthManager.CurrentUser.Destroy();

                return true;
            }
        }
        public static class CurrentUser
        {
            public static bool IsAdmin
            {
                get
                {
                    if (HttpContext.Current.Session["UserLogin"] != null)
                    {
                        // Your code
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            public static object GetUser
            {
                get
                {
                    if (HttpContext.Current.Session["UserLogin"] != null)
                    {
                        // object is your user class model
                        return new { };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            public static void Update(object newUser)
            {
                // object is your user class model
                HttpContext.Current.Session["UserLogin"] = newUser;
            }
            public static void Destroy()
            {
                if (HttpContext.Current.Session["UserLogin"] != null)
                {
                    HttpContext.Current.Session.Remove("UserLogin");
                }
            }
        }
        public static class Roles
        {
            public static int AdminRole
            {
                get => 10;
            }
            public static int EmployeeRole
            {
                get => 5;
            }
        }
    }
}