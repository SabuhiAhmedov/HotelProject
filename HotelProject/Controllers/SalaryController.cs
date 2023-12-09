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
    public class SalaryController : Controller
    {
        private readonly AppDbContext _db;
        public SalaryController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index(int page=1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.PageCount = Math.Ceiling((decimal)_db.Salary.Count() / 4);
            List<Salary> salaries = await _db.Salary.Skip((page-1)*4).Take(4).Include(x=>x.Staffs).ToListAsync();
           
            
            return View(salaries);
        }
        public async Task<IActionResult> SendSalarySum()
        {
            #region Indexe Maaslarin cemini gonderir
            List<Salary> salaries = await _db.Salary.Include(x=>x.Staffs).ToListAsync();
            int SumSalary = 0;
            foreach (Salary item in salaries)
            {
                SumSalary += item.Amount;
            }
            return Json(SumSalary);
            #endregion
        }
        public async Task<IActionResult> Create()
        {
            #region Create Salary Staffs
            ViewBag.Staffs = await _db.Staffs.Where(x=>x.IsDeactive==false).ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Salary salary , int staffid,CashRegister newCash)
        {
            ViewBag.Staffs = await _db.Staffs.Where(x => x.IsDeactive == false).ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();
            }           
            salary.StaffsId = staffid;
            Staffs staffs = await _db.Staffs.FirstOrDefaultAsync(x => x.Id == staffid);
            //AddCashRegister salaries
            CashRegister cashRegister = new CashRegister()
            {
                Amount = -salary.Amount,
                LastChangeTime = salary.Time,
                Executor = salary.Executor,
                ChangeInformation = staffs.FullName +" Adlı işçiyə maaş ödənildi",
                Id = newCash.Id
            };
            await _db.AddAsync(salary);
            await _db.AddAsync(cashRegister);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
            #endregion

        }
        public async Task<IActionResult> GetSalaryAmount(int customerId)
        {
            #region GetSalaryAmount
            Staffs staffs = await _db.Staffs.FirstOrDefaultAsync(x => x.Id == customerId);
            int staffAmount = staffs.Salary;
            return Json(staffAmount);
            #endregion 
        }
        
    }
}
