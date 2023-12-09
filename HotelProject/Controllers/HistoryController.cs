using HotelProject.DAL;
using HotelProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelProject.Controllers
{
   
    public class HistoryController : Controller
    {
        private readonly AppDbContext _db;
        public HistoryController(AppDbContext db )
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Guests = await _db.Guests.Include(x=>x.Rooms).ThenInclude(x=>x.RoomType).Where(x=>x.IsDeactive==true).ToListAsync();
            List<Reservation> reservations = await _db.Reservation.Where(x => x.IsDeactive == true).Include(x => x.Rooms).Include(x => x.Rooms.RoomType).ToListAsync();
            return View(reservations);
        }
        public async Task<IActionResult> Delete()
        {
            #region Delete Reservations
            List<Reservation> dbReservations = await _db.Reservation.Where(x=>x.IsDeactive==true).ToListAsync();          
            return View(dbReservations);
        }
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost()
        {
            ViewBag.Guests = await _db.Guests.Include(x => x.Rooms).ThenInclude(x => x.RoomType).Where(x => x.IsDeactive == true).ToListAsync();
            List<Reservation> dbReservations = await _db.Reservation.Where(x => x.IsDeactive == true).ToListAsync();

            foreach (Reservation item in dbReservations)
            {
                _db.Remove(item);
            }
            foreach (Guests item in ViewBag.Guests)
            {
                _db.Remove(item);
            }
            
            await _db.SaveChangesAsync();
            return RedirectToAction("Index","History");
        }
        #endregion

    }
}
