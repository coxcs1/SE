///////////////////////////////////////////////////////////////////////////////////////////////////
//
//	File Name:         SectionsController.cs
//	Description:       This class handles operations for the Section data model.
//
//	Author:            Dana Jarred Light, lightdj@etsu.edu
//
///////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SoftwareEngineering1Project.DataContexts;
using SoftwareEngineering1Project.Models;
using SoftwareEngineering1Project.Helpers;

namespace SoftwareEngineering1Project.Controllers
{
    public class SectionsController : Controller
    {
        private ApplicationDb db = new ApplicationDb();

        /// <summary>
        /// This function fetches a list of sections for a specific course
        /// and displays them in a data table to be manipulated.
        /// </summary>
        /// <param name="id">Used to identify a course.</param>
        /// <returns></returns>
        public ActionResult Index(int ? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //find the course in the database
            var course = db.Courses.Find(id);
            //create the list of sections for the course
            var sectionsList = new List<object>();
            //build the list of sections
            foreach (var section in course.Sections)
            {
                sectionsList.Add(
                    new
                    {
                        id = section.ID,
                        courseID = section.CourseID,
                        academicYear = section.AcademicYear + " (" + section.Semester.ToString() + ")",
                        teacher = section.Teacher.GetFullName(),
                    }
                );
            }
            //create the sections data table model to hold all sections
            //for the course passed in
            DataTableModel sectionsDataTableModel = new DataTableModel();
            sectionsDataTableModel.
                setTitle("Sections").
                setData(sectionsList).
                setSearchSort(true).
                setHeaders(new List<object>()
                {
                    new
                    {
                        Name = "Teacher",
                        Field = "teacher"
                    },
                    new
                    {
                        Name = "Academic Year",
                        Field = "academicYear"
                    },
                }).
                setActions(new List<object>()//define any links to modify sections
                {
                    new
                    {
                        text = "View All Questions",
                        url = "/sections/viewallquestions/{{id}}"
                    },
                    new
                    {
                        text = "Upload Questions",
                        url = "/questions/uploadquestions/{{id}}"
                    },
                    new
                    {
                        text = "Edit",
                        url = "/sections/edit/{{id}}"
                    },
                    new
                    {
                        text = "Delete",
                        url = "/sections/delete/{{id}}"
                    }
                }).
                setTableButtons(new List<object>()//add a link to create a section
                {
                    new
                    {
                        text = "Add New Section",
                        url = "/sections/create/" + course.ID
                    }
                });

            ViewBag.Table = sectionsDataTableModel.Render();//pass the data table to the view
            return View(course);
        }

        /// <summary>
        /// Displays a form allowing the user to select
        /// a semseter the section is being taught, academic year,
        /// and who is teaching the course.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult Create(int ? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.TeacherID = new SelectList(db.Teachers, "ID", "FirstName");//build a list of teachers
            ViewBag.CourseID = id;//pass the course id to be a hidden element
            ViewBag.Course = db.Courses.Find(id);//pass the course to the view for displaying information
            return View();
        }

        /// <summary>
        /// Creates a section and stores it into the database.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Semester,AcademicYear,CourseID,TeacherID")] Section section)
        {
            if (ModelState.IsValid)
            {
                db.Sections.Add(section);
                db.SaveChanges();
                //add flash message for successful deletion
                TempData["Message"] = new { Message = "Successfully created section", Type = "success" };
                return RedirectToAction("Index", new { id = section.CourseID });
            }

            ViewBag.CourseID = section.CourseID;//pass the course of the section back to the invalid form
            ViewBag.TeacherID = new SelectList(db.Teachers, "ID", "FirstName", section.TeacherID);//pass a list of teachers to the invalid form
            return View(section);
        }

        /// <summary>
        /// Displays a form to edit a section.
        /// </summary>
        /// <param name="id">The identifier of a section</param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Section section = db.Sections.Find(id);
            if (section == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseID = section.CourseID;//passes the course id to the invalid form
            ViewBag.TeacherID = new SelectList(db.Teachers, "ID", "FirstName", section.TeacherID);//passes a list of teachers to the invalid form
            ViewBag.Course = db.Courses.Find(section.CourseID);
            return View(section);
        }

        /// <summary>
        /// Edits a section for a particular course.
        /// </summary>
        /// <param name="section">The section being modified.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Semester,AcademicYear,CourseID,TeacherID")] Section section)
        {
            if (ModelState.IsValid)
            {
                db.Entry(section).State = EntityState.Modified;
                db.SaveChanges();
                //add flash message for successful deletion
                TempData["Message"] = new { Message = "Successfully edited section", Type = "success" };
                return RedirectToAction("Index", new { id = section.CourseID });
            }
            ViewBag.CourseID = section.CourseID;//passes the course id to the invalid form
            ViewBag.TeacherID = new SelectList(db.Teachers, "ID", "FirstName", section.TeacherID);//passes a list of teachers to the invalid form
            return View(section);
        }

        /// <summary>
        /// Deletes a section for a course
        /// </summary>
        /// <param name="id">The identifier of a section.</param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Section section = db.Sections.Find(id);
            if (section == null)
            {
                return HttpNotFound();
            }

            //create a panel table of information about a section
            //to display above the delete form
            PanelTable viewTable = new PanelTable();
            viewTable.Title = "Section Information";
            viewTable.ItemsPerRow = 4;
            viewTable.Data = new Dictionary<string, string>()
            {
                { "Course Name", section.Course.CourseName },
                { "Teacher", section.Teacher.GetFullName() },
                { "Academic Year", section.AcademicYear.ToString() },
                { "Semester", section.Semester.ToString() },
            };

            ViewBag.Table = viewTable.Render();//pass the table html to the view

            return View(section);
        }

        /// <summary>
        /// Deletes a section from the database.
        /// </summary>
        /// <param name="id">The identifier of a section.</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Section section = db.Sections.Find(id);
            db.Sections.Remove(section);
            db.SaveChanges();
            //add flash message for successful deletion
            TempData["Message"] = new { Message = "Successfully deleted section", Type = "success" };
            return RedirectToAction("Index", new { id = section.CourseID });
        }

        public ActionResult ViewAllQuestions(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Section section = db.Sections.Find(id);
            if (section == null)
            {
                return HttpNotFound();
            }

            List<Question> questionsForSection = section.Questions.ToList();

            string questionsAndAnswers = "";

            string html = "<div class='panel panel-default'>" +
                            "<div class='panel-heading'>" +
                                "<h3 class='panel-title'>#question#</h3>" +
                            "</div>" +
                            "<div class='panel-body'>#answer#</div>" +
                            "<div class='panel-footer'>#button#</div>" +
                          "</div>";

            string htmlButton = "<a class='btn btn-primary btn-sm' href='/questions/edit/#id#'>Edit</a>";

            string tempString = "";
            string buttonString = "";
            foreach (Question q in questionsForSection)
            {
                tempString = html.Replace("#question#", q.Text);
                tempString = tempString.Replace("#answer#", q.Answer);            
                buttonString = htmlButton.Replace("#id#", q.ID.ToString());
                tempString = tempString.Replace("#button#", buttonString);
                questionsAndAnswers += tempString;
                questionsAndAnswers += "<br />";
            }

            ViewBag.SectionCourse = section.Course.CourseName + ", " + section.Semester + " " + section.AcademicYear + ", " + section.Teacher.LastName;
            ViewBag.SectionReturn = "/sections/index/" + section.CourseID.ToString();

            return View(new HtmlString(questionsAndAnswers));

        }

        /// <summary>
        /// Releases unmanaged resources and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
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
