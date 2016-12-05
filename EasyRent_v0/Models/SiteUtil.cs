using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyRental.Database;

namespace EasyRent_v0.Models
{
    public class SiteUtil
    {
        public static EasyRentalEntities NewDb
        {
            get
            {
                var db = new EasyRentalEntities();
                return db;
            }
        }

        public static User CurrentUser
        {
            get
            {
                if (!HttpContext.Current.Request.IsAuthenticated && HttpContext.Current.Session["CurrentUser"] == null)
                {
                    return null;
                }

                if (HttpContext.Current.Session["CurrentUser"] == null)
                {
                    //AccountController.ResetCurrentUserSession();
                }

                return (User)HttpContext.Current.Session["CurrentUser"];
            }

            set { HttpContext.Current.Session["CurrentUser"] = value; }
        }

        public static string GetToken(string Email)
        {
            var i = Email.LastIndexOf('.');

            if (i >= 0)
            {
                Email = Email.Insert(i, string.Format("{0}", Guid.NewGuid()));
            }
            return Email;
        }
    }
}