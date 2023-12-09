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
    public class IncomeController : Controller
    {
        private readonly AppDbContext _db;
        public IncomeController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index(int page=1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.PageCount = Math.Ceiling((decimal)_db.Incomes.Count() / 4);
            List<Income> incomes = await _db.Incomes.Skip((page - 1) * 4).Take(4).ToListAsync();
            return View(incomes);
        }
        public async Task<IActionResult> GetSumIncomes()
        {
            #region Indexe gelirlerin cemini gonderir
            List<Income> incomes = await _db.Incomes.ToListAsync();
            int SumIncomes = 0;
            foreach (Income item in incomes)
            {
                SumIncomes += item.Amount;
            }
            return Json(SumIncomes);
            #endregion
        }
        public IActionResult Create()
        {
            #region Create Incomes   
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Income income,CashRegister newCash)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            List<Expensesies> expence = await _db.Expensesies.ToListAsync();
            //AddCashRegister incomes
            CashRegister cashRegister = new CashRegister()
            {
                Amount = income.Amount,
                LastChangeTime = income.Time,
                ChangeInformation ="Gəlir: "+ income.Source,
                Executor = income.Executor,
                Id = newCash.Id
            };
            
            await _db.AddAsync(income);
            await _db.AddAsync(cashRegister);
          
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
            #endregion
        }
      
    }
}
