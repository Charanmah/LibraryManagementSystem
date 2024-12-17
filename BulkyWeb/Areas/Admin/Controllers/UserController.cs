using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.ViewModels;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Bulky.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;  // Initialize the UserManager
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(string userid)
        {
            // Get the current user's role ID
            string RoleID = _db.UserRoles.FirstOrDefault(u => u.UserId == userid)?.RoleId;

            if (RoleID == null) return NotFound("Role not found.");

            RoleManagementVM RoleVM = new RoleManagementVM()
            {
                applicationUser = _db.ApplicationUsers.Include(U => U.Company).FirstOrDefault(u => u.Id == userid),
                RoleList = _db.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = _db.Companies.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };

            RoleVM.applicationUser.Role = _db.Roles.FirstOrDefault(u => u.Id == RoleID)?.Name;

            return View(RoleVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
        {
            // Get the current role ID of the user
            string RoleID = _db.UserRoles.FirstOrDefault(u => u.UserId == roleManagementVM.applicationUser.Id)?.RoleId;
            string oldRole = _db.Roles.FirstOrDefault(u => u.Id == RoleID)?.Name;

            if (oldRole == null) return NotFound("Old role not found.");

            // If role has changed
            if (!(roleManagementVM.applicationUser.Role == oldRole))
            {
                ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == roleManagementVM.applicationUser.Id);
                if (roleManagementVM.applicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = roleManagementVM.applicationUser.CompanyId;
                }
                if (oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }

                _db.SaveChanges();

                // Remove old role and add new role
                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagementVM.applicationUser.Role).GetAwaiter().GetResult();

                return RedirectToAction("Index");
            }

            // If roles are the same, return to the Role Management page
            return RedirectToAction("RoleManagement", new { userid = roleManagementVM.applicationUser.Id });
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _db.ApplicationUsers.Include(u => u.Company).ToList();
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach (var user in objUserList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id)?.RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId)?.Name;
                if (user.Company == null)
                {
                    user.Company = new() { Name = "" };
                }
            }
            return Json(new { data = objUserList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                // Unlock the user
                objFromDb.LockoutEnd = DateTime.Now;
                _db.SaveChanges();
                return Json(new { success = true, message = "User unlocked successfully." });
            }
            else
            {
                // Lock the user
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
                _db.SaveChanges();
                return Json(new { success = true, message = "User locked successfully." });
            }
        }

        #endregion
    }
}