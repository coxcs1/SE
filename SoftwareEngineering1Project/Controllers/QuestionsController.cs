///////////////////////////////////////////////////////////////////////////////////////////////////
//
//	File Name:         QuestionsController.cs
//	Description:       This class contains action methods associated with the questionsController. 
//
//	Author:            Seth Howerton, howertons@etsu.edu
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
            var allTeachers = questionDb.Teachers.ToList();
            string course_Name = "";
            string profile_Name = "";
            string teacher_Name = "";

            foreach (var question in allQuestions)
            {
                foreach (var course in allCourses)
                {
                    if(course.ID == question.CourseID)
                    {
                        course_Name = "CSCI " + course.CourseAttributeNumber + " - " + course.CourseName;
                    }
                }

                foreach (var profile in allProfiles)
                {
                    if (profile.Id == question.ProfileID)
                    {
                        profile_Name = profile.LastName + ", "+ profile.FirstName ;
                        
                    }
                }

                foreach (var teacher in allTeachers)
                {
                    if (teacher.ID == question.TeacherID)
                    {
                        teacher_Name = teacher.LastName + ", " + teacher.FirstName;

                    }
                }

                //adds new object to list - setup like key-value pairs
                allUserQuestions.Add(
                    new
                    {
                        id = question.ID,
                        courseId = course_Name,
                        teacherId = teacher_Name,
                        profileId = profile_Name,
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
                    Name = "Professor",
                    Field = "teacherId"
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
                    text = "View",
                    url = "/questions/view/{{id}}"
                },

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

        // GET: Questions/View/5
        public ActionResult View(int? id)
        {
            List<object> allUserQuestions = new List<object>();

            var allQuestions = questionDb.Questions.ToList();
            var allCourses = questionDb.Courses.ToList();
            var allProfiles = questionDb.Profiles.ToList();
            var allTeachers = questionDb.Teachers.ToList();
            string course_Name = "";
            string profile_Name = "";
            string teacher_Name = "";

            if (id == null)
            {
                return Redirect("Index");
            }

            Question question = questionDb.Questions.Find(id);

            if (question == null)
            {
                return HttpNotFound();
            }

            //Creates the PanelTable and sets its title and the number of 
            //items displayed in a row (includes headers)
            PanelTable viewTable = new PanelTable();
            viewTable.Title = "Question Information";
            viewTable.ItemsPerRow = 5;

            foreach (var questions in allQuestions)
            {
                foreach (var course in allCourses)
                {
                    if (course.ID == question.CourseID)
                    {
                        course_Name = "CSCI " + course.CourseAttributeNumber + " - " + course.CourseName;
                    }
                }

                foreach (var profile in allProfiles)
                {
                    if (profile.Id == question.ProfileID)
                    {
                        profile_Name = profile.LastName + ", " + profile.FirstName;

                    }
                }

                foreach (var teacher in allTeachers)
                {
                    if (teacher.ID == question.TeacherID)
                    {
                        teacher_Name = teacher.LastName + ", " + teacher.FirstName;

                    }
                }
            }

                //the key is the the label and the value is the specific course's information
                viewTable.Data = new Dictionary<string, string>()
            {
                {"Course Name:" , course_Name},
                {"Professor:", teacher_Name},
                {"Created By:", profile_Name},
                {"Question:", question.Text},
                {"Answer:", question.Answer },
                {"", "" }
            };
            //key is the url link and the value is what is displayed to the user
            viewTable.TableButtons = new Dictionary<string, string>()
            {
                {"/questions/edit/" + question.ID, "Edit" },
                {"/questions/delete/" + question.ID, "Delete" }
            };

            //render function returns an HtmlString to the view
            return View(viewTable.Render());
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
                    //add flash message for successful creation
                    TempData["Message"] = new { Message = "Successfully Created Question", Type = "success" };
                    return RedirectToAction("Index", "Questions");
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
                TempData["Message"] = new { Message = "Successfully Updated Question", Type = "success" };
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
            List<object> allUserQuestions = new List<object>();

            var allQuestions = questionDb.Questions.ToList();
            var allCourses = questionDb.Courses.ToList();
            var allProfiles = questionDb.Profiles.ToList();
            var allTeachers = questionDb.Teachers.ToList();
            string course_Name = "";
            string profile_Name = "";
            string teacher_Name = "";

            if (id == null)
            {
                return Redirect("Index");
            }

            Question question = questionDb.Questions.Find(id);

            if (question == null)
            {
                return HttpNotFound();
            }

            //Creates the PanelTable and sets its title and the number of 
            //items displayed in a row (includes headers)
            PanelTable viewTable = new PanelTable();
            viewTable.Title = "Question Information";
            viewTable.ItemsPerRow = 5;

            foreach (var questions in allQuestions)
            {
                foreach (var course in allCourses)
                {
                    if (course.ID == question.CourseID)
                    {
                        course_Name = "CSCI " + course.CourseAttributeNumber + " - " + course.CourseName;
                    }
                }

                foreach (var profile in allProfiles)
                {
                    if (profile.Id == question.ProfileID)
                    {
                        profile_Name = profile.LastName + ", " + profile.FirstName;

                    }
                }

                foreach (var teacher in allTeachers)
                {
                    if (teacher.ID == question.TeacherID)
                    {
                        teacher_Name = teacher.LastName + ", " + teacher.FirstName;

                    }
                }
            }

            //the key is the the label and the value is the specific course's information
            viewTable.Data = new Dictionary<string, string>()
            {
                {"Course Name:" , course_Name},
                {"Professor:", teacher_Name},
                {"Created By:", profile_Name},
                {"Question:", question.Text},
                {"Answer:", question.Answer },
                {"","" }
            };

            //render function returns an HtmlString to the view
            return View(viewTable.Render());
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Question question = questionDb.Questions.Find(id);
            questionDb.Questions.Remove(question);
            questionDb.SaveChanges();
            TempData["Message"] = new { Message = "Successfully Deleted Question", Type = "success" };
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
