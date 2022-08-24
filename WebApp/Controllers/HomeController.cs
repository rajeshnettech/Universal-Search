using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }       
  

        [HttpPost]
        public PartialViewResult _MenuBar(WebApp.Models.SearchModel model)
        {
            model.Result = Data.UnifiedWebSearch.LocalFileSearch(model.Keywords);
            return PartialView("_MenuBar", model);
        }

        public ActionResult JsSearch()
        {
            return View();
        }

    }
}