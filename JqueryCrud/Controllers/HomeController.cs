using JqueryCrud.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JqueryCrud.Controllers
{
    public class HomeController : Controller
    {
        JqueryTbaloEntities db = new JqueryTbaloEntities();
        public ActionResult Index()
        {
            List<tblDepartment> DeptList = db.tblDepartment.ToList();
            ViewBag.ListOfDepartment = new SelectList(DeptList, "DepartmentId", "DepartmentName");
            return View();
        }
        public JsonResult GetStudentList()
        {
            List<StudentViewModel> StuList = db.tblStudent.Where(x => x.IsDeleted == false).Select(x => new StudentViewModel
            {
                StudentId = x.StudentId,
                StudentName = x.StudentName,
                DepartmentName = x.tblDepartment.DepartmentName,
                Email = x.Email
            }).ToList();
            return Json(StuList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStudentById(int StudentId)
        {
            tblStudent model = db.tblStudent.Where(x => x.StudentId == StudentId).SingleOrDefault();
            string value = string.Empty;
            value = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveDataInDatabase(StudentViewModel model)
        {
            var result = false;
            try
            {
                if (model.StudentId > 0)
                {
                    tblStudent stu = db.tblStudent.SingleOrDefault(x => x.IsDeleted == false && x.StudentId == model.StudentId);
                    stu.StudentName = model.StudentName;
                    stu.Email = model.Email;
                    stu.DepartmentId = model.DepartmentId;
                    db.SaveChanges();
                    result = true;
                }
                else
                {
                    tblStudent stu = new tblStudent();
                    stu.StudentName = model.StudentName;
                    stu.Email = model.Email;
                    stu.DepartmentId = model.DepartmentId;
                    stu.IsDeleted = false;
                    db.tblStudent.Add(stu);
                    db.SaveChanges();
                    result = true;
                }
            }
            catch(Exception ex) 
            {
                throw ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
    
        }
        public JsonResult DeleteStudentRecord(int StudentId)
        {
            bool result = false;
            tblStudent Stu = db.tblStudent.SingleOrDefault(x => x.IsDeleted == false && x.StudentId == StudentId);
            if (Stu!=null)
            {
                Stu.IsDeleted = true;
                db.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}