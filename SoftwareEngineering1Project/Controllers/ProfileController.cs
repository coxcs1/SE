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
            List<UserProfile> allUserProfiles = new List<UserProfile>();        
            var allProfiles = _profileDb.Profiles.ToList();

            //Roles.GetUsersInRole("roleName").Select(Membership.GetUser).ToList();

            DataTableModel profileTable = new DataTableModel();
            profileTable.Title = "Users";
            profileTable.Headers = new List<object>()
            {
                "Email/Username",
                "First Name",
                "Last Name",
                "Role"
            };
            List<object> data = new List<object>();

            for (int i = 0; i < allProfiles.Count; i++)
            {

            }


            return View();
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
                    return RedirectToAction("Index", "Home");
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
    }
}