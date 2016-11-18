///////////////////////////////////////////////////////////////////////////////////////////////////
//
//	File Name:         StudentController.cs
//	Description:       This class contains action methods associated with the StudentController. 
//
//	Author:            Cynthia Benjamin, benjamin@etsu.edu
//
///////////////////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SoftwareEngineering1Project.DataContexts;
using SoftwareEngineering1Project.Models;
using SoftwareEngineering1Project.Helpers;
using SoftwareEngineering1Project.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Security;
using System.Data.Entity;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNet.Identity.Owin;
using System.Diagnostics;

namespace SoftwareEngineering1Project.Controllers
{
    [Authorize(Roles = "professor")]
    public class StudentController : Controller
    {
        private ApplicationDb _studentDb = new ApplicationDb();
        private IdentityDb _userDb = new IdentityDb();

        protected override void Dispose(bool disposing)
        {
            _studentDb.Dispose();
            _userDb.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Index Action - displays a list of all students using a DataTable view helper
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //DataTable is built using lists of generic objects
            List<object> allStudentProfiles = new List<object>();

            int sysadminID = 0;

            var allStudents = _studentDb.Students.ToList();

            var roles = _userDb.Roles.ToList();
            string tempRoleHolder = "";
            foreach (var student in allStudents)
            {
                if(student.Archived == false)
                {
                    if (tempRoleHolder == "Sysadmin")
                    {
                        ViewBag.SysadminId = student.ID;
                        sysadminID = student.ID;
                    }

                    //adds new object to list - setup like key-value pairs
                    allStudentProfiles.Add(
                        new
                        {
                            id = student.ID,
                            firstName = student.FirstName,
                            lastName = student.LastName,
                            concentration = student.Concentration
                        }
                    );
                }

            }

            DataTableModel studentTable = new DataTableModel();
            studentTable.Title = "Students";

            //sets the table headers, the Field attribute must  match the key value from the allStudentProfiles list
            studentTable.Headers = new List<object>()
            {
                new
                {
                    Name = "First Name",
                    Field = "firstName"
                },
                new
                {
                    Name = "Last Name",
                    Field = "lastName"
                },
                new
                {
                    Name = "Concentration",
                    Field = "concentration"
                },

            };
            studentTable.Data = allStudentProfiles;

            //sets the actions that will display for every entry in the table
            //uses a str replace to replace the {{id}} values with the actual id
            //id matches to the key seen earlier
            studentTable.Actions = new List<object>()
            {
                new
                {
                    text = "View",
                    url = "/student/view/{{id}}"
                },
                new
                {
                    text = "Edit",
                    url = "/student/edit/{{id}}"
                },
                new
                {
                    text = "Archive",
                    url = "/student/archive/{{id}}"
                }
            };
            //allows the table to be searched and sorted
            studentTable.SearchSort = true;
            //adds the table button for adding a student
            studentTable.TableButtons = new List<object>()
            {
                new
                {
                    text = "Create Student",
                    url = "/student/add"
                },
                new
                {
                    text = "View Archived Students",
                    url = "/student/viewarchived"
                }
            };

            string modifiedTable = studentTable.Render().ToString();

            //render function returns an HtmlString to the view
            return View(new HtmlString(modifiedTable));
        }

        /// <summary>
        /// Get Add action
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            ViewBag.AllSections = _studentDb.Sections.Include(s => s.Course).ToList();
            return View();
        }

        /// <summary>
        /// Post Add Action
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(Student newStudent, FormCollection data)
        {
            //checks to make sure everything was filled out appropriately in accordance
            //to the tags in the ViewModel
            if (!ModelState.IsValid)
            {
                //creates the student from the information entered
                Student student = new Student();
                student.FirstName = newStudent.FirstName;
                student.LastName = newStudent.LastName;
                student.Concentration = newStudent.Concentration;
                student.EnterDate = newStudent.EnterDate;
                student.Notes = newStudent.Notes;

                //saves both the profile and the user to the database                    
                _studentDb.Students.Add(student);
                _studentDb.SaveChanges();

                foreach (String element in data)
                {
                    if (element.Contains("sections"))
                    {
                        var sectionIDArray = data[element].Split(',');
                        foreach (var id in sectionIDArray)
                        {
                            var section = _studentDb.Sections.Find(Int32.Parse(id));
                            section.Students.Add(student);
                            _studentDb.SaveChanges();
                        }
                    }
                }

                //add flash message for successful adding
                TempData["Message"] = new { Message = "Successfully added a student", Type = "success" };
                return RedirectToAction("Index", "Student");
            }
            ViewBag.AllSections = _studentDb.Sections.Include(s => s.Course).ToList();
            return View(newStudent);
        }

        /// <summary>
        /// The View Action - displays the user information using a PanelTable ViewHelper
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult View(int? id)
        {
            if (id == null)
            {
                return Redirect("Index");
            }

            Student studentInfo = _studentDb.Students.Find(id);
            if (studentInfo == null)
            {
                return HttpNotFound();
            }

            //Creates the PanelTable and sets its title and the number of 
            //items displayed in a row (includes headers)
            PanelTable viewTable = new PanelTable();
            viewTable.Title = "Student Information";
            viewTable.ItemsPerRow = 4;

            //the key is the the label and the value is the specific user's information
            viewTable.Data = new Dictionary<string, string>()
            {
                {"First Name",  studentInfo.FirstName},
                {"Last Name" , studentInfo.LastName},
                {"Concentration", studentInfo.Concentration},
                {"Notes", studentInfo.Notes },
                {"Enter Date" , studentInfo.EnterDate.Date.ToString("MMMM dd, yyyy")  },
                {"", "" }
            };
            //key is the url link and the value is what is displayed to the user
            viewTable.TableButtons = new Dictionary<string, string>()
            {
                {"/student/edit/" + studentInfo.ID, "Edit" },
                {"/student/archive/" + studentInfo.ID, "Archive" },
            };

            //render function returns an HtmlString to the view
            return View(viewTable.Render());

        }

        /// <summary>
        /// Displays a populated form for updating a teacher.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ViewResult</returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return Redirect("Index");

            var student = _studentDb.Students.Find(id);//find the student in the database
            if (student == null)//if the student doesn't exist then throw a 404
            {
                return HttpNotFound();
            }

            ViewBag.AllSections = _studentDb.Sections.Include(s => s.Course).ToList();

            return View(student);
        }

        /// <summary>
        /// Updates a student.
        /// </summary>
        /// <param name="student"></param>
        /// <returns>Mixed</returns>
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Edit(Student student, FormCollection data)
        {
            if (!ModelState.IsValid)
            {
                student.Sections.Clear();
                _studentDb.Entry(student).State = EntityState.Modified;//let the ORM know object has been modified
                _studentDb.SaveChanges();//persist changes

                foreach (String element in data)
                {
                    if (element.Contains("sections"))
                    {
                        var sectionIDArray = data[element].Split(',');
                        foreach (var id in sectionIDArray)
                        {
                            var section = _studentDb.Sections.Find(Int32.Parse(id));
                            section.Students.Add(student);
                            _studentDb.SaveChanges();
                        }
                    }
                }

                //add flash message for successful udpate
                TempData["Message"] = new { Message = "Successfully updated student", Type = "success" };
                return RedirectToAction("Index");
            }

            ViewBag.AllSections = _studentDb.Sections.Include(s => s.Course).ToList();
            return View(student);
        }
        /// <summary>
        /// Displays a form and information about the student that will be deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ViewResult</returns>
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //        return Redirect("Index");

        //    var student = _studentDb.Students.Find(id);//find the student in the database
        //    if (student == null)//if the student doesn't exist then throw a 404
        //    {
        //        return HttpNotFound();
        //    }

        //    //Creates the PanelTable and sets its title and the number of 
        //    //items displayed in a row (includes headers)
        //    PanelTable viewTable = new PanelTable();
        //    viewTable.Title = "Student Information";
        //    viewTable.ItemsPerRow = 4;

        //    //the key is the the label and the value is the specific student's information
        //    viewTable.Data = new Dictionary<string, string>()
        //    {
        //        { "First Name", student.FirstName},
        //        {"Last Name",  student.LastName},
        //        {"Enter Date" , student.EnterDate.ToString()},
        //        {"Concentration", student.Concentration},
        //        {"Notes",  student.Notes },
        //        { "", "" }
        //    };
        //    //add the table to the view bag
        //    ViewBag.Table = viewTable;

        //    return View(student);
        //}
        ///// <summary>
        ///// Deletes the specified teacher.
        ///// </summary>
        ///// <param name="student"></param>
        ///// <returns>RedirectToActionResult</returns>
        //[HttpPost, ActionName("Delete")]
        //public ActionResult PersistDelete(int id)
        //{
        //    _studentDb.Students.Remove(_studentDb.Students.Find(id));//remove the student from the database
        //    _studentDb.SaveChanges();//persist the changes
        //    //add flash message for successful deletion
        //    TempData["Message"] = new { Message = "Successfully deleted student", Type = "success" };
        //    return RedirectToAction("Index");
        //}

        /// <summary>
        /// Displays a form and information about the student that will be archived
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ViewResult</returns>
        public ActionResult Archive(int? id)
        {
            if (id == null)
                return Redirect("Index");

            var student = _studentDb.Students.Find(id);//find the student in the database
            if (student == null)//if the student doesn't exist then throw a 404
            {
                return HttpNotFound();
            }

            //Creates the PanelTable and sets its title and the number of 
            //items displayed in a row (includes headers)
            PanelTable viewTable = new PanelTable();
            viewTable.Title = "Student Information";
            viewTable.ItemsPerRow = 4;

            //the key is the the label and the value is the specific student's information
            viewTable.Data = new Dictionary<string, string>()
            {
                { "First Name", student.FirstName},
                {"Last Name",  student.LastName},
                {"Enter Date" , student.EnterDate.ToString()},
                {"Concentration", student.Concentration},
                {"Notes",  student.Notes },
                { "", "" }
            };
            //add the table to the view bag
            ViewBag.Table = viewTable;

            return View(student);
        }
        /// <summary>
        /// Archives student
        /// </summary>
        /// <param name="student"></param>
        /// <returns>RedirectToActionResult</returns>
        [HttpPost, ActionName("Archive")]
        public ActionResult ArchiveConfirmed(int id)
        {
            Student student = _studentDb.Students.Find(id);
            student.Archived = true;
            _studentDb.SaveChanges();//persist the changes
            //add flash message for successful deletion
            TempData["Message"] = new { Message = "Successfully archived student", Type = "success" };
            return RedirectToAction("Index");
        }

        /// <summary>
        /// View archived students
        /// </summary>
        /// <param name="student"></param>
        /// <returns>RedirectToActionResult</returns>
        public ActionResult ViewArchived()
        {
            //DataTable is built using lists of generic objects
            List<object> allStudentProfiles = new List<object>();

            int sysadminID = 0;

            var allStudents = _studentDb.Students.ToList();

            var roles = _userDb.Roles.ToList();
            string tempRoleHolder = "";
            foreach (var student in allStudents)
            {
                if (student.Archived == true)
                {
                    if (tempRoleHolder == "Sysadmin")
                    {
                        ViewBag.SysadminId = student.ID;
                        sysadminID = student.ID;
                    }

                    //adds new object to list - setup like key-value pairs
                    allStudentProfiles.Add(
                        new
                        {
                            id = student.ID,
                            firstName = student.FirstName,
                            lastName = student.LastName,
                            concentration = student.Concentration
                        }
                    );
                }

            }

            DataTableModel studentTable = new DataTableModel();
            studentTable.Title = "Students";

            //sets the table headers, the Field attribute must  match the key value from the allStudentProfiles list
            studentTable.Headers = new List<object>()
            {
                new
                {
                    Name = "First Name",
                    Field = "firstName"
                },
                new
                {
                    Name = "Last Name",
                    Field = "lastName"
                },
                new
                {
                    Name = "Concentration",
                    Field = "concentration"
                },

            };
            studentTable.Data = allStudentProfiles;
            //allows the table to be searched and sorted
            studentTable.SearchSort = true;

            string modifiedTable = studentTable.Render().ToString();

            //render function returns an HtmlString to the view
            return View(new HtmlString(modifiedTable));
        }
    }
}
