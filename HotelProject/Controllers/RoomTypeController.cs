using HotelProject.DAL;
using HotelProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelProject.Controllers
{
    public class RoomTypeController : Controller
    {
        private readonly AppDbContext _db;
        public RoomTypeController(AppDbContext db)
        {
            _db = db;
        }
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomType roomType)
        {
            
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool isExist =await _db.RoomType.AnyAsync(x => x.Type == roomType.Type);
            if (isExist)
            {
                ModelState.AddModelError("Type", "Bu adda otaq mövcuddur");
                return View();
            }

            await _db.AddAsync(roomType);
            await _db.SaveChangesAsync();
            return RedirectToAction("Create","Rooms");
        }
    }
}
