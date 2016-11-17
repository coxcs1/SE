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
using System.Text.RegularExpressions;
using SoftwareEngineering1Project.Helpers;
using System.Web.Script.Serialization;

namespace SoftwareEngineering1Project.Controllers
{
    public class TestsController : Controller
    {
        private ApplicationDb db = new ApplicationDb();

        // GET: Tests
        public ActionResult Index()
        {
            var tests = db.Tests.Include(t => t.Student);
            //build a list of tests for the data table
            var testsList = new List<object>();
            foreach (var test in tests)
            {
                if (!test.Archived)
                {
                    //adds new object to list - setup like key-value pairs
                    testsList.Add(
                        new
                        {
                            id = test.ID,
                            dateTaken = test.DateTaken.ToShortDateString(),
                            student = test.Student.FirstName + " " + test.Student.LastName,
                            passed = (test.Passed) ? "Yes" : "No"
                        }
                    );
                }
            }

            //create the data table for the index page
            //I used the method chaining that I build into the DataTableModel
            DataTableModel testsDataTableModel = new DataTableModel();
            testsDataTableModel.
                setTitle("Tests").//set the title
                setData(testsList).//pass in the teacher list
                setSearchSort(true).//initializes the jQuery data table library
                setHeaders(new List<object>()//add the headers and map them to the teachersList data
                {
                    new
                    {
                        Name = "Student",
                        Field = "student"
                    },
                    new
                    {
                        Name = "Date Taken",
                        Field = "dateTaken"
                    },
                    new
                    {
                        Name = "Passed",
                        Field = "passed"
                    }
                }).
                setActions(new List<object>()//define any links to modify teachers
                {
                    new
                    {
                        text = "Administer",
                        url = "/tests/administertest/{{id}}"
                    },
                    new
                    {
                        text = "Archive",
                        url = "/tests/archive/{{id}}"
                    }
                }).
                setTableButtons(new List<object>()//add a link to create a teacher
                {
                    new
                    {
                        text = "Create Test",
                        url = "/tests/create"
                    },
                    new
                    {
                        text = "Archived Tests",
                        url = "/tests/archivedindex"
                    }
                });


            return View(testsDataTableModel.Render());
        }

        public ActionResult ArchivedIndex()
        {
            var tests = db.Tests.Where(t => t.Archived == true).ToList();
            //build a list of tests for the data table
            var testsList = new List<object>();
            foreach (var test in tests)
            {
                //adds new object to list - setup like key-value pairs
                testsList.Add(
                    new
                    {
                        id = test.ID,
                        dateTaken = test.DateTaken.ToShortDateString(),
                        student = test.Student.FirstName + " " + test.Student.LastName,
                        passed = (test.Passed) ? "Yes" : "No"
                    }
                );
            }

            //create the data table for the index page
            //I used the method chaining that I build into the DataTableModel
            DataTableModel testsDataTableModel = new DataTableModel();
            testsDataTableModel.
                setTitle("Tests").//set the title
                setData(testsList).//pass in the teacher list
                setSearchSort(true).//initializes the jQuery data table library
                setHeaders(new List<object>()//add the headers and map them to the teachersList data
                {
                    new
                    {
                        Name = "Student",
                        Field = "student"
                    },
                    new
                    {
                        Name = "Date Taken",
                        Field = "dateTaken"
                    },
                    new
                    {
                        Name = "Passed",
                        Field = "passed"
                    }
                });

            return View(testsDataTableModel.Render());
        }

        // GET: Tests/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test test = db.Tests.Find(id);
            if (test == null)
            {
                return HttpNotFound();
            }
            return View(test);
        }

        // GET: Tests/Create
        public ActionResult Create()
        {
            ViewBag.StudentID = new SelectList(db.Students, "ID", "FirstName");
            return View();
        }

        // POST: Tests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,StudentID,DateTaken")] Test test)
        {
            if (ModelState.IsValid)
            {
                //add the test to the database
                db.Tests.Add(test);
                db.SaveChanges();

                CreateTestQuestions(test);

                TempData["Message"] = new { Message = "Successfully Created Test", Type = "success" };
                return RedirectToAction("Index");
            }

            ViewBag.StudentID = new SelectList(db.Students, "ID", "FirstName", test.StudentID);
            return View(test);
        }

        // GET: Tests/Delete/5
        public ActionResult Archive(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test test = db.Tests.Find(id);
            if (test == null)
            {
                return HttpNotFound();
            }
            return View(test);
        }

        // POST: Tests/Delete/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public ActionResult ArchiveConfirmed(int id)
        {
            Test test = db.Tests.Find(id);
            test.Archived = true;//make the test archived
            db.Entry(test).State = EntityState.Modified;//let the ORM know object has been modified
            db.SaveChanges();//save the changes
            TempData["Message"] = new { Message = "Successfully archived test", Type = "success" };
            return RedirectToAction("Index");
        }

        public ActionResult GetTestResults(int? id)
        {
            Test test = db.Tests.Find(id);
            List<object> questions = new List<object>();
            foreach (var testQuestion in test.TestQuestions)
            {
                questions.Add(
                    new {
                        Section = testQuestion.Question.Section.Course.CourseName,
                        Question = Regex.Replace(testQuestion.Question.Text, "<.*?>", String.Empty).Replace("&nbsp;", " "),
                        Score = testQuestion.QuestionScore
                    }    
                );
            }

            return Json(questions, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ScoreQuestion(FormCollection data)
        {
            var Score = Int32.Parse(data["score"]);
            var testQuestion = db.TestQuestions.Find(Int32.Parse(data["id"]));
            testQuestion.QuestionScore = Score;
            db.Entry(testQuestion).State = EntityState.Modified;
            db.SaveChanges();
            //add flash message for successful creation
            return Json(new { Success = "Successfully scored test" });
        }

        public ActionResult NewQuestion(int? id)
        {
            TestQuestion oldTestQuestion = db.TestQuestions.Find(id);
            Test test = db.Tests.Find(oldTestQuestion.TestID);
            Section section = db.Sections.Find(oldTestQuestion.Question.SectionID);

            List<Question> possibleQuestions = section.Questions.ToList(); // all questions eligible for this section
            possibleQuestions = ShuffleQuestions(possibleQuestions);

            if (section.Course.Core)
            {
                if (possibleQuestions.Count <= 5)
                {
                    return Json(new { Message = "No replacement question was found!" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (possibleQuestions.Count <= 3)
                {
                    return Json(new { Message = "No replacement question was found!" }, JsonRequestBehavior.AllowGet);
                }
            }

            Boolean foundQuestion = false; // determines whether a new question has been found
            Boolean match = false;  // determines whether the possible question matches an existing question on the test
            int remainingQuestions = possibleQuestions.Count; // determines how many possible questions remain
            int step = 0; // controls which possible question is being checked
            Question possibleQuestion = new Question(); // placeholder for potential replacement question

            while (!foundQuestion)
            {
                possibleQuestion = possibleQuestions[step];

                foreach (TestQuestion tq in test.TestQuestions)
                {
                    if (possibleQuestion.ID == tq.QuestionID)
                    {
                        // indicates a match was found if the possible question is the same as one of the
                        // existing questions
                        match = true;
                    }
                }

                if (!match)
                {
                    // indicates the possible question is qualified to be the replacement question
                    foundQuestion = true;
                }
                else
                {
                    remainingQuestions--;
                    step++;
                    match = false;

                    if (remainingQuestions <= 0)
                    {
                        // sends an error if all possible questions are already part of the test
                        return Json(new { Message = "No replacement question was found!" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            //change out the question for that test question
            oldTestQuestion.Question = possibleQuestion;
            db.Entry(oldTestQuestion).State = EntityState.Modified;
            db.SaveChanges();

            // builds question as JSON
            var jsonQuestion = new
            {
                ID = oldTestQuestion.ID,
                Text = Regex.Replace(oldTestQuestion.Question.Text, "<.*?>", String.Empty).Replace("&nbsp;", " "),
                Answer = Regex.Replace(oldTestQuestion.Question.Answer, "<.*?>", String.Empty).Replace("&nbsp;", " "),
            };

            return Json(jsonQuestion, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ScoreTest(FormCollection data)
        {
            var passFail = Boolean.Parse(data["passFail"]);
            var test = db.Tests.Find(Int32.Parse(data["id"]));
            test.Passed = passFail;
            db.Entry(test).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { Success = "Successfully scored test", Redirect = "/tests/index" });
        }

        public ActionResult AdministerTest(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test test = db.Tests.Find(id);
            if (test == null)
            {
                return HttpNotFound();
            }
            if(test.Student.Sections.Count == 0)
            {
                TempData["Message"] = new { Message = "There not enough questions in this section!", Type = "error" };
                return RedirectToAction("Index");
            }
            var stepCounter = 1;//step counter
            var totalQuestionCounter = 1;//keeps track of total questions (used in field names)
            var administrationModelData = new List<object>();//json model data
            foreach (var section in test.Student.Sections)//loop through each section building stages for the administration process
            {
                var questionCount = 1;//question counter
                var step = new { Step = stepCounter, Section = section.Course.CourseName, Questions = new List<object>() };//set up step object
                foreach (var question in test.TestQuestions)//loop through each test question and add question data
                {
                    if(section.Course.Core == true)
                    {
                        if(section.Questions.Count < 5)
                        {
                            //insert error here
                            TempData["Message"] = new { Message = "There not enough questions in this section!", Type = "error" };
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        if (section.Questions.Count < 3)
                        {
                            //insert error here
                            TempData["Message"] = new { Message = "There not enough questions in this section!", Type = "error" };
                            return RedirectToAction("index");
                        }
                    }
                    if (question.Question.Section.CourseID == section.CourseID)
                    {
                        step.Questions.Add(new
                        {
                            ID = question.ID,
                            Label = "Question " + questionCount,
                            FieldName = "question" + totalQuestionCounter,
                            Text = Regex.Replace(question.Question.Text, "<.*?>",String.Empty).Replace("&nbsp;", " "),
                            Answer = Regex.Replace(question.Question.Answer, "<.*?>", String.Empty).Replace("&nbsp;", " "),
                            Score = question.QuestionScore
                        });
                        questionCount++;//Question counter incrementation
                        totalQuestionCounter++;//increment total questions
                    }
                }
                administrationModelData.Add(step);//add the step to the overall data
                stepCounter++;//increment the step
            }

            var json = Json(administrationModelData);//build a JSONResult
            var testJson = Json(new { TestID = test.ID, PassFail = test.Passed });
            ViewBag.ViewModel = new JavaScriptSerializer().Serialize(json.Data);//serialize the json data into a string
            ViewBag.TestModel = new JavaScriptSerializer().Serialize(testJson.Data);//pass test data to the front end
            ViewBag.StudentName = test.Student.FirstName + " " + test.Student.LastName;//pass the student's name to the view

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void CreateTestQuestions(Test t)
        {
            Test newT = db.Tests.Find(t.ID);
            Student stu = db.Students.Find(t.StudentID);

            foreach (Section sec in stu.Sections)
            {
                List<Question> unshuffledQuestions = sec.Questions.ToList();
                List<Question> shuffledQuestions = ShuffleQuestions(unshuffledQuestions);

                if (true)// course is a core course
                {

                    for (int i = 0; i < 5; i++)
                    {
                        TestQuestion tq = new TestQuestion();
                        tq.Question = shuffledQuestions[i];
                        tq.Test = newT;
                        db.TestQuestions.Add(tq);
                        db.SaveChanges();
                    }
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        TestQuestion tq = new TestQuestion();
                        tq.Question = shuffledQuestions[i];
                        tq.Test = newT;
                        db.TestQuestions.Add(tq);
                        db.SaveChanges();
                    }
                }

                
            }
        }

        private List<Question> ShuffleQuestions(List<Question> qs)
        {
            Random r = new Random();

            for(int i = 0; i < 5; i++)
            {
                int randomIndex1 = r.Next(0, qs.Count);
                int randomIndex2 = r.Next(0, qs.Count);

                Question temp = qs[randomIndex1];
                qs[randomIndex1] = qs[randomIndex2];
                qs[randomIndex2] = temp;                
            }

            return qs;
        }
    }
}
