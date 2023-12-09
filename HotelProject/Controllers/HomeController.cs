using HotelProject.DAL;
using HotelProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HotelProject.Controllers
{
   

    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
             CashRegister cashRegisters = await _db.CashRegister.OrderByDescending(x=>x.Id).FirstOrDefaultAsync();
            return View(cashRegisters);
        }

    
      
        public IActionResult Error()
        {
            return View();
        }
    }
}
