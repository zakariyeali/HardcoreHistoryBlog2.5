﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HardcoreHistoryBlog.Core;
using HardcoreHistoryBlog.Data;
using HardcoreHistoryBlog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HardcoreHistoryBlog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private IRepository _repo;
        private readonly ApplicationDbContext _context;


        public RolesController(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager, IRepository repo, ApplicationDbContext context)

        {
            _userManager = userManager;
            _roleManager = roleManager;
            _repo = repo;
            _context = context;

        }
        public IActionResult Index()
        {
            List<RoleListViewModel> model = new List<RoleListViewModel>();
            model = _roleManager.Roles.Select(r => new RoleListViewModel
            {
                RoleName = r.Name,
                Description = r.Description,
                Id = r.Id,
                NumberOfUsers = r.UserRoles.Count
            }).ToList();
            return View(model);
        }

        [AutoValidateAntiforgeryToken]

        public ActionResult Details(string id)
        {
            var role = _repo.GetRole((string)id);
            if (role == null)
            {
                return RedirectToAction("Index");
            }
            return View(role);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);
            {
                var role = new ApplicationRole
                { Name = vm.Name };
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    _repo.AddRole(role);
                    return RedirectToAction("Index");
                }
                else
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult Delete(string Id)
        {
            var role = _context.Roles.Find(Id);
            if (role == null)
            {
                return RedirectToAction("Index");
            }
            return View(role);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Delete([Bind(include: "Id,Name")]ApplicationRole myRole)
        {
            ApplicationRole role = _context.Roles.Find(myRole.Id);
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Edit(string Id)
        {
            var role = _repo.GetRole((string)Id);
            if (role == null)
            {
                return RedirectToAction("Index");
            }
            return View(new RoleViewModel { Id = role.Id, Name = role.Name, Description = role.Description });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleViewModel vm)
        {
            var role = await _roleManager.FindByIdAsync(vm.Id);
            if (vm.Name != role.Name)
            {
                role.Name = vm.Name;
            }
            if(vm.Description != role.Description)
            {
                role.Description = vm.Description;
            }
            var result = _roleManager.UpdateAsync(role).Result;

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Roles");
            }
            else return View(vm);
        }

        [HttpGet]
        public IActionResult UserRoles(string Id)
        {
            List<UserRolesViewModel> model = (from u in _context.Users
                                              join ur in _context.UserRoles on u.Id equals ur.UserId
                                              join r in _context.Roles on ur.RoleId equals r.Id
                                              orderby u.Email
                                              select new UserRolesViewModel
                                              {
                                                  Id = u.Id,
                                                  Email = u.Email,
                                                  Rolename = r.Name,
                                                  RoleId = ur.RoleId
                                              }).ToList();

            return View(model);
        }


        [HttpGet]
        public IActionResult Assign(string Id)
        {
            List<SelectListItem> ulist = new List<SelectListItem>();
            foreach(var user in _userManager.Users)
                ulist.Add(new SelectListItem() { Value = user.Email, Text = user.Email });
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var role in _roleManager.Roles)
                list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            ViewBag.Roles = list;
            ViewBag.Users = ulist;
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Assign(UserRolesViewModel vm)
        {
            var user = await _userManager.FindByEmailAsync(vm.Email);
            var result = _userManager.AddToRoleAsync(user, vm.Rolename).Result;
            if (result.Succeeded)
            {
                await _userManager.UpdateAsync(user);
                return RedirectToAction("UserRoles");
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult Remove(string Id) 
        {
            var user = _repo.GetUser((string)Id);
            return View(new UserRolesViewModel
            { Email = user.Email });
        }

        //[HttpPost]
        //public async Task Remove(List<string> deleteList, string Id) 
        //{
        //    var user = _repo.GetUser((string)Id);
        //    var role = _userManager.GetRolesAsync(user);
        //    roleName = role.ToAsyncEnumerable<roleNamse>            


        //    foreach (var roleName in deleteList)
        //    {
        //        var result = await _userManager.RemoveFromRoleAsync(user, roleName);                
        //    }
            
        //}
    }
}