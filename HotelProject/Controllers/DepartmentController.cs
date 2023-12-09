using HotelProject.DAL;
using HotelProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static HotelProject.Helpers.Helper;

namespace HotelProject.Controllers
{
   
    public class DepartmentController : Controller
    {
        private readonly AppDbContext _db;
        public DepartmentController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index(int page=1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.PageCount = Math.Ceiling((decimal)_db.Departments.Count() / 4);
            List<Department> department = await _db.Departments.OrderByDescending(x=>x.IsDeactive==false).Skip((page-1)*4).Take(4).ToListAsync();
            return View(department);
        }
        public IActionResult Create()
        #region Create Department 
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
        {

            if (!ModelState.IsValid)
            {
              
                return View();
            }
            bool IsExist = await _db.Departments.AnyAsync(x => x.Name == department.Name);
            if (IsExist)
            {
                ModelState.AddModelError("Name", "Bu departament artıq mövcuddur");
                return View();
            }
            await _db.Departments.AddAsync(department);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Department");
        }
        #endregion
        public async Task<IActionResult> Update(int? id)
        #region Update Departments
        {
            Department dbDepartment = await _db.Departments.FirstOrDefaultAsync(x => x.Id == id);
            if (id == null)
            {
                return NotFound();
            }
            if (dbDepartment == null)
            {
                return BadRequest();
            }
            return View(dbDepartment);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Department department)
        {
            Department dbDepartment = await _db.Departments.FirstOrDefaultAsync(x => x.Id == id);
            if (id == null)
            {
                return NotFound();
            }
            if (dbDepartment == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return View(dbDepartment);
            }
            bool isExist = await _db.Departments.AnyAsync(x => x.Name == department.Name && x.Id != id);
            if (isExist)
            {
                ModelState.AddModelError("Name", "Bu adda başqa departament mövcuddur");
                return View(dbDepartment);
            }
            dbDepartment.Name = department.Name;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Department");
        }
        #endregion
        public async Task<IActionResult> Activity(int? id)
        #region  Department Activity
        {
            Department dbDepartment = await _db.Departments.FirstOrDefaultAsync(x => x.Id == id);
            if (id == null)
            {
                return NotFound();
            }
            if (dbDepartment == null)
            {
                return BadRequest();
            }
            if (dbDepartment.IsDeactive)
            {
                dbDepartment.IsDeactive = false;
            }
            else
            {
                dbDepartment.IsDeactive = true;
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Department");


        }
        #endregion
    }
}
