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
   
    public class PositionsController : Controller
    {
        private readonly AppDbContext _db;
        public PositionsController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index(int page=1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.PageCount = Math.Ceiling((decimal)_db.Positions.Count() / 4);
            List<Positions> position = await _db.Positions.OrderByDescending(x => x.IsDeactive == false).Skip((page-1)*4).Take(4).Include(x=>x.Department).ToListAsync();

            return View(position);
        }
        
        public async Task<IActionResult> Create(int depid)
        #region Create Positions
        {
            ViewBag.Department = await _db.Departments.Where(x=>x.IsDeactive==false).ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Positions position, int depid)
        {
            ViewBag.Department = await _db.Departments.Where(x => x.IsDeactive == false).ToListAsync();
            position.DepartmentId = depid;
            if (!ModelState.IsValid)
            {
               
                return View();
            }
            bool IsExist = await _db.Positions.AnyAsync(x => x.Name == position.Name);
            if (IsExist)
            {
                ModelState.AddModelError("Name", "Belə vəzifə arıtq mövcuddur");
                return View();
            }
            await _db.AddAsync(position);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
#endregion

        public async Task<IActionResult> Update(int? id,int depid)
        #region Update Positions
        {
            ViewBag.Department = await _db.Departments.Where(x=>x.IsDeactive==false).ToListAsync();
            Positions dbPositions = await _db.Positions.FirstOrDefaultAsync(x => x.Id == id);
            return View(dbPositions);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, int depid,Positions positions)
        {
            Positions dbPositions = await _db.Positions.FirstOrDefaultAsync(x => x.Id == id);
            ViewBag.Department = await _db.Departments.Where(x => x.IsDeactive == false).ToListAsync();
            dbPositions.DepartmentId = depid;
            if (!ModelState.IsValid)
            {
               
                return View(dbPositions);
            }
            if (id == null)
            {
                return NotFound();
            }
            if (dbPositions == null)
            {
                return BadRequest();
            }
            bool IsExist = await _db.Positions.AnyAsync(x => x.Name == positions.Name && x.Id != id);
            if (IsExist)
            {
                ModelState.AddModelError("Name", "Bu adda vızifə artq mövcuddur");
                return View(dbPositions);
                   
            }
            dbPositions.Name = positions.Name;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion
        public async Task<IActionResult> Activity(int? id)
        #region Position Activity
        {
            Positions dbPositions = await _db.Positions.FirstOrDefaultAsync(x => x.Id == id);
            Department dbDepartment = await _db.Departments.FirstOrDefaultAsync(x => x.Id == dbPositions.DepartmentId);
            if (id == null)
            {
                return NotFound();
            }
            if (dbPositions == null)
            {
                return BadRequest();
            }
            if (dbPositions.IsDeactive)
            {
                dbPositions.IsDeactive = false;
            }
            else
            {
                dbPositions.IsDeactive = true;
            }
         
            
            
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion
    }
}
