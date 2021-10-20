using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentElection.Models;
using System.Data.Entity.Validation;
using System.Data.Entity;

namespace StudentElection.Controllers
{
    public class StudentListController : Controller
    {
        // GET: StudentList
        public ActionResult Index(int categeoryid)
        {
            Student_ElectionEntities db = new Student_ElectionEntities();
            List<Student> students = db.Students.Where(e => e.Id == categeoryid).ToList();
            var currentcatergoryname = db.Categeories.Find(categeoryid);
            if (categeoryid != null)
            {
                TempData["selectedcategoryname"] = currentcatergoryname.Name;
            }
            return View(students);
        }

        //post:
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
          
            DateTime selectedCat = Convert.ToDateTime(form["startdate"]);
            DateTime  selectedCat1 = Convert.ToDateTime(form["enddate"]);
            using (Student_ElectionEntities db = new Student_ElectionEntities())
            {
                Categeory cat = new Categeory();
                string catname = TempData["selectedcategoryname"].ToString();
                List<Categeory> catlist = db.Categeories.Where(c => c.Name == catname).ToList();
                foreach(var c in catlist)
                {
                    c.Start_Date = selectedCat;
                    c.End_Date = selectedCat1;
                    c.Name = TempData["selectedcategoryname"].ToString();
                }
                db.SaveChanges();
                //var catname = db.Categeories.Where(x => x.Name);
                //Categeory cat = new Categeory();
                //    cat.Start_Date = selectedCat;
                //cat.End_Date = selectedCat1;
                //cat.Name = TempData["selectedcategoryname"].ToString();
                // db.Categeories.Add(cat);
                
                
               

            }
                return View();
        }
    }
}