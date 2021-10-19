using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentElection.Controllers
{
    public class CategeoryController : Controller
    {
        // GET: Categeory
        public ActionResult Index()
        {

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "class representative", Value = "0" });
            items.Add(new SelectListItem { Text = "Student President", Value = "1" });
            items.Add(new SelectListItem { Text = "Affinity Club", Value = "2" });
            items.Add(new SelectListItem { Text = "Ecofriendly Club", Value = "3" });
            items.Add(new SelectListItem { Text = "Cultural Club", Value = "4" });
            items.Add(new SelectListItem { Text = "Photography Club", Value = "5" });
            ViewData["Options"] = items;
            return View();
        }
        public ViewResult image()
        {
            //    //Code to save the customer data here
            //    ViewData["error"] = "Candidate enrolled  successfully";
            return View();
        }
        [HttpPost]
        public ActionResult image(HttpPostedFileBase file)
        {
            string path = Server.MapPath("~file");
            string fileName = Path.GetFileName(file.FileName);
            string fullapth = Path.Combine(path, fileName);
            file.SaveAs(fullapth);
            return View();

        }
    }
}