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
    [Authorize(Roles = "sysadmin")]
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

            DataTableModel studentTable = new DataTableModel();
            studentTable.Title = "Students";

            //sets the table headers, the Field attribute must match the key value from the allStudentProfiles list
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
                    text = "Delete",
                    url = "/student/delete/{{id}}"
                }
            };
            //allows the table to be searched and sorted
            studentTable.SearchSort = true;
            //adds the table button for adding a user
            studentTable.TableButtons = new List<object>()
            {
                new
                {
                    text = "Create Student",
                    url = "/student/add"
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
            //uses a ViewModel to be able to store all needed information
            Student newStudent = new Student();
            return View(newStudent);
        }

        /// <summary>
        /// Post Add Action
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(Student newStudent)
        {
            //checks to make sure everything was filled out appropriately in accordance
            //to the tags in the ViewModel
            if (ModelState.IsValid)
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
                return RedirectToAction("Index", "Student");
            }

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
            else
            {
                return View(studentInfo);
            }
            
        }
            
        }
    }
