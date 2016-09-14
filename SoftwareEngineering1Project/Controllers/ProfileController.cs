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
    //[Authorize(Roles = "sysadmin")] 
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

        // GET: Profile
        public ActionResult Index()
        {
            List<object> allUserProfiles = new List<object>();  
                  
            var allProfiles = _profileDb.Profiles.ToList();

            var roles = _userDb.Roles.ToList();
            string tempRoleHolder = "";
            foreach (var profile in allProfiles)
            {                
                foreach (var role in roles)
                {
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
            profileTable.SearchSort = true;
            profileTable.TableButtons = new List<object>()
            {
                new
                {
                    text = "Create User",
                    url = "/profile/add"
                }
            };

            return View(profileTable.Render());
        }

        public ActionResult Add()
        {
            UserProfile newUser = new UserProfile();
            return View(newUser);        
        }

        [HttpPost]
        public ActionResult Add(UserProfile newUser)
        {
            if (ModelState.IsValid)
            {
                if (!_profileDb.Profiles.Any(p => p.UserEmail == newUser.Email))
                {

                    Profile profile = new Profile();
                    profile.FirstName = newUser.FirstName;
                    profile.LastName = newUser.LastName;
                    profile.UserEmail = newUser.Email;

                    RoleStore<IdentityRole> roleStore = new RoleStore<IdentityRole>(_userDb);
                    RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(roleStore);
                    UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(_userDb);
                    UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(userStore);

                    ApplicationUser user = new ApplicationUser { UserName = newUser.Email, Email = newUser.Email, EmailConfirmed = true };
                    userManager.Create(user, newUser.Password);
                    userManager.AddToRole(user.Id, "professor");

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

        public ActionResult View(int? id)
        {
            if(id == null)
            {
                return Redirect("Index");
            }

            Profile profileInfo = _profileDb.Profiles.Find(id);
            if(profileInfo == null)
            {
                return HttpNotFound();
            }

            ApplicationUser user = _userDb.Users.Single(u => u.Email == profileInfo.UserEmail);
            string username = user.UserName.ToString();
            UserProfile up = new UserProfile(profileInfo);

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

            PanelTable viewTable = new PanelTable();
            viewTable.Title = "User Information";
            viewTable.ItemsPerRow = 4;
            viewTable.Data = new Dictionary<string, string>()
            {
                {"First Name",  up.FirstName},
                {"Last Name" , up.LastName},
                {"Email/Username", up.Email},
                {"Role", up.Role }
            };
            viewTable.TableButtons = new Dictionary<string, string>()
            {
                {"/profile/edit/" + profileInfo.Id, "Edit" },
                {"/profile/delete/" + profileInfo.Id, "Delete" }
            };

            return View(viewTable.Render());

        }

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

            return View(profileDelete);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Profile profileDelete = _profileDb.Profiles.Find(id);
            _profileDb.Profiles.Remove(profileDelete);
            _profileDb.SaveChanges();

            UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(_userDb);
            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(userStore);

            ApplicationUser user = _userDb.Users.Single(u => u.Email == profileDelete.UserEmail);
            
            userManager.Delete(user);
            _userDb.SaveChanges();

            return RedirectToAction("Index");
        }

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

            ViewBag.UserFName = profilePassword.FirstName;
            ViewBag.UserLName = profilePassword.LastName;

            ResetPasswordViewModel reset = new ResetPasswordViewModel();
            reset.Email = user.Email;

            return View(reset);
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel reset)
        {
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

            userManager.ResetPassword(user.Id, resetToken, reset.Password);
            _userDb.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}