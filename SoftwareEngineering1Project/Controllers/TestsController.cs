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

namespace SoftwareEngineering1Project.Controllers
{
    public class TestsController : Controller
    {
        private ApplicationDb db = new ApplicationDb();

        // GET: Tests
        public ActionResult Index()
        {
            var tests = db.Tests.Include(t => t.Student);
            return View(tests.ToList());
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
        public ActionResult Create([Bind(Include = "ID,StudentID,Passed,DateTaken,question*")] Test test, FormCollection formData)
        {
            if (ModelState.IsValid)
            {
                //add the test to the database
                db.Tests.Add(test);
                db.SaveChanges();
                //add each test question
                foreach (String element in formData)
                {
                    if (element.Contains("question"))
                    {
                        var testQuestion = new TestQuestion(test.ID, Int32.Parse(formData[element]), 100);
                        db.TestQuestions.Add(testQuestion);//add test question to list
                    }
                }
                db.SaveChanges();//save after adding all the test questions

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
        public ActionResult Edit([Bind(Include = "ID,StudentID,Passed,DateTaken")] Test test, FormCollection formData)
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

                //add new test questions selected
                foreach (String element in formData)
                {
                    if (element.Contains("question"))
                    {
                        var testQuestion = new TestQuestion(test.ID, Int32.Parse(formData[element]), 100);
                        db.TestQuestions.Add(testQuestion);//add test question to list
                    }
                }
                db.SaveChanges();//save after adding all the test questions

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
    }
}
