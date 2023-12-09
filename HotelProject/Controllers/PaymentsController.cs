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
    public class PaymentsController : Controller
    {
        private readonly AppDbContext _db;
        public PaymentsController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index(int page=1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.PageCount = Math.Ceiling((decimal)_db.Payments.Count() / 4);
            List<Payments> payments = await _db.Payments.Skip((page-1)*4).Take(4).Include(x => x.Guests).ToListAsync();
            return View(payments);
        }
        public async Task<IActionResult> SendPaymentsSum()
        {
            #region Indexe Odenislerin cemini gonderir
            List<Payments> payments = await _db.Payments.Include(x => x.Guests).ToListAsync();
            int SumPayments = 0;
            foreach (Payments item in payments)
            {
                SumPayments += item.Amount;
            }
            return Json(SumPayments);
            #endregion
        }
        public async Task<IActionResult> Create(Payments payment)
        {
            #region Create Payments Guests
            ViewBag.Guests = await _db.Guests.Where(x => x.IsDeactive == false).ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payments payment, int guestsid, CashRegister newCash)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            ViewBag.Guests = await _db.Guests.Where(x => x.IsDeactive == false).ToListAsync();
            payment.GuestsId = guestsid;

            Guests guests = await _db.Guests.FirstOrDefaultAsync(x => x.Id == guestsid);
            //AddCashRegister this Payment
            CashRegister cashRegister = new CashRegister()
            {
                Amount = payment.Amount,
                LastChangeTime = payment.Time,
                Executor = payment.Executor,
                ChangeInformation = "Qonaq "+guests.FullName+" ödəniş etdi" ,
                Id = newCash.Id
            };
            //await _db.Payments.AddAsync(payment);
            ////Kassaya Odenisi elave etmek
            //await _db.CashRegister.AddAsync(cashRegister);
            //await _db.SaveChangesAsync();
            return View();
            #endregion
        }

        public async Task<ActionResult> GetPaymentAmount(int customerId)
        {
            #region For Rent Add to Input , Send data
            Guests guests = await _db.Guests.Include(x => x.Rooms).FirstOrDefaultAsync(x => x.Id == customerId);
            int paymentAmount = guests.Rooms.Rent;
            return Json(paymentAmount);
            #endregion
        }
      
    }
}
