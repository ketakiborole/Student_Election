using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentElection.Models;

namespace StudentElection.Controllers
{
    public class CategoryListController : Controller
    {
        // GET: CategoryList
        public ActionResult Index()
        {
            Student_ElectionEntities db = new Student_ElectionEntities();
            return View(db.Categeories.ToList());
        }
    }
}