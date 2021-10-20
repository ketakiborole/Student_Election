using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentElection.Models;

namespace StudentElection.Controllers
{
    public class CategeoryDifferenceController : Controller
    {
        // GET: CategeoryDifference
        public ActionResult Index()
        {
            Student_ElectionEntities db = new Student_ElectionEntities();
            var Categeory = db.Categeories.ToList();
            ViewBag.catdetails = Categeory;
           
            return View();
        }
    }
}