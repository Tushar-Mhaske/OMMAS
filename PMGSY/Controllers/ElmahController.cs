using PMGSY.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Controllers
{
    public class ElmahController : Controller
    {
        //
        // GET: /Elmah/

        public ActionResult Index(string type)
        {
            return new ElmahResult(type);
        }

    }
}
