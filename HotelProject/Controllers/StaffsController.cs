using HotelProject.DAL;
using HotelProject.Helpers;
using HotelProject.Models;
using HotelProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static HotelProject.Helpers.Helper;

namespace HotelProject.Controllers
{
   
    public class StaffsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public StaffsController(AppDbContext db, IWebHostEnvironment env)
        {
            _env = env;
            _db = db;
            
        }
        public async Task<IActionResult> Index( int page =1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.PageCount = Math.Ceiling((decimal)_db.Staffs.Count() / 4);
            List<Staffs> staffs = await _db.Staffs.OrderByDescending(x => x.IsDeactive == false).Skip((page-1)*4).Take(4).Include(x => x.Positionn).ToListAsync();
            return View(staffs);
        }

        public async Task<IActionResult> Create()
        #region Create Staffs
        {
            ViewBag.Positions = await _db.Positions.Where(x => x.IsDeactive == false).ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Staffs staffs, int posid)
        {
            ViewBag.Positions = await _db.Positions.Where(x => x.IsDeactive == false).ToListAsync();
            staffs.PositionnId = posid;//Positiondaki id ile staafdaki positionidni berberlesdirir

            if (!ModelState.IsValid)
            {
                return View();
            }
            if (staffs.Photo == null)
            {
                ModelState.AddModelError("Photo", "Sekil secin");
                return View();
            }
            if (staffs.Photo.IsOlderTwoMb())
            {
                ModelState.AddModelError("Photo", "Zehmet olmazsa 2 mb dan az olculu sekil secin");
                return View();
            }
            if (!staffs.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "Sekil secin");
                return View();
            }
            bool IsExist = await _db.Staffs.AnyAsync(x => x.Email == staffs.Email);
            if (IsExist)
            {
                ModelState.AddModelError("Email", "Bu emailli isci movcuddur");
                return View();
            }
            string folder = Path.Combine(_env.WebRootPath, "assets", "img");
            staffs.Image = await staffs.Photo.SaveFileAsync(folder);

            await _db.Staffs.AddAsync(staffs);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Staffs");
        }
        #endregion

        public async Task<IActionResult> Update(int? id)
        #region Update Staffs
        {

            Staffs dbStaffs = await _db.Staffs.FirstOrDefaultAsync(x => x.Id == id);//ID si id beraber olani goturur
            if (id == null)
            {
                return NotFound();
            }
            if (dbStaffs == null)
            {
                return BadRequest();
            }
            ViewBag.Positions = await _db.Positions.Where(x => x.IsDeactive == false).ToListAsync();
            return View(dbStaffs);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Staffs staffs, int posid)
        {
            ViewBag.Positions = await _db.Positions.Where(x => x.IsDeactive == false).ToListAsync();
            Staffs dbStaffs = await _db.Staffs.FirstOrDefaultAsync(x => x.Id == id);//ID si id beraber olani goturur
            dbStaffs.PositionnId = posid;//biz databaseni deysmeliyjde

            if (id == null)
            {
                return NotFound();
            }
            if (dbStaffs == null)
            {
                return BadRequest();
            }
            if (staffs.Photo != null)
            {
                if (staffs.Photo.IsOlderTwoMb())
                {
                    ModelState.AddModelError("Photo", "2 mbdan asagi yaddasli sekil secin");
                    return View(dbStaffs);
                }
                if (!staffs.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Sekil secin");
                    return View(dbStaffs);
                }


                string folder = Path.Combine(_env.WebRootPath, "assets", "img");
                dbStaffs.Image = await staffs.Photo.SaveFileAsync(folder);
            }
            bool isExist = await _db.Staffs.AnyAsync(x => x.Email == staffs.Email && x.Id != id);
            if (isExist)
            {
                ModelState.AddModelError("Email", "Bu emaile malik bashqa isci vardir");
                return View(dbStaffs);
            }
            dbStaffs.FullName = staffs.FullName;
            dbStaffs.Adress = staffs.Adress;
            dbStaffs.Email = staffs.Email;
            dbStaffs.JoinDate = staffs.JoinDate;
            dbStaffs.Number = staffs.Number;
            dbStaffs.Salary = staffs.Salary;

            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Staffs");
        }
        #endregion

        public async Task<IActionResult> Activity(int? id)
        #region  Staffs Activity
        {
            Staffs dbStaffs = await _db.Staffs.FirstOrDefaultAsync(x => x.Id == id);
            if (id == null)
            {
                return NotFound();
            }
            if (dbStaffs == null)
            {
                return BadRequest();
            }
            if (dbStaffs.IsDeactive)
            {
                dbStaffs.IsDeactive = false;
            }
            else
            {
                dbStaffs.IsDeactive = true;
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Staffs");
        }
        #endregion
        public async Task<IActionResult> Detail(int? id)
        #region Staffs Detail

        {
            Staffs dbStaffs = await _db.Staffs.Include(x => x.Positionn.Department).FirstOrDefaultAsync(x => x.Id == id);
            if (id == null)
            {
                return NotFound();
            }
            if (dbStaffs == null)
            {
                return BadRequest();
            }
            return View(dbStaffs);
        }
        #endregion
        public async Task<IActionResult> SendMail(int? id, string title, string message)
        {
            #region Send Gmail to Staff

            ViewBag.Staffs = await _db.Staffs.FirstOrDefaultAsync(x => x.Id == id);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("SendMail")]
        public async Task<IActionResult> SendMailPost(int? id, string title, string message)
        {
            ViewBag.Staffs = await _db.Staffs.FirstOrDefaultAsync(x => x.Id == id);
            MailVM mail = new MailVM()
            {
                Title=title,
                Message=message
            };

            await Helpers.MailSend.SendMailAsync(mail.Title, mail.Message, ViewBag.Staffs.Email);

            return RedirectToAction("Index");
            #endregion
        }
        public IActionResult SendMailEveryone(string title, string message)
        {
            #region Send mail to everyone

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("SendMailEveryone")]
        public async Task<IActionResult> SendMailEveryonePostAsync(string title, string message)
        {
            List<Staffs> staffs = await _db.Staffs.Where(x=>x.IsDeactive==false).ToListAsync();
            foreach (Staffs item in staffs)
            {
                await Helpers.MailSend.SendMailAsync(title, message, item.Email);
            }
            return RedirectToAction("Index");
            #endregion
        }
    }
}
