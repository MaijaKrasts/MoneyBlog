﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MoneyBlog.DataLayer;
using MoneyBlog.DataLayer.Constants;
using MoneyBlog.DataLayer.Models;
using MoneyBlog.Services.IService;
using MoneyBlog.Services.Service;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace MoneyBlog.Web.Controllers
{
    [Authorize(Roles = AdminConstants.AdminRole)]
    public class AdminController : Controller
    {
        private IAdminService _iAdminService;
        readonly DefaultConnection db = new DefaultConnection();


        public AdminController(IAdminService iAdminService /*ApplicationUserManager applicationUserManager*/)
        {
            _iAdminService = iAdminService;
            //_userManager = applicationUserManager;

        }

        ApplicationDbContext context = new ApplicationDbContext();

        //public ActionResult Index()
        //{
        //    return View(db.AspNetUsers.ToList());
        //}
        public ActionResult UserDetails(string id /*UsersInRolesModel model*/)
        {
            var user = _iAdminService.GetUser(id);

            return View(user);
        }
    
        public ActionResult EditUser(string id)
        {
           var user = _iAdminService.GetUser(id);
            return View(user);
        }
        
        [HttpPost]
        public ActionResult EditUser( UsersInRolesModel model /*AspNetUser aspNetUser*/)
        {
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("UsersWithRoles");
        }
        public ActionResult CreateUserRole()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateUserRole(FormCollection form)
        {
            string roleName = form["RoleName"];
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            if (!roleManager.RoleExists(roleName))
            {
                var role = new IdentityRole(roleName);
                roleManager.Create(role);
            }
            else
            {
                ModelState.AddModelError("error", "role already exists");
            }
            return View("Index");
        }
        public ActionResult AssignUserRole()
        {
            ViewBag.Roles = context.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
            return View();
        }
        [HttpPost]
        public ActionResult AssignUserRole(FormCollection form)
        {
            string username = form["txtUserName"];
            string rolename = form[FormConstants.formRoleName];
            ApplicationUser user = context.Users.Where(u => u.UserName.Equals(username, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            userManager.AddToRole(user.Id, rolename);

            return View("Index");
        }
        public ActionResult UsersWithRoles()
        {

            var usersWithRoles = (from user in context.Users
                                  select new
                                  {
                                      UserId = user.Id,
                                      Username = user.UserName,
                                      Email = user.Email,
                                      RoleNames = (from userRole in user.Roles
                                                   join role in context.Roles on userRole.RoleId
                                                   equals role.Id
                                                   select role.Name).ToList()
                                  }).ToList().Select(p => new UsersInRolesModel()

                                  {
                                      UserId = p.UserId,
                                      Email = p.Email,
                                      Role = string.Join(",", p.RoleNames)
                                  });


            return View(usersWithRoles);
        }
        [HttpPost]
        public ActionResult UsersWithRoles(string searchText)
        {
            var usersWithRoles = (from user in context.Users
                                  select new
                                  {
                                      UserId = user.Id,
                                      Username = user.UserName,
                                      Email = user.Email,
                                      RoleNames = (from userRole in user.Roles
                                                   join role in context.Roles on userRole.RoleId
                                                   equals role.Id
                                                   select role.Name).ToList()
                                  }).ToList().Select(p => new UsersInRolesModel()

                                  {
                                      UserId = p.UserId,
                                      Email = p.Email,
                                      Role = string.Join(",", p.RoleNames)
                                  });
            if (searchText != null)
            {
                var usersWithRole = (from user in context.Users
                                 select new
                                 {
                                     UserId = user.Id,
                                     Email = user.Email,
                                     RoleNames = (from userRole in user.Roles
                                                  join role in context.Roles on userRole.RoleId
                                                  equals role.Id
                                                  select role.Name).ToList()
                                 }).Where(x => x.Email.Contains(searchText)).ToList();
            };
            return View(usersWithRoles);

        }

    }
}