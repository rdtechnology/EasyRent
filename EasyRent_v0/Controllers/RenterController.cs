using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyRent_v0.Controllers
{
    public class RenterController : Controller
    {
        // GET: Renter
        public ActionResult EditProfile()
        {
            return View();
        }
    }
}