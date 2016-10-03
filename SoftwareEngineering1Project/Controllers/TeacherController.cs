///////////////////////////////////////////////////////////////////////////////////////////////////
//
//	File Name:         TeacherController.cs
//	Description:       This class contains action methods associated with the TeacherController. 
//
//	Author:            Dana Light, lightdj@etsu.edu
//
///////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SoftwareEngineering1Project.DataContexts;
using SoftwareEngineering1Project.Helpers;
using SoftwareEngineering1Project.Models;
using System.Data.Entity;

namespace SoftwareEngineering1Project.Controllers
{
    [Authorize(Roles = "professor")]
    public class TeacherController : Controller
    {
        /// <summary>
        /// The _application database context
        /// </summary>
        private ApplicationDb _applicationDB = new ApplicationDb();

        /// <summary>
        /// Displays a jQuery data table containing a list of teachers in the system
        /// </summary>
        /// <returns>ViewResult</returns>
        public ActionResult Index()
        {
            //fetch all the teachers from the database
            var teachers = _applicationDB.Teachers.ToList();
            //create a list of teachers formatted for the data table model
            var teachersList = new List<object>();
            //build the list of teachers
            foreach (var teacher in teachers)
            {
                teachersList.Add(
                    new
                    {
                        id = teacher.ID,
                        firstName = teacher.FirstName,
                        lastName = teacher.LastName,
                        eNumber = teacher.ENumber,
                        title = teacher.Title.ToString() + ".",
                        status = teacher.Status,
                    }
                );
            }
            //create the data table for the index page
            //I used the method chaining that I build into the DataTableModel
            DataTableModel teachersDataTableModel = new DataTableModel();
            teachersDataTableModel.
                setTitle("Teachers").//set the title
                setData(teachersList).//pass in the teacher list
                setSearchSort(true).//initializes the jQuery data table library
                setHeaders(new List<object>()//add the headers and map them to the teachersList data
                {
                    new
                    {
                        Name = "Title",
                        Field = "title"
                    },
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
                        Name = "ENumber",
                        Field = "eNumber"
                    },
                    new
                    {
                        Name = "Status",
                        Field = "status"
                    }
                }).
                setActions(new List<object>()//define any links to modify teachers
                {
                    new
                    {
                        text = "Update",
                        url = "/teacher/update/{{id}}"
                    },
                    new
                    {
                        text = "Delete",
                        url = "/teacher/delete/{{id}}"
                    }
                }).
                setTableButtons(new List<object>()//add a link to create a teacher
                {
                    new
                    {
                        text = "Create Teacher",
                        url = "/teacher/create"
                    }
                });
            return View(teachersDataTableModel.Render());//pass the HtmlString from the render function to the view
        }

        /// <summary>
        /// Displays a form to the user to allow the creation of a teacher.
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }
        /// <summary>
        /// Creates the specified teacher.
        /// </summary>
        /// <param name="teacher">The teacher built from the post request.</param>
        /// <returns>Mixed</returns>
        [HttpPost]
        public ActionResult Create(Teacher teacher)
        {
            //if the model state is valid then add the teach to the teachers list, save, and redirect
            if (ModelState.IsValid)
            {
                _applicationDB.Teachers.Add(teacher);//add teach to ORM list
                _applicationDB.SaveChanges();//perisist the changes
                //add flash message for successful creation
                TempData["Message"] = new { Message = "Successfully created teacher", Type = "success" };
                return RedirectToAction("Index", "Teacher");
            }
            //pass the teacher that was attempted back to the view to
            //populate the form on unsuccessful attempt
            return View(teacher);
        }
        /// <summary>
        /// Displays a populated form for updating a teacher.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ViewResult</returns>
        public ActionResult Update(int? id)
        {
            if (id == null)
                return Redirect("Index");
            
            var teacher = _applicationDB.Teachers.Find(id);//find the teacher in the database
            if (teacher == null)//if the teacher doesn't exist then throw a 404
            {
                return HttpNotFound();
            }
            return View(teacher);
        }
        /// <summary>
        /// Updates the specified teacher.
        /// </summary>
        /// <param name="teacher">The teacher.</param>
        /// <returns>Mixed</returns>
        [HttpPost]
        public ActionResult Update(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                _applicationDB.Entry(teacher).State = EntityState.Modified;//let the ORM know object has been modified
                _applicationDB.SaveChanges();//persist changes
                //add flash message for successful udpate
                TempData["Message"] = new { Message = "Successfully updated teacher", Type = "success" };
                return RedirectToAction("Index");
            }
            return View(teacher);
        }
        /// <summary>
        /// Displays a form and information about the teacher that will be deleted
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ViewResult</returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return Redirect("Index");

            var teacher = _applicationDB.Teachers.Find(id);//find the teacher in the database
            if (teacher == null)//if the teacher doesn't exist then throw a 404
            {
                return HttpNotFound();
            }

            //Creates the PanelTable and sets its title and the number of 
            //items displayed in a row (includes headers)
            PanelTable viewTable = new PanelTable();
            viewTable.Title = "Teacher Information";
            viewTable.ItemsPerRow = 4;

            //the key is the the label and the value is the specific teacher's information
            viewTable.Data = new Dictionary<string, string>()
            {
                { "Title", teacher.Title.ToString() + "." },
                {"First Name",  teacher.FirstName},
                {"Last Name" , teacher.LastName},
                {"ENumber", teacher.ENumber},
                {"Status", teacher.Status },
                { "", "" }
            };
            //add the table to the view bag
            ViewBag.Table = viewTable;

            return View(teacher);
        }
        /// <summary>
        /// Deletes the specified teacher.
        /// </summary>
        /// <param name="teacher">The teacher.</param>
        /// <returns>RedirectToActionResult</returns>
        [HttpPost, ActionName("Delete")]
        public ActionResult PersistDelete(int id)
        {
            _applicationDB.Teachers.Remove(_applicationDB.Teachers.Find(id));//remove the teach from the database
            _applicationDB.SaveChanges();//persist the changes
            //add flash message for successful deletion
            TempData["Message"] = new { Message = "Successfully deleted teacher", Type = "success" };
            return RedirectToAction("Index");
        }
    }
}