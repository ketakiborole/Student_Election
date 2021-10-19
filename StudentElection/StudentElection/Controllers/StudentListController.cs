using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentElection.Models;

namespace StudentElection.Controllers
{
    public class StudentListController : Controller
    {
        // GET: StudentList
        public ActionResult Index(int categeoryid)
        {
            Student_ElectionEntities db = new Student_ElectionEntities();
            List<Student> students = db.Students.Where(e => e.Id == categeoryid).ToList();
            return View(students);
        }
    }
}