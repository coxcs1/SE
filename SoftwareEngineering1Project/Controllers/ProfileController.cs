///////////////////////////////////////////////////////////////////////////////////////////////////
//
//	File Name:         ProfileController.cs
//	Description:       This class contains action methods associated with the ProfileController. 
//
//	Author:            Mackenzie Eagan, eaganm@etsu.edu
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

namespace SoftwareEngineering1Project.Controllers
{
    [Authorize(Roles = "sysadmin")] 
    public class ProfileController : Controller
    {        
        private ApplicationDb _profileDb = new ApplicationDb();
        private IdentityDb _userDb = new IdentityDb();

        protected override void Dispose(bool disposing)
        {
            _profileDb.Dispose();
            _userDb.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Index Action - displays a list of all users using a DataTable view helper
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //DataTable is built using lists of generic objects
            List<object> allUserProfiles = new List<object>();

            int sysadminID = 0;  
                  
            var allProfiles = _profileDb.Profiles.ToList();

            var roles = _userDb.Roles.ToList();
            string tempRoleHolder = "";
            foreach (var profile in allProfiles)
            {                
                foreach (var role in roles)
                {
                    //grabs the user and gets their role
                    var user = _userDb.Users.Single(u => u.Email == profile.UserEmail);
                    foreach (var r in user.Roles)
                    {
                        if (role.Id == r.RoleId)
                        {
                            tempRoleHolder = role.Name;
                            tempRoleHolder = tempRoleHolder.First().ToString().ToUpper() + String.Join("", role.Name.Skip(1));
                            break;
                        }
                    }
                }

                if(tempRoleHolder == "Sysadmin")
                {
                    ViewBag.SysadminId = profile.Id;
                    sysadminID = profile.Id;
                }
                
                //adds new object to list - setup like key-value pairs
                allUserProfiles.Add(
                    new
                    {
                        id = profile.Id,
                        firstName = profile.FirstName,
                        lastName = profile.LastName,
                        email = profile.UserEmail,
                        role = tempRoleHolder
                    }
                );
            }

            DataTableModel profileTable = new DataTableModel();
            profileTable.Title = "Users";

            //sets the table headers, the Field attribute must match the key value from the allUserProfiles list
            profileTable.Headers = new List<object>()
            {
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
                    Name = "Email",
                    Field = "email"
                },
                new
                {
                    Name = "Role",
                    Field = "role"
                } 
            };
            profileTable.Data = allUserProfiles;

            //sets the actions that will display for every entry in the table
            //uses a str replace to replace the {{id}} values with the actual id
            //id matches to the key seen earlier
            profileTable.Actions = new List<object>()
            {
                new
                {
                    text = "View",
                    url = "/profile/view/{{id}}"
                },
                new
                {
                    text = "Edit",
                    url = "/profile/edit/{{id}}"               
                },
                new
                {
                    text = "Delete",
                    url = "/profile/delete/{{id}}"
                },
                new
                {
                    text = "Reset Password",
                    url = "/profile/resetpassword/{{id}}"
                }
            };
            //allows the table to be searched and sorted
            profileTable.SearchSort = true;

            string originalTable = profileTable.Render().ToString();
            Response.Write(originalTable);
            
            originalTable = originalTable.Replace("a href='/profile/resetpassword/" + sysadminID.ToString() + " class='btn btn-primary btn-xs'>", "this");
            Response.Write(originalTable);
            Response.End();
            //< a href = "/profile/resetpassword/7" class="btn btn-primary btn-xs">Reset Password</a>
            //+ " class='btn btn-primary btn-xs'>"
            //render function returns an HtmlString to the view
            return View(profileTable.Render());
        }

        /// <summary>
        /// Get Add action
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            //uses a ViewModel to be able to store all needed information
            UserProfile newUser = new UserProfile();
            return View(newUser);        
        }

        /// <summary>
        /// Post Add Action
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(UserProfile newUser)
        {
            //checks to make sure everything was filled out appropriately in accordance
            //to the tags in the ViewModel
            if (ModelState.IsValid)
            {
                //makes sure a user with this email has not already been added
                if (!_profileDb.Profiles.Any(p => p.UserEmail == newUser.Email))
                {
                    //creates the profile from the information entered
                    Profile profile = new Profile();
                    profile.FirstName = newUser.FirstName;
                    profile.LastName = newUser.LastName;
                    profile.UserEmail = newUser.Email;
                    
                    //uses the userManager to create a user with the email entered and adds them to the professor role
                    UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(_userDb);
                    UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(userStore);
                    ApplicationUser user = new ApplicationUser { UserName = newUser.Email, Email = newUser.Email, EmailConfirmed = true };
                    userManager.Create(user, newUser.Password);
                    userManager.AddToRole(user.Id, "professor");

                    //saves both the profile and the user to the database                    
                    _profileDb.Profiles.Add(profile);
                    _profileDb.SaveChanges();
                    _userDb.SaveChanges();
                    return RedirectToAction("Index", "Profile");
                }
                else
                {
                    ModelState.AddModelError("Email", "This email is currently being used. Please select another one.");
                }
            }

            return View(newUser);
        }

        /// <summary>
        /// The View Action - displays the user information using a PanelTable ViewHelper
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult View(int? id)
        {
            if (id == null)
            {
                return Redirect("Index");
            }

            Profile profileInfo = _profileDb.Profiles.Find(id);
            if(profileInfo == null)
            {
                return HttpNotFound();
            }

            //finds the application user and sets the UserProfile view model using the Profile
            ApplicationUser user = _userDb.Users.Single(u => u.Email == profileInfo.UserEmail);
            string username = user.UserName.ToString();
            UserProfile up = new UserProfile(profileInfo);

            //finds the role the user has
            var roles = _userDb.Roles.ToList();
            foreach(var role in roles )
            {
                foreach (var r in user.Roles)
                {
                    if(role.Id == r.RoleId)
                    {
                        up.Role = role.Name;
                        up.Role = up.Role.First().ToString().ToUpper() + String.Join("", role.Name.Skip(1));
                        break;
                    }
                }
            }

            //Creates the PanelTable and sets its title and the number of 
            //items displayed in a row (includes headers)
            PanelTable viewTable = new PanelTable();
            viewTable.Title = "User Information";
            viewTable.ItemsPerRow = 4;
            
            //the key is the the label and the value is the specific user's information
            viewTable.Data = new Dictionary<string, string>()
            {
                {"First Name",  up.FirstName},
                {"Last Name" , up.LastName},
                {"Email/Username", up.Email},
                {"Role", up.Role }
            };
            //key is the url link and the value is what is displayed to the user
            viewTable.TableButtons = new Dictionary<string, string>()
            {
                {"/profile/edit/" + profileInfo.Id, "Edit" },               
            };
            if(up.Role != "Sysadmin")
            {
                viewTable.TableButtons.Add("/profile/delete/" + profileInfo.Id, "Delete");
                viewTable.TableButtons.Add("/profile/resetpassword/" + profileInfo.Id, "Reset Password");
            }

            //render function returns an HtmlString to the view
            return View(viewTable.Render());
        }

        /// <summary>
        /// Get Edit Action - makes sure there is a profile to edit before returning
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

            Profile profileEdit = _profileDb.Profiles.Find(id);
            if (profileEdit == null)
            {
                return HttpNotFound();
            }

            return View(profileEdit);
        }

        /// <summary>
        /// Post Edit Action - marks the profile as having changed and then
        /// saves the changes
        /// </summary>
        /// <param name="profileEdit"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Profile profileEdit)
        {
            if (ModelState.IsValid)
            {
                _profileDb.Entry(profileEdit).State = EntityState.Modified;
                _profileDb.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(profileEdit);
        }

        /// <summary>
        /// Get Delete Action - makes sure there is a profile to delete before returning
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

            Profile profileDelete = _profileDb.Profiles.Find(id);
            if (profileDelete == null)
            {
                return HttpNotFound();
            }

            //gets the role of the user so it can be passed to the view in the viewbag
            ApplicationUser user = _userDb.Users.Single(u => u.Email == profileDelete.UserEmail);

            var roles = _userDb.Roles.ToList();
            foreach (var role in roles)
            {
                foreach (var r in user.Roles)
                {
                    if (role.Id == r.RoleId)
                    {
                        ViewBag.RoleName = role.Name.First().ToString().ToUpper() + String.Join("", role.Name.Skip(1));
                        break;
                    }
                }
            }

            if(ViewBag.RoleName == "Sysadmin")
            {
                return RedirectToAction("Index");
            }

            return View(profileDelete);
        }

        /// <summary>
        /// Post Delete Action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            //finds the profile and removes it from the database
            Profile profileDelete = _profileDb.Profiles.Find(id);
            _profileDb.Profiles.Remove(profileDelete);
            _profileDb.SaveChanges();

            UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(_userDb);
            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(userStore);

            //finds the associated user in the User table and removes it from the database
            ApplicationUser user = _userDb.Users.Single(u => u.Email == profileDelete.UserEmail);            
            userManager.Delete(user);
            _userDb.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Get ResetPasswordAction - makes sure a profile exists before returning to the view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ResetPassword(int? id)
        {
            if (id == null)
            {
                return Redirect("Index");
            }

            Profile profilePassword = _profileDb.Profiles.Find(id);
            if (profilePassword == null)
            {
                return HttpNotFound();
            }

            ApplicationUser user = _userDb.Users.Single(u => u.Email == profilePassword.UserEmail);

            //passes the user's first and last name to the viewbag to be displayed in the view
            ViewBag.UserFName = profilePassword.FirstName;
            ViewBag.UserLName = profilePassword.LastName;

            //Uses a ResetPasswordViewModel provided by ASP.NET
            ResetPasswordViewModel reset = new ResetPasswordViewModel();
            reset.Email = user.Email;

            return View(reset);
        }

        /// <summary>
        /// Post - ResetPassword Action
        /// </summary>
        /// <param name="reset"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel reset)
        {
            //makes sure the model state is valid before proceeding
            if (!ModelState.IsValid)
            {
                return View(reset);
            }

            ApplicationUser user = _userDb.Users.Single(u => u.Email == reset.Email);

            UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(_userDb);
            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(userStore);

            var provider = new DpapiDataProtectionProvider("Sample");

            userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
                provider.Create("ResetPassword"));


            var resetToken = userManager.GeneratePasswordResetToken(user.Id);

            //resets the user's password to the specified password
            userManager.ResetPassword(user.Id, resetToken, reset.Password);
            _userDb.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}