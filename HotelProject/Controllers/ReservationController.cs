using HotelProject.DAL;
using HotelProject.Helpers;
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
  
    public class ReservationController : Controller
    {
        private readonly AppDbContext _db;
        public ReservationController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index(int page=1)
        {

            ViewBag.CurrentPage = page;
            ViewBag.PageCount = Math.Ceiling((decimal)_db.Reservation.Count() / 4);
            List<Reservation> reservations = await _db.Reservation.Skip((page-1)*4).Take(4).
            Where(x => x.IsDeactive == false).Include(x => x.Rooms).Include(x => x.Rooms.RoomType).ToListAsync();
           

            return View(reservations);
        }
        
        public async Task<IActionResult> Create()
        {
            #region Create Reservations 
            ViewBag.RoomType = await _db.RoomType.ToListAsync();
            RoomType roomType = await _db.RoomType.FirstOrDefaultAsync();

            ViewBag.Rooms = await _db.Rooms.Include(x => x.RoomType).Where(x => x.RoomTypeId == roomType.Id && x.IsDeactive == false).ToListAsync();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reservation reservation, int rnumb)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            ViewBag.RoomType = await _db.RoomType.ToListAsync();
            RoomType roomType = await _db.RoomType.FirstOrDefaultAsync();
            ViewBag.Rooms = await _db.Rooms.Include(x => x.RoomType).Where(x => x.RoomTypeId == roomType.Id && x.IsDeactive == false).ToListAsync();

            reservation.RoomsId = rnumb;
            //bool IsReserve = await _db.Reservation.AnyAsync(x => x.EntryDate>reservation.EntryDate&&x.EntryDate>reservation.DepartDate && x.RoomsId == reservation.RoomsId && x.IsDeactive == false
            //|| x.DepartDate<reservation.EntryDate&&x.RoomsId==reservation.RoomsId&&x.IsDeactive==false);
            //if (!IsReserve)
            //{
            //    ModelState.AddModelError("", "Otaqlari digər rezerv vaxtlarından ən az bir gün əvvəl/sonra rezerv etmək olur");
            //    return View();
            //}
            //bool isExist = await _db.Reservation.AnyAsync(x => x.RoomsId == reservation.RoomsId && x.IsDeactive == false);
            //if (isExist)
            //{
            //    ModelState.AddModelError("Rooms.RoomNumber", "Bu nömrəli otaq rezerv olunub");
            //    return View();
            //}
            if (reservation.FullName == null)
            {
                ModelState.AddModelError("FullName", "Zəhmət olmasa rezerv edən şəxsin adını daxil edin");
                return View();
            }
            await _db.AddAsync(reservation);
            await MailSend.SendMailAsync("LizardHotel ", "Rezervasiyaniz ugurla bitdi", reservation.Email);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GetRoomsByType(int typeId)
        {
            List<Rooms> rooms = await _db.Rooms.Include(x => x.RoomType).Where(x => x.RoomTypeId == typeId && x.IsDeactive == false).ToListAsync();
            return PartialView("_RoomNumbersPartial", rooms);
        }

        #endregion

        public async Task<IActionResult> Delete(int? id)
        {
            Reservation dbReservations = await _db.Reservation.FirstOrDefaultAsync(x => x.Id == id);
            #region Delete Reservations
            if (id == null)
            {
                return NotFound();
            }
            if (dbReservations == null)
            {
                return BadRequest();
            }
            return View(dbReservations);
        }
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            Reservation dbReservations = await _db.Reservation.FirstOrDefaultAsync(x => x.Id == id);

            if (id == null)
            {
                return NotFound();
            }
            if (dbReservations == null)
            {
                return BadRequest();
            }
            dbReservations.IsDeactive = true;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        public async Task<IActionResult> Update(int? id)
        {
            #region Update Reservations 
            Reservation dbReservation = await _db.Reservation.Include(x => x.Rooms).ThenInclude(x => x.RoomType).FirstOrDefaultAsync(x => x.Id == id);
            if (id == null)
            {
                return NotFound();
            }
            if (dbReservation == null)
            {
                return BadRequest();
            }

            ViewBag.RoomType = await _db.RoomType.OrderByDescending(x => x.Id == dbReservation.Rooms.RoomTypeId).ToListAsync();
            RoomType roomType = await _db.RoomType.FirstOrDefaultAsync();

            ViewBag.Rooms = await _db.Rooms.Include(x => x.RoomType).Where(x => x.RoomTypeId == dbReservation.Rooms.RoomTypeId && x.IsDeactive == false).OrderByDescending(x => x.Id == dbReservation.Rooms.Id).ToListAsync();
            return View(dbReservation);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Reservation reservation, int rnumb)
        {
            Reservation dbReservation = await _db.Reservation.Include(x => x.Rooms).ThenInclude(x => x.RoomType).FirstOrDefaultAsync(x => x.Id == id);
            if (id == null)
            {
                return NotFound();
            }
            if (dbReservation == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return View(dbReservation);
            }
            ViewBag.RoomType = await _db.RoomType.OrderByDescending(x => x.Id == dbReservation.Rooms.RoomTypeId).ToListAsync();
            RoomType roomType = await _db.RoomType.FirstOrDefaultAsync();

            ViewBag.Rooms = await _db.Rooms.Include(x => x.RoomType).Where(x => x.RoomTypeId == dbReservation.Rooms.RoomTypeId && x.IsDeactive == false ).OrderByDescending(x => x.Id == dbReservation.Rooms.Id).ToListAsync();
            dbReservation.RoomsId = rnumb;

            //bool isExist = await _db.Reservation.AnyAsync(x => x.RoomsId == dbReservation.RoomsId && x.Id != id && x.IsDeactive == false);
            //if (isExist)
            //{

            //    ModelState.AddModelError("Rooms.RoomNumber", "Bu Nömrəli otaq rezerv olunub ");
            //    return View(dbReservation);
            //}

            dbReservation.FullName = reservation.FullName;
            dbReservation.Quantity = reservation.Quantity;
            dbReservation.Email = reservation.Email;
            dbReservation.EntryDate = reservation.EntryDate;
            dbReservation.DepartDate = reservation.DepartDate;
            dbReservation.Executor = reservation.Executor;


            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
            #endregion
        }
        public async Task<IActionResult> Detail(int? id)
        {
            #region Detail Reservations
            Reservation dbReservation = await _db.Reservation.Include(x => x.Rooms).ThenInclude(x => x.RoomType).FirstOrDefaultAsync(x => x.Id == id);
            if (id == null)
            {
                return NotFound();
            }
            if (dbReservation == null)
            {
                return BadRequest();
            }
            return View(dbReservation);
            #endregion
        }

    }
}
