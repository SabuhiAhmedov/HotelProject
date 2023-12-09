using HotelProject.DAL;
using HotelProject.Helpers;
using HotelProject.Models;
using HotelProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelProject.Controllers
{
    public class GuestsController : Controller
    {
        private readonly AppDbContext _db;
        public GuestsController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index(int page =1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.PageCount = Math.Ceiling((decimal)_db.Guests.Count() / 4);
            List<Guests> guests = await _db.Guests.Where(x => x.IsDeactive == false).Skip((page-1)*4).Take(4).Include(x => x.Rooms).ThenInclude(x => x.RoomType).ToListAsync();
            
            await _db.SaveChangesAsync();
            return View(guests);
        }
        public async Task<IActionResult> Create()
        {
            #region Create Guests
            ViewBag.RoomType = await _db.RoomType.ToListAsync();
            RoomType roomType = await _db.RoomType.FirstOrDefaultAsync();

            ViewBag.Rooms = await _db.Rooms.Include(x => x.RoomType).
                Where(x => x.RoomTypeId == roomType.Id && x.IsDeactive == false).Where(x => x.IsBusy == false).ToListAsync();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guests guests, int rnumb)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            ViewBag.RoomType = await _db.RoomType.ToListAsync();
            RoomType roomType = await _db.RoomType.FirstOrDefaultAsync();
            ViewBag.Rooms = await _db.Rooms.Include(x => x.RoomType).Where(x => x.RoomTypeId == roomType.Id && x.IsDeactive == false && x.IsBusy == false).ToListAsync();


            bool isExist = await _db.Guests.AnyAsync(x => x.RoomsId == guests.RoomsId && x.IsDeactive == false);
            if (isExist)
            {
                ModelState.AddModelError("Rooms.RoomNumber", "Bu nömrəli otaq doludur");
                return View();
            }
            guests.RoomsId = rnumb;
            await _db.AddAsync(guests);
            string emailGuests = guests.Email;
            await MailSend.SendMailAsync("LizardHotel ", "Otelimize xos gelmisiniz", emailGuests);
            Rooms rooms = await _db.Rooms.FirstOrDefaultAsync(x => x.Id == rnumb);
            rooms.IsBusy=true;
           
          
            await _db.SaveChangesAsync();
            

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GetRoomsByType(int typeId)
        {
            List<Rooms> rooms = await _db.Rooms.Include(x => x.RoomType).Where(x => x.RoomTypeId == typeId && x.IsDeactive == false && x.IsBusy == false).ToListAsync();
            return PartialView("_RoomNumbersPartial", rooms);
        }

        #endregion
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
            

            ViewBag.Rooms = await _db.Rooms.Include(x => x.RoomType).Where(x => x.RoomTypeId == dbGuests.Rooms.RoomTypeId && x.IsDeactive == false && (x.Id == dbGuests.RoomsId || x.IsBusy == false))
                
                .OrderByDescending(x => x.Id == dbGuests.Rooms.Id).ToListAsync();
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

            ViewBag.Rooms = await _db.Rooms.Include(x => x.RoomType).Where(x => x.RoomTypeId == dbGuests.Rooms.RoomTypeId && x.IsDeactive == false && x.IsBusy == false).OrderByDescending(x => x.Id == dbGuests.Rooms.Id).ToListAsync();
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
            dbGuests.Executor = guests.Executor;
            dbGuests.EntryDate = guests.EntryDate;
            dbGuests.DepartDate = guests.DepartDate;



            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
            #endregion
        }
        public async Task<IActionResult> Detail(int? id)
        {

            #region Guests Detail
            Guests guests = await _db.Guests.Include(x => x.Rooms).ThenInclude(x => x.RoomType).FirstOrDefaultAsync(x => x.Id == id);
            if (id == null)
            {
                return NotFound();

            }
            if (guests == null)
            {
                return BadRequest();
            }
            return View(guests);
            #endregion
        }
        public async Task<IActionResult> Delete(int? id)
        {
            #region Delete Guest from AdminPanel
            Guests dbGuests = await _db.Guests.Include(x => x.Rooms).FirstOrDefaultAsync(x => x.Id == id);
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
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id)
        {
            Guests dbGuests = await _db.Guests.Include(x => x.Rooms).FirstOrDefaultAsync(x => x.Id == id);
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
            return RedirectToAction("Index", "Guests");
            #endregion
        }
        public async Task<IActionResult> SendMail(int? id, string title, string message)
        {
            #region Send Gmail to Guest

            ViewBag.Guest = await _db.Guests.FirstOrDefaultAsync(x => x.Id == id);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("SendMail")]
        public async Task<IActionResult> SendMailPost(int? id, string title, string message)
        {
            ViewBag.Guest = await _db.Guests.FirstOrDefaultAsync(x => x.Id == id);
            MailVM mail = new MailVM()
            {
                Title = title,
                Message = message
            };

            await Helpers.MailSend.SendMailAsync(mail.Title, mail.Message, ViewBag.Guest.Email);

            return RedirectToAction("Index");
            #endregion
        }
        public IActionResult SendMailEveryone(string title,string message)
        {
            #region Send mail to everyone

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("SendMailEveryone")]
        public async Task<IActionResult> SendMailEveryonePostAsync(string title, string message)
        {
            List<Guests> guests = await _db.Guests.Where(x=>x.IsDeactive==false).ToListAsync();
            foreach (Guests item in guests)
            {
                await Helpers.MailSend.SendMailAsync(title, message, item.Email);
            }
            return RedirectToAction("Index");
            #endregion
        }
    }
}