using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ExcelDataReader;
using OfficeOpenXml;
using StudentElection.Models;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.IO;
using System.Text.RegularExpressions;
using LinqToExcel;
using System.Data.SqlClient;

namespace StudentElection.Controllers
{
    public class AdminController : Controller
    {
        private Student_ElectionEntities db = new Student_ElectionEntities();
        private IQueryable<Student> studentexcel;
        private int CurrentRollNumber;
        // GET: Admin
        public AdminController()
        {

        }
        public ActionResult Index()
        {
            return View(db.Admins.ToList());
        }

        // GET: Admin/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = db.Admins.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,userid,password")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                db.Admins.Add(admin);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(admin);
        }

        // GET: Admin/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = db.Admins.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,userid,password")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(admin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(admin);
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = db.Admins.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Admin admin = db.Admins.Find(id);
            db.Admins.Remove(admin);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //for creating login page
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Admin objadmin)
        {
            if (ModelState.IsValid)
            {
                using(Student_ElectionEntities db =new Student_ElectionEntities())
                {
                    var obj = db.Admins.Where(a => a.userid.Equals(objadmin.userid) && a.password.Equals(objadmin.password)).FirstOrDefault();
                    if(obj != null)
                    {
                        Session["userid"] = obj.userid.ToString();
                        Session["password"] = obj.password.ToString();
                        return RedirectToAction("AdminHomePage");
                    }
                    
                   
                   
                }
                
            }
            ModelState.AddModelError("", "Invalid Username and Password");
            return View(objadmin);
            
            
            
        }
        //for admin home page
        public ActionResult AdminHomePage()
        {
            if (Session["userid"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        //for logout
        public ActionResult LogOut()
        {
            TempData["SuccessMessage"] = "Your Success Message";

            return RedirectToAction("Login");
        }
        //for uploading excel
        public ActionResult Upload()
        {
            return View();
        }
        //for uploading resister student
        [HttpPost]
        public JsonResult UploadExcel(Student students, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/Doc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
                    var ds = new DataSet();
                    adapter.Fill(ds, "ExcelTable");
                    DataTable dtable = ds.Tables["ExcelTable"];
                    string sheetName = "Sheet1";
                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    studentexcel = from a in excelFile.Worksheet<Student>(sheetName) select a;
                    foreach (var a in studentexcel)
                    {
                        try
                        {
                            if (a.studentname != ""  && a.branch != "")
                            {
                                Student stu = new Student();
                                stu.studentname = a.studentname;
                                stu.branch = a.branch;
                                stu.mobilenumber = a.mobilenumber;
                                stu.yearofjoining = a.yearofjoining;

                                stu.password = Convert.ToDateTime(a.password).ToShortDateString();
                                stu.DOB = a.DOB;

                                // 

                               
                                db.Students.Add(stu);
                                db.SaveChanges();
                                //TU.Name = a.Name;
                                //TU.Address = a.Address;
                                //TU.ContactNo = a.ContactNo;
                                //db.Users.Add(TU);
                                //db.SaveChanges();
                            }
                            else
                            {
                                data.Add("<ul>");
                                if (a.studentname == "" || a.studentname == null) data.Add("<li> name is required</li>");
                                // if (a.mobilenumber == "" || a.mobilenumber == null) data.Add("<li> Address is required</li>");
                                if (a.branch == "" || a.branch == null) data.Add("<li>ContactNo is required</li>");
                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }
                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {
                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {
                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                                }
                            }
                        }
                    }
                    //deleting excel file from folder
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //alert message for invalid file format
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Enroll(int? rollno)
        {
            ViewBag.EnrollSucessMessage = "";
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Class Representative", Value = "Class Representative" });
            items.Add(new SelectListItem { Text = "Student President", Value = "Student President" });
            items.Add(new SelectListItem { Text = "Affinity Club", Value = "Affinity Club" });
            items.Add(new SelectListItem { Text = "Ecofriendly Club", Value = "Ecofriendly Club" });
            items.Add(new SelectListItem { Text = "Cultural Club", Value = "Cultural Club" });
            items.Add(new SelectListItem { Text = "Photography Club", Value = "Photography Club" });
            ViewData["Options"] = items;
            var details = db.Students.Find(rollno);
            if(rollno != null)
            {
                TempData["CurrentRollNumber"] = rollno;
            }
            
            if (rollno!= null && details == null)
            {
                ModelState.AddModelError("", "Roll number not found");
            }
            return View(details);
            //return View();
        }
        //for enroll candidate page
        [HttpPost]
       
        public ActionResult Enroll(HttpPostedFileBase file, FormCollection form)
        {
            if(form["submit"].ToString()== "Enroll")
            {
                string path = Server.MapPath("~/file");
                string fileName = Path.GetFileName(file.FileName);
                string fullapth = Path.Combine(path, fileName);
                file.SaveAs(fullapth);
                string selectedCat = form["Options"].ToString();
                using (Student_ElectionEntities db = new Student_ElectionEntities())
                {
                    Categeory cat = new Categeory();
                    cat.Name = selectedCat;
                    cat.rollno = int.Parse(TempData["CurrentRollNumber"].ToString());
                    db.Categeories.Add(cat);
                    db.SaveChanges();

                }
                ViewBag.EnrollSucessMessage = "Sucessfully Enrolled";


            }
            else
            {
                ViewBag.EnrollSucessMessage = "";
            }

            return View();
        }
        [HttpPost]
        public ActionResult Result()
        {
            ViewBag.EnrollSucessMessage = "";
            return View();
        }
       // Analyse vote
       public ActionResult AnalyseVote()
        {
            Student_ElectionEntities db = new Student_ElectionEntities();
            DateTime datetimenow = DateTime.Now;
            var election = db.Categeories.Where(x => x.End_Date < datetimenow);
           var unique_category= election.GroupBy(e => e.Name).Select(e => e.FirstOrDefault()).ToList();

            return View(unique_category);
           
        }
        //get student by category
        public ActionResult GetStudentByCategory(string Name)
        {
            Student_ElectionEntities db = new Student_ElectionEntities();
            List<Student> students = new List<Student>();
            List<Categeory> cat= db.Categeories.Where(c => c.Name == Name).ToList();
            foreach(var c in cat)
            {
                Student stu = db.Students.Find(c.rollno);
                students.Add(stu);
            }
            ViewData["Students"] = students;
            //Employee employee = entities.Employees.FirstOrDefault(e => e.EmployeeId == id);
            DateTime datetimenow = DateTime.Now;
            var election = db.Categeories.Where(x => x.End_Date < datetimenow);
            var unique_category = election.GroupBy(e => e.Name).Select(e => e.FirstOrDefault()).ToList();
            return View("AnalyseVote", unique_category);
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
