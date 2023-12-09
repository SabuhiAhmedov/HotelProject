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
    public class CashRegisterController : Controller
    {
        private readonly AppDbContext _db;
        public CashRegisterController(AppDbContext db)
        {
                _db=db;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.CashRegister = await _db.CashRegister.ToListAsync(); 

            return View();
        }
        public async Task<IActionResult> SendDataHome()
        {
            List<CashRegister> cashRegisters = await _db.CashRegister.ToListAsync();
            int sumCash = 0;
            foreach (CashRegister item in cashRegisters)
            {
                sumCash += item.Amount;
            }
            return Json(sumCash);
        }
    }
}
