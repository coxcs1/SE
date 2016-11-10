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
                //adds new object to list - setup like key-value pairs
                testsList.Add(
                    new
                    {
                        id = test.ID,
                        dateTaken = test.DateTaken.ToShortDateString(),
                        student = test.Student.FirstName + " " + test.Student.LastName,
                        passed  = (test.Passed) ? "Yes" : "No"
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
                        text = "Edit",
                        url = "/tests/edit/{{id}}"
                    },
                    new
                    {
                        text = "Delete",
                        url = "/tests/delete/{{id}}"
                    }
                }).
                setTableButtons(new List<object>()//add a link to create a teacher
                {
                    new
                    {
                        text = "Create Test",
                        url = "/tests/create"
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

        // GET: Tests/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.StudentID = new SelectList(db.Students, "ID", "FirstName", test.StudentID);
            return View(test);
        }

        // POST: Tests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,StudentID,Passed,DateTaken")] Test test)
        {
            if (ModelState.IsValid)
            {
                var testQuestions = db.TestQuestions.Where(tq => tq.TestID == test.ID);
                foreach (var question in testQuestions)
                {
                    db.TestQuestions.Remove(question);
                }
                //save the test
                db.Entry(test).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            ViewBag.StudentID = new SelectList(db.Students, "ID", "FirstName", test.StudentID);
            return View(test);
        }

        // GET: Tests/Delete/5
        public ActionResult Delete(int? id)
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Test test = db.Tests.Find(id);
            db.Tests.Remove(test);
            db.SaveChanges();
            return RedirectToAction("Index");
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

            var stepCounter = 1;//step counter
            var administrationModelData = new List<object>();//json model data
            foreach (var section in test.Student.Sections)//loop through each section building stages for the administration process
            {
                var questionCount = 1;//question counter
                var step = new { Step = stepCounter, Section = section.Course.CourseName, Questions = new List<object>() };//set up step object
                foreach (var question in test.TestQuestions)//loop through each test question and add question data
                {
                    step.Questions.Add(new {
                        ID = question.ID,
                        Label = "Question " + questionCount,
                        FieldName = "question" + questionCount,
                        Text = question.Question.Text,
                        Answer = question.Question.Answer,
                        Score = question.QuestionScore
                    });
                    questionCount++;//Question counter incrementation
                }
                administrationModelData.Add(step);//add the step to the overall data
                stepCounter++;//increment the step
            }

            var json = Json(administrationModelData);
            ViewBag.ViewModel = new JavaScriptSerializer().Serialize(json.Data);

            return View();
        }

        /// <summary>
        /// Fetches all the questions for a test and returns them in JSON format.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult GetTestQuestions(int? id)
        {
            if (id == null)//throw bad request of no id is passed
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test test = db.Tests.Find(id);//find the test in the db
            if (test == null)//if no test is found then throw a 404
            {
                return HttpNotFound();
            }
            var testQuestions = new List<object>();//create a generic list of objects for JSON formatting
            foreach (var testQuestion in test.TestQuestions)//build the generic list
            {
                testQuestions.Add(
                    new
                    {
                        ID = testQuestion.Question.ID,
                        Text = Regex.Replace(
                            testQuestion.Question.Section.Course.CourseName + " (" +
                            testQuestion.Question.Section.Semester + " " +
                            testQuestion.Question.Section.AcademicYear + ") - " +
                            testQuestion.Question.Text, "<.*?>", String.Empty
                            ),
                        Answer = testQuestion.Question.Answer
                    });
            }
            return Json(testQuestions, JsonRequestBehavior.AllowGet);//pass the JSON string to the view for JavaScript processing
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
