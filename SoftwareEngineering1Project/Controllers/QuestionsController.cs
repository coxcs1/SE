﻿///////////////////////////////////////////////////////////////////////////////////////////////////
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
using SoftwareEngineering1Project.ViewModels;
using Microsoft.AspNet.Identity;
using System.IO;
using Microsoft.Office.Interop.Word;


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
            var allSections = questionDb.Sections.ToList();
            var allProfiles = questionDb.Profiles.ToList();
            var allTeachers = questionDb.Teachers.ToList();
            string course_Name = "";
            string profile_Name = "";

            foreach (var question in allQuestions)
            {
                foreach (var section in allSections)
                {
                    if(section.ID == question.SectionID)
                    {
                        course_Name = "CSCI " + section.Course.CourseAttributeNumber +
                            " - " + section.Course.CourseName + " " + section.Semester.ToString() +  " (" + section.AcademicYear + ")";
                    }
                }

                foreach (var profile in allProfiles)
                {
                    if (profile.Id == question.ProfileID)
                    {
                        profile_Name = profile.LastName + ", "+ profile.FirstName ;
                        
                    }
                }

                //adds new object to list - setup like key-value pairs
                allUserQuestions.Add(
                    new
                    {
                        id = question.ID,
                        courseId = course_Name,
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
                    Name = "Section",
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
            var allSections = questionDb.Sections.ToList();
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
                foreach (var section in allSections)
                {
                    if (section.ID == question.SectionID)
                    {
                        course_Name = "CSCI " + section.Course.CourseAttributeNumber +
                            " - " + section.Course.CourseName + section.Semester.ToString() + " (" + section.AcademicYear + ")";
                    }
                }

                foreach (var profile in allProfiles)
                {
                    if (profile.Id == question.ProfileID)
                    {
                        profile_Name = profile.LastName + ", " + profile.FirstName;

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
            ViewBag.SectionID = getSectionsList("Create", null);
            ViewBag.ProfileID = questionDb.Profiles.Single(u => u.UserEmail == User.Identity.Name).Id;
            return View();
        }

        // POST: Questions/Create
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SectionID,ProfileID,Text,Answer")] Question question)
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

            ViewBag.SectionID = getSectionsList("CreatePost", question.SectionID);
            ViewBag.ProfileID = questionDb.Profiles.Single(u => u.UserEmail == User.Identity.Name).Id;
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
            ViewBag.SectionID = getSectionsList("Edit", question.SectionID);
            ViewBag.ProfileID = question.ProfileID;
            return View(question);
        }

        // POST: Questions/Edit/5
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,SectionID,ProfileID,Text,Answer")] Question question)
        {
            if (ModelState.IsValid)
            {
                questionDb.Entry(question).State = EntityState.Modified;
                questionDb.SaveChanges();
                TempData["Message"] = new { Message = "Successfully Updated Question", Type = "success" };
                return RedirectToAction("Index");
            }
            ViewBag.SectionID = getSectionsList("Edit", question.SectionID);
            ViewBag.ProfileID = question.ProfileID;
            return View(question);
        }

        // GET: Questions/Delete/5
        public ActionResult Delete(int? id)
        {
            List<object> allUserQuestions = new List<object>();

            var allQuestions = questionDb.Questions.ToList();
            var allSections = questionDb.Sections.ToList();
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
                foreach (var section in allSections)
                {
                    if (section.ID == question.SectionID)
                    {
                        course_Name = "CSCI " + section.Course.CourseAttributeNumber +
                            " - " + section.Course.CourseName + section.Semester.ToString() + " (" + section.AcademicYear + ")";
                    }
                }

                foreach (var profile in allProfiles)
                {
                    if (profile.Id == question.ProfileID)
                    {
                        profile_Name = profile.LastName + ", " + profile.FirstName;

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


        public ActionResult UploadQuestions(int? id)
        {
            if(id == null)
            {

            }
            else
            {
                int sectionId = (int)id;
            }

            string selectList = "<select id='sectionID' name='sectionID' class='form-control'>";
            string option = "<option value='#val#'>#label#</option>";
            string temp = "";

            List<Models.Section> sections = questionDb.Sections.ToList();

            foreach(Models.Section s in sections)
            {
                temp = option.Replace("#val#", s.ID.ToString());
                temp = temp.Replace("#label#", s.Course.CourseName + ", " + s.AcademicYear + ", " + s.Teacher.LastName);

                selectList += temp;

            }

            selectList += "</select>";

            ViewBag.SectionSelectList = selectList;

            return View();
        }

        [HttpPost]
        public ActionResult UploadQuestions(HttpPostedFileBase file, int sectionId)
        {
            string path = "";
            
            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                //saves the file in the app_data folder and then opens it
                path = Path.Combine(Server.MapPath("~/App_Data"), Path.GetFileName(file.FileName));            
                file.SaveAs(path);

                Application app = new Application();
                Document doc = app.Documents.Open(path);

                List<string> questions = new List<string>();
                List<string> answers = new List<string>();

                string currentAnswer = "";

                for (int i = 1; i <= doc.Paragraphs.Count; i++)
                {
                    //grabs the first nine characters and checks to see if it specifies a question to follow  
                    string checkForQuestion = new string(doc.Paragraphs[i].Range.Text.Trim().Take(9).ToArray());

                    if (checkForQuestion == "Question:")
                    {
                        //if it is a question it skips the first nine characters and adds it to the questions list
                        string temp  = new string(doc.Paragraphs[i].Range.Text.Trim().Skip(9).ToArray());
                        questions.Add("<p>" + temp + "</p>");

                        //if there was a previous question, adds the previous's question to the answer list
                        if (currentAnswer != "")
                        {
                            answers.Add(currentAnswer);
                            currentAnswer = "";
                        }

                    }
                    //blank line in the document - skip
                    else if (doc.Paragraphs[i].Range.Text.Trim() == "")
                    {
                        continue;
                    }
                    //concatenates the answer if it appears in multiple paragraphs into a single string
                    else
                    {
                        currentAnswer += "<p>" + doc.Paragraphs[i].Range.Text.Trim() + "</p><br />";
                    }
                }
                //adds the last answer from the end of the document
                answers.Add(currentAnswer);

                //closes the document and app and then deletes the file from the server
                doc.Close();
                app.Quit();            
                System.IO.File.Delete(path);
                
                Models.Section selectedSection = questionDb.Sections.Find(sectionId);

                //if the document was formatted correcctly - adds the questions to the database
                if (questions.Count == answers.Count)
                {
                    for (int i = 0; i < questions.Count; i++)
                    {
                        Question newQ = new Question();
                        newQ.Profile = questionDb.Profiles.Single(u => u.UserEmail == User.Identity.Name);
                        newQ.Answer = answers[i];
                        newQ.Section = selectedSection;
                        newQ.Text = questions[i];
                        questionDb.Questions.Add(newQ);
                        questionDb.SaveChanges();
                    }
                    TempData["Message"] = new { Message = "Successfully Uploaded Questions", Type = "success" };
                    return RedirectToAction("Index");
                }                            
            }
            return RedirectToAction("UploadQuestinos", new  { id = sectionId });


        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                questionDb.Dispose();
            }
            base.Dispose(disposing);
        }

        public IEnumerable<SelectListItem> getSectionsList(string type, int ? selectedID)
        {
            var sections = questionDb.Sections;

            switch (type)
            {
                case "Create":
                    IEnumerable<SelectListItem> selectList =
                        from section in sections
                        select new SelectListItem
                        {
                            Text = section.Course.CourseName + " " + section.Semester.ToString() + " (" + section.AcademicYear + ")",
                            Value = section.ID.ToString()
                        };
                    return selectList;
                case "CreatePost":
                    IEnumerable<SelectListItem> createPostSelectList =
                        from section in sections
                        select new SelectListItem
                        {
                            Text = section.Course.CourseName + " " + section.Semester.ToString() + " (" + section.AcademicYear + ")",
                            Value = section.ID.ToString(),
                            Selected = (section.ID == selectedID) ? true : false,
                            
                        };
                    return createPostSelectList;
                case "Edit":
                    IEnumerable<SelectListItem> editSelectList =
                        from section in sections
                        select new SelectListItem
                        {
                            Text = section.Course.CourseName + " " + section.Semester.ToString() + " (" + section.AcademicYear + ")",
                            Value = section.ID.ToString(),
                            Selected = (section.ID == selectedID) ? true : false,

                        };
                    return editSelectList;
                default:
                    throw new Exception("Invalid Type Passed.");
            }
        }
    }
}
