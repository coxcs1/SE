///////////////////////////////////////////////////////////////////////////////////////////////////
//
//	File Name:         CourseController.cs
//	Description:       This class contains action methods associated with the CourseController. 
//
//	Author:            Jonathan Bible, zjbb44@etsu.edu
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
    [Authorize]
    public class CourseController : Controller
    {
        private ApplicationDb _courseDb = new ApplicationDb();

        protected override void Dispose(bool disposing)
        {
            _courseDb.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Index Action - displays a list of all courses using a DataTable view helper
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            List<object> allCourses = new List<object>();
            var courses = _courseDb.Courses.ToList();

            foreach (var course in courses)
            {
                allCourses.Add(
                    new
                    {
                        id = course.ID,
                        name = course.CourseName,
                        crn = course.CourseCRN,
                        attributeNum = course.CourseAttributeNumber
                    });
            }

            DataTableModel courseTable = new DataTableModel();
            courseTable.Title = "Courses";

            courseTable.Headers = new List<object>()
            {
                new
                {
                    Name = "Course ID",
                    Field = "id"
                },

                new
                {
                    Name = "Course Name",
                    Field = "name"
                },

                new
                {
                    Name = "CRN",
                    Field = "crn"
                },

                new
                {
                    Name = "Attribute Number",
                    Field = "attributeNum"
                }
            };
            courseTable.Data = allCourses;


            courseTable.Actions = new List<object>()
            {
                new
                {
                    text = "View",
                    url = "/course/view/{{id}}"
                },

                new
                {
                    text = "Edit",
                    url = "/course/edit/{{id}}"
                },

                new
                {
                    text = "Delete",
                    url = "/course/delete/{{id}}"
                }
            };

            courseTable.SearchSort = true;
            courseTable.TableButtons = new List<object>()
            {
                new
                {
                    text = "Add Course",
                    url = "/course/add"
                }
            };

            string modifiedTable = courseTable.Render().ToString();
            return View(new HtmlString(modifiedTable));
        }

        /// <summary>
        /// Get Add action
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            //uses a ViewModel to be able to store all needed information
            return View();
        }

        /// <summary>
        /// Post Add Action
        /// </summary>
        /// <param name="newCourse"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(Course newCourse)
        {
            //checks to make sure everything was filled out appropriately in accordance
            //to the tags in the ViewModel
            if (ModelState.IsValid)
            {
                //makes sure a course with this ID has not already been added
                if (!_courseDb.Courses.Any(p => p.ID == newCourse.ID))
                {
                    //creates the course from the information entered
                    Course course = new Course();
                    course.ID = newCourse.ID;
                    course.CourseName = newCourse.CourseName;
                    course.CourseCRN = newCourse.CourseCRN;
                    course.CourseAttributeNumber = newCourse.CourseAttributeNumber;

                    //saves the course to the database                    
                    _courseDb.Courses.Add(course);
                    _courseDb.SaveChanges();
                    return RedirectToAction("Index", "Course");
                }
                else
                {
                    ModelState.AddModelError("ID", "You have entered a duplicate course. Please try again.");
                }
            }

            return View();
            
        }

        /// <summary>
        /// The View Action - displays the course information using a PanelTable ViewHelper
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult View(int? id)
        {
            if (id == null)
            {
                return Redirect("Index");
            }

            Course course = _courseDb.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }

            //Creates the PanelTable and sets its title and the number of 
            //items displayed in a row (includes headers)
            PanelTable viewTable = new PanelTable();
            viewTable.Title = "Course Information";
            viewTable.ItemsPerRow = 4;

            //the key is the the label and the value is the specific course's information
            viewTable.Data = new Dictionary<string, string>()
            {
                {"Course ID",  course.ID.ToString()},
                {"Course Name" , course.CourseName},
                {"CRN", course.CourseCRN.ToString()},
                {"Attribute Number", course.CourseAttributeNumber.ToString() }
            };
            //key is the url link and the value is what is displayed to the user
            viewTable.TableButtons = new Dictionary<string, string>()
            {
                {"/course/edit/" + course.ID, "Edit" },
            };

            //render function returns an HtmlString to the view
            return View(viewTable.Render());
        }
    }
}