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
    [Authorize(Roles = "professor")]
    public class QuestionsController : Controller
    {
        private ApplicationDb questionDb = new ApplicationDb();       

        // GET: Questions
        public ActionResult Index()
        {
            //DataTable is built using lists of generic objects
            List<object> allUserQuestions = new List<object>();

            var allQuestions = questionDb.Questions.ToList();
            var allCourses = questionDb.Courses.ToList();
            var allProfiles = questionDb.Profiles.ToList();
            string course_Name = "";
            string profile_First_Name = "";

            foreach (var question in allQuestions)
            {
                foreach (var course in allCourses)
                {
                    if(course.ID == question.CourseID)
                    {
                        course_Name = course.CourseName;
                    }
                }

                foreach (var profile in allProfiles)
                {
                    if (profile.Id == question.ProfileID)
                    {
                        profile_First_Name = profile.LastName + ", "+ profile.FirstName ;
                        
                    }
                }

                //adds new object to list - setup like key-value pairs
                allUserQuestions.Add(
                    new
                    {
                        id = question.ID,
                        courseId = course_Name,
                        profileId = profile_First_Name,
                        text = question.Text,
                        answer = question.Answer

                    }
                );
            }


            DataTableModel questionTable = new DataTableModel();
            questionTable.
            setTitle("Questions").
            setData(allUserQuestions).
            //allows the table to be searched and sorted
            setSearchSort(true).
            //sets the table headers, the Field attribute must match the key value from the allUserProfiles list
            setHeaders(new List<object>()
            {
                new
                {
                    Name = "Course",
                    Field = "courseId"
                },
                new
                {
                    Name = "Created By",
                    Field = "profileId"
                },
                new
                {
                    Name = "Question",
                    Field = "text"
                },
                new
                {
                    Name = "Answer",
                    Field = "answer"
                }



            });

            //sets the actions that will display for every entry in the table
            //uses a str replace to replace the {{id}} values with the actual id
            //id matches to the key seen earlier
            questionTable.Actions = new List<object>()
            {
                
                new
                {
                    text = "Edit",
                    url = "/questions/edit/{{id}}"
                },
                new
                {
                    text = "Delete",
                    url = "/questions/delete/{{id}}"
                }
            };
            
            
            //adds the table button for creating a question
            questionTable.TableButtons = new List<object>()
            {
                new
                {
                    text = "Create Question",
                    url = "/Questions/Create"
                }
            };

            string Table = questionTable.Render().ToString();

            //render function returns an HtmlString to the view
            return View(new HtmlString(Table));
        }

        // GET: Questions/Create
        public ActionResult Create()
        {
            ViewBag.CourseID = new SelectList(questionDb.Courses, "ID", "CourseName");
            ViewBag.ProfileID = new SelectList(questionDb.Profiles, "Id", "UserEmail");
            ViewBag.TeacherID = new SelectList(questionDb.Teachers, "ID", "FullName");
            return View();
        }

        // POST: Questions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseID,TeacherID,ProfileID,Text,Answer")] Question question)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    questionDb.Questions.Add(question);
                    questionDb.SaveChanges();
                    return RedirectToAction("Index");
                }

            }
            catch (DataException)
            {
               
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            ViewBag.CourseID = new SelectList(questionDb.Courses, "ID", "CourseName", question.CourseID);
            ViewBag.ProfileID = new SelectList(questionDb.Profiles, "Id", "UserEmail", question.ProfileID);
            ViewBag.TeacherID = new SelectList(questionDb.Teachers, "ID", "FullName", question.TeacherID);
            return View(question);
        }

        // GET: Questions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = questionDb.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseID = new SelectList(questionDb.Courses, "ID", "CourseName", question.CourseID);
            ViewBag.ProfileID = new SelectList(questionDb.Profiles, "Id", "UserEmail", question.ProfileID);
            ViewBag.TeacherID = new SelectList(questionDb.Teachers, "ID", "FullName", question.TeacherID);
            return View(question);
        }

        // POST: Questions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CourseID,TeacherID,ProfileID,Text,Answer")] Question question)
        {
            if (ModelState.IsValid)
            {
                questionDb.Entry(question).State = EntityState.Modified;
                questionDb.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseID = new SelectList(questionDb.Courses, "ID", "CourseName", question.CourseID);
            ViewBag.ProfileID = new SelectList(questionDb.Profiles, "Id", "UserEmail", question.ProfileID);
            ViewBag.TeacherID = new SelectList(questionDb.Teachers, "ID", "FullName", question.TeacherID);
            return View(question);
        }

        // GET: Questions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = questionDb.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Question question = questionDb.Questions.Find(id);
            questionDb.Questions.Remove(question);
            questionDb.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                questionDb.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
