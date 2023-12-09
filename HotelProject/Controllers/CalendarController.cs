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
    public class CalendarController : Controller
    {
        private readonly AppDbContext _db;
        public CalendarController(AppDbContext db)
        {
            _db=db;
        }
        public async Task<IActionResult> Index( int? id)
        {
            Rooms room = await _db.Rooms.FirstOrDefaultAsync(x => x.Id == id);
            List<Reservation> reservations = await _db.Reservation.Where(x=>x.IsDeactive==false).Where(x=>x.RoomsId==room.Id).ToListAsync();
            ViewBag.Guests = await _db.Guests.Include(x=>x.Rooms).FirstOrDefaultAsync(x =>x.RoomsId == id);
           
            return View(reservations);
        }
    }
}
