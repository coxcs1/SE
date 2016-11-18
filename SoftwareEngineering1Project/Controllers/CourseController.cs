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
using System.Net;

namespace SoftwareEngineering1Project.Controllers
{
    [Authorize(Roles = "professor")]
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
                if(!course.Archived)
                {
                    allCourses.Add(
                    new
                    {
                        id = course.ID,
                        name = course.CourseName,
                        abbr = course.DeptAbbreviation,
                        attributeNum = course.CourseAttributeNumber
                    });
                }                
            }

            DataTableModel courseTable = new DataTableModel();
            courseTable.Title = "Courses";

            courseTable.Headers = new List<object>()
            {
                new
                {
                    Name = "Course Name",
                    Field = "name"
                },

                new
                {
                    Name = "Dept. Abbreviation",
                    Field = "abbr"
                },

                new
                {
                    Name = "Course Number",
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
                },
                new
                {
                    text = "Sections",
                    url = "/sections/index/{{id}}"
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
                    course.DeptAbbreviation = newCourse.DeptAbbreviation;
                    course.CourseAttributeNumber = newCourse.CourseAttributeNumber;

                    //saves the course to the database                    
                    _courseDb.Courses.Add(course);
                    _courseDb.SaveChanges();
                    //add flash message for successful creation
                    TempData["Message"] = new { Message = "Successfully created course", Type = "success" };
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
                {"Course Name" , course.CourseName},
                {"Dept. Abbreviation", course.DeptAbbreviation},
                {"Attribute Number", course.CourseAttributeNumber.ToString() },
                {"","" }
            };
            //key is the url link and the value is what is displayed to the user
            viewTable.TableButtons = new Dictionary<string, string>()
            {
                {"/course/edit/" + course.ID, "Edit" },
                {"/course/delete/" + course.ID, "Delete" },
            };

            //render function returns an HtmlString to the view
            return View(viewTable.Render());
        }

        /// <summary>
        /// Get Edit Action - makes sure there is a course to edit before returning
        /// it to the view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return Redirect("Index");
            }

            Course courseEdit = _courseDb.Courses.Find(id);
            if (courseEdit == null)
            {
                return HttpNotFound();
            }

            return View(courseEdit);
        }

        /// <summary>
        /// Post Edit Action - marks the course as having changed and then
        /// saves the changes
        /// </summary>
        /// <param name="courseEdit"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Course courseEdit)
        {
            if (ModelState.IsValid)
            {
                _courseDb.Entry(courseEdit).State = EntityState.Modified;
                _courseDb.SaveChanges();
                TempData["Message"] = new { Message = "Successfully edited course", Type = "success" };
                return RedirectToAction("Index");
            }
            return View(courseEdit);
        }

        /// <summary>
        /// Get Delete Action - makes sure there is a course to delete before returning
        /// to the view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return Redirect("Index");
            }

            Course courseDelete = _courseDb.Courses.Find(id);
            if (courseDelete == null)
            {
                return HttpNotFound();
            }

            return View(courseDelete);
        }

        /// <summary>
        /// Post Delete Action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            //finds the course and removes it from the database
            Course courseDelete = _courseDb.Courses.Find(id);
            _courseDb.Courses.Remove(courseDelete);
            _courseDb.SaveChanges();
            //add flash message for successful creation
            TempData["Message"] = new { Message = "Successfully deleted coruse", Type = "success" };
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Get Archive Action - makes sure there is a course to archive before returning
        /// to the view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Archive(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = _courseDb.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        /// <summary>
        /// Post Archive Action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Archive"), ValidateAntiForgeryToken]
        public ActionResult ArchiveConfirmed(int id)
        {
            Course course = _courseDb.Courses.Find(id);
            course.Archived = true;
            _courseDb.Entry(course).State = EntityState.Modified;
            _courseDb.SaveChanges();
            TempData["Message"] = new { Message = "Successfully archived test", Type = "success" };
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Archive Index Action - displays a list of all archived courses using a DataTable view helper
        /// </summary>
        /// <returns></returns>
        public ActionResult ArchivedIndex()
        {
            List<object> allCourses = new List<object>();
            var courses = _courseDb.Courses.ToList();

            foreach (var course in courses)
            {
                if (course.Archived)
                {
                    allCourses.Add(
                    new
                    {
                        id = course.ID,
                        name = course.CourseName,
                        abbr = course.DeptAbbreviation,
                        attributeNum = course.CourseAttributeNumber
                    });
                }
            }

            DataTableModel courseTable = new DataTableModel();
            courseTable.Title = "Courses";

            courseTable.Headers = new List<object>()
            {
                new
                {
                    Name = "Course Name",
                    Field = "name"
                },

                new
                {
                    Name = "Dept. Abbreviation",
                    Field = "abbr"
                },

                new
                {
                    Name = "Course Number",
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
                },
                new
                {
                    text = "Sections",
                    url = "/sections/index/{{id}}"
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
    }
}