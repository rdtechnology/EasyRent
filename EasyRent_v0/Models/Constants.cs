using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyRent_v0.Models
{
    public class UserType
    {
        public static string EntityName
        { get { return "UserType"; } }

        public static string Renter
        { get { return "Renter"; } }

        public static string LandLord
        { get { return "LandLord"; } }

        public static string Admin
        { get { return "Admin"; }
        }
        public static string SuperAdmin
        { get { return "SuperAdmin"; } }
    }
}