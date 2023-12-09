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
    public class NotficationsController : Controller
    {
        private readonly AppDbContext _db;
        public NotficationsController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            #region Index 
            DateTime currentTime = DateTime.Now;

            List<Reservation> reservations = await _db.Reservation.Where(
                x => x.IsDeactive == false &&
                x.EntryDate <= currentTime
            ).ToListAsync();
            int count = reservations.Count;


            return View(reservations);
            #endregion
        }

        public async Task<IActionResult> GuestsIndex()
        {
            #region Guests Index
            DateTime currentTime = DateTime.Now;

            List<Guests> guests = await _db.Guests.Where(
                x => x.IsDeactive == false &&
                x.DepartDate <= currentTime
            ).ToListAsync();



            return View(guests);
            #endregion
        }

        public async Task<IActionResult> SendDataLayout()
        {
            #region Send Data to Layout With Json from controller for Reservations
            DateTime currentTime = DateTime.Now;

            List<Reservation> reservations = await _db.Reservation.Where(
                x => x.IsDeactive == false &&
                x.EntryDate <= currentTime
            ).ToListAsync();
            int count = reservations.Count;
            return Json(count);
            #endregion
        }
        public async Task<IActionResult> SendDataLayoutGuests()
        {
            #region Send Data to Layout With Json from controller For Guests
            DateTime currentTime = DateTime.Now;

            List<Guests> guests = await _db.Guests.Where(
                x => x.IsDeactive == false &&
                x.DepartDate <= currentTime
            ).ToListAsync();
            int countguests = guests.Count;
            return Json(countguests);
            #endregion
        }
        public async Task<IActionResult> SendDataLayoutSumResGuest()
        {
            #region Send Data to Layout With Json from controller For SumGuestsReservations
            DateTime currentTime = DateTime.Now;

            List<Reservation> reservations = await _db.Reservation.Where(
                x => x.IsDeactive == false &&
                x.EntryDate <= currentTime
            ).ToListAsync();
            List<Guests> guests = await _db.Guests.Where(
               x => x.IsDeactive == false &&
               x.DepartDate <= currentTime
           ).ToListAsync();
            int countguests = guests.Count;
            int count = reservations.Count;
            int sum = count + countguests;
            return Json(sum);
            #endregion
        }
        public async Task<IActionResult> Delete(int? id)
        {
            #region Delete Notfications with Reservations
            Reservation dbReservation = await _db.Reservation.FirstOrDefaultAsync(x => x.Id == id);
            if (id == null)
            {
                return NotFound();
            }
            if (dbReservation == null)
            {
                return BadRequest();
            }

            return View(dbReservation);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id)
        {
            Reservation dbReservation = await _db.Reservation.FirstOrDefaultAsync(x => x.Id == id);
            if (id == null)
            {
                return NotFound();
            }
            if (dbReservation == null)
            {
                return BadRequest();
            }
            dbReservation.IsDeactive = true;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
            #endregion
        }

        public async Task<IActionResult> Create(int? id)
        {
            #region Create Guests For in Notfications

            Reservation dbReservation = await _db.Reservation.Include(x => x.Rooms).ThenInclude(x => x.RoomType).FirstOrDefaultAsync(x => x.Id == id);
            if (id == null)
            {
                return NotFound();
            }
            if (dbReservation == null)
            {
                return BadRequest();
            }
            Guests guestsresrevr = new Guests
            {
                FullName = dbReservation.FullName,
                EntryDate = dbReservation.EntryDate,
                DepartDate = dbReservation.DepartDate,
                Quantity = dbReservation.Quantity,
                Email = dbReservation.Email,
                Executor=dbReservation.Executor


            };
            ViewBag.RoomType = await _db.RoomType.OrderByDescending(x => x.Id == dbReservation.Rooms.RoomTypeId).ToListAsync();
            RoomType roomType = await _db.RoomType.FirstOrDefaultAsync(x => x.Id == dbReservation.Rooms.RoomTypeId);

            ViewBag.Rooms = await _db.Rooms.Include(x => x.RoomType).Where(x => x.RoomType.Id == roomType.Id && x.IsDeactive == false).OrderByDescending(x => x.Id == dbReservation.RoomsId).ToListAsync();

            return View(guestsresrevr);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guests guests, int rnumb, int? id)
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
                return View();
            }
            Guests guestsresrevr = new Guests
            {
                FullName = dbReservation.FullName,
                EntryDate = dbReservation.EntryDate,
                DepartDate = dbReservation.DepartDate,
                Quantity = dbReservation.Quantity,
                RoomsId = dbReservation.RoomsId,
                Executor = dbReservation.Executor,
                Email=dbReservation.Email

            };
            guests.Id = guestsresrevr.Id;//Yaratdgi avtomatik id ni databaseye elave edecyimz gueste menimsetne
            ViewBag.RoomType = await _db.RoomType.OrderByDescending(x => x.Id == dbReservation.Rooms.RoomTypeId).ToListAsync();
            RoomType roomType = await _db.RoomType.FirstOrDefaultAsync(x => x.Id == dbReservation.Rooms.RoomTypeId);

            ViewBag.Rooms = await _db.Rooms.Include(x => x.RoomType).Where(x => x.RoomType.Id == roomType.Id && x.IsDeactive == false).OrderByDescending(x => x.Id == dbReservation.RoomsId).ToListAsync();

            guests.RoomsId = rnumb;
            bool isExist = await _db.Guests.AnyAsync(x => x.RoomsId == guests.RoomsId && x.IsDeactive == false);
            if (isExist)
            {
                ModelState.AddModelError("", "Bu nömrəli otaq doludur");
                return View();
            }
            await _db.AddAsync(guests);
            await Helpers.MailSend.SendMailAsync("Lizard", "Otele xos gelmisiniz", guests.Email);

            dbReservation.IsDeactive = true;
            guests.Rooms.IsBusy = true;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GetRoomsByType(int typeId)
        {
            List<Rooms> rooms = await _db.Rooms.Include(x => x.RoomType).Where(x => x.RoomTypeId == typeId && x.IsDeactive == false).ToListAsync();
            return PartialView("_RoomNumbersPartial", rooms);
        }

        #endregion
        public async Task<IActionResult> DeleteGuest(int? id)
        {
            #region Delete Guest from AdminPanel
            Guests dbGuests = await _db.Guests.FirstOrDefaultAsync(x => x.Id == id);
            if (id == null)
            {
                return NotFound();
            }
            if (dbGuests == null)
            {
                return BadRequest();
            }
            return View(dbGuests);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteGuest")]
        public async Task<IActionResult> DeleteGuestPost(int? id)
        {
            Guests dbGuests = await _db.Guests.Include(x=>x.Rooms).FirstOrDefaultAsync(x => x.Id == id);
            if (id == null)
            {
                return NotFound();
            }
            if (dbGuests == null)
            {
                return BadRequest();
            }

            dbGuests.IsDeactive = true;
            dbGuests.Rooms.IsBusy = false;
            await _db.SaveChangesAsync();
            return RedirectToAction("GuestsIndex", "Notfications");
            #endregion
        }
        public async Task<IActionResult> Update(int? id)
        {
            #region Update Guests
            Guests dbGuests = await _db.Guests.Include(x => x.Rooms).ThenInclude(x => x.RoomType).FirstOrDefaultAsync(x => x.Id == id);
            if (id == null)
            {
                return NotFound();
            }
            if (dbGuests == null)
            {
                return BadRequest();
            }

            ViewBag.RoomType = await _db.RoomType.OrderByDescending(x => x.Id == dbGuests.Rooms.RoomTypeId).ToListAsync();
            RoomType roomType = await _db.RoomType.FirstOrDefaultAsync();

            ViewBag.Rooms = await _db.Rooms.Include(x => x.RoomType).Where(x => x.RoomTypeId == dbGuests.Rooms.RoomTypeId && x.IsDeactive == false).OrderByDescending(x => x.Id == dbGuests.Rooms.Id).ToListAsync();
            return View(dbGuests);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Guests guests, int rnumb)
        {
            Guests dbGuests = await _db.Guests.Include(x => x.Rooms).ThenInclude(x => x.RoomType).FirstOrDefaultAsync(x => x.Id == id);
            if (id == null)
            {
                return NotFound();
            }
            if (dbGuests == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return View(dbGuests);
            }
            ViewBag.RoomType = await _db.RoomType.OrderByDescending(x => x.Id == dbGuests.Rooms.RoomTypeId).ToListAsync();
            RoomType roomType = await _db.RoomType.FirstOrDefaultAsync();

            ViewBag.Rooms = await _db.Rooms.Include(x => x.RoomType).Where(x => x.RoomTypeId == dbGuests.Rooms.RoomTypeId && x.IsDeactive == false).OrderByDescending(x => x.Id == dbGuests.Rooms.Id).ToListAsync();
            dbGuests.RoomsId = rnumb;

            bool isExist = await _db.Guests.AnyAsync(x => x.RoomsId == dbGuests.RoomsId && x.Id != id && x.IsDeactive == false);
            if (isExist)
            {

                ModelState.AddModelError("Rooms.RoomNumber", "Bu Nömrəli otaq Doludur ");
                return View(dbGuests);
            }

            dbGuests.FullName = guests.FullName;
            dbGuests.PasspNumb = guests.PasspNumb;
            dbGuests.Quantity = guests.Quantity;
            dbGuests.Email = guests.Email;
            dbGuests.EntryDate = guests.EntryDate;
            dbGuests.DepartDate = guests.DepartDate;
            dbGuests.Executor = guests.Executor;


            await _db.SaveChangesAsync();
            return RedirectToAction("GuestsIndex","Notfications");
            #endregion
        }
    }
}
