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
    public class ExpensesiesController : Controller
    {
        private readonly AppDbContext _db;
        public ExpensesiesController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index(int page=1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.PageCount = Math.Ceiling((decimal)_db.Expensesies.Count() / 4);
            List<Expensesies> expensesies = await _db.Expensesies.Skip((page - 1) * 4).Take(4).ToListAsync();
            return View(expensesies);
        }
        public async Task<IActionResult> GetSumExpence()
        {
            #region Indexe xerclerin cemini gonderir
            List<Expensesies> expensesies = await _db.Expensesies.ToListAsync();
            int SumExpence = 0;
            foreach (Expensesies item in expensesies)
            {
                SumExpence += item.Amount;
            }
            return Json(SumExpence);
            #endregion
        }
        public IActionResult Create()
        {
            #region Create Expenses   
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expensesies expensesies,CashRegister newCash)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            CashRegister cashRegister = new CashRegister()
            {
                Amount = -expensesies.Amount,
                LastChangeTime = expensesies.Time,
                ChangeInformation ="Xərc: " +expensesies.Source ,
                Executor = expensesies.Executor,
                Id = newCash.Id
            };
            await _db.AddAsync(expensesies);
            await _db.AddAsync(cashRegister);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
            #endregion
        }
       
    }
}
