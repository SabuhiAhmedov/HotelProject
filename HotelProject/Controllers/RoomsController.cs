using HotelProject.DAL;
using HotelProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelProject.Helpers;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using static HotelProject.Helpers.Helper;
using Microsoft.AspNetCore.Authorization;

namespace HotelProject.Controllers
{
  
    public class RoomsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public RoomsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        public async Task<IActionResult> Index(int page=1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.PageCount = Math.Ceiling((decimal)_db.Rooms.Count() / 4);
            List<Rooms> rooms = await _db.Rooms.OrderByDescending(x => x.IsBusy == false&&x.IsDeactive==false).Skip((page-1)*4).Take(4).Include(x => x.RoomType).Include(x=>x.RoomsImages).ToListAsync();
            return View(rooms);
        }
        #region Create Rooms...
        public async Task<IActionResult> Create(int typeid)

        {
            ViewBag.RoomType = await _db.RoomType.ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Rooms rooms,int typeid)
        {
            ViewBag.RoomType = await _db.RoomType.ToListAsync();
            rooms.RoomTypeId = typeid; 
            
            if (!ModelState.IsValid)
            {
                
                return View();
               
            }

            bool IsExist = await _db.Rooms.AnyAsync(x => x.RoomNumber == rooms.RoomNumber);
            if (IsExist)
            {
                ModelState.AddModelError("RoomNumber", "Bu Nömrəli otaq mövcuddur");
                return View();
            }
            if (rooms.Photos == null)
            {
                ModelState.AddModelError("Photos", "Zəhmət olmasa şəkil seçin");
                return View();
            }
            List<RoomsImage> roomsImages = new List<RoomsImage>();
            
            
                foreach (IFormFile photos in rooms.Photos)
                {
                   
                    if (photos.IsOlderTwoMb())
                    {
                        ModelState.AddModelError("Photos", "Zehmet olmazsa 2 mb dan az olculu sekil secin");
                        return View();
                    }
                    if (!photos.IsImage())
                    {
                        ModelState.AddModelError("Photos", "Sekil secin");
                        return View();
                    }
                    string folder = Path.Combine(_env.WebRootPath, "assets", "img");
                    RoomsImage roomsImage = new RoomsImage();
                    roomsImage.Image = await photos.SaveFileAsync(folder);
                    roomsImages.Add(roomsImage);
                }
                  rooms.RoomsImages = roomsImages;
            
            await _db.Rooms.AddAsync(rooms);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Rooms");
        }
        #endregion

        #region Update Rooms ......
        public async Task<IActionResult> Update(int? id)
        {
            Rooms rooms = await _db.Rooms.FirstOrDefaultAsync(x => x.Id == id);
            ViewBag.RoomType = await _db.RoomType.OrderByDescending(x=>x.Id==rooms.RoomTypeId).ToListAsync();

            Rooms dbRooms = await _db.Rooms.Include(x => x.RoomType).Include(x => x.RoomsImages).FirstOrDefaultAsync(x => x.Id == id);
            if (id == null)
            {
                return NotFound();
            }
            if (dbRooms == null)
            {
                return BadRequest();
            }
            return View(dbRooms);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Rooms rooms,int typeid)
        {
            ViewBag.RoomType = await _db.RoomType.ToListAsync();
         
            Rooms dbRooms = await _db.Rooms.Include(x => x.RoomType).Include(x => x.RoomsImages).FirstOrDefaultAsync(x => x.Id == id);

            if (!ModelState.IsValid)
            {
                return View(dbRooms);
            }
            bool IsExist = await _db.Rooms.AnyAsync(x => x.RoomNumber == rooms.RoomNumber && x.Id != id);
            if (IsExist)
            {
                ModelState.AddModelError("RoomNumber", "Bu nömrəli otaq artq mövcuddur");
                return View(dbRooms);
            }
            List<RoomsImage> roomsImages = new List<RoomsImage>();
            if (rooms.Photos != null)
            {

                foreach (IFormFile photos in rooms.Photos)
                {
                    if (photos == null)
                    {
                        ModelState.AddModelError("Photos", "Sekil secin");
                        return View(dbRooms);
                    }
                    if (photos.IsOlderTwoMb())
                    {
                        ModelState.AddModelError("Photos", "Zehmet olmazsa 2 mb dan az olculu sekil secin");
                        return View(dbRooms);
                    }
                    if (!photos.IsImage())
                    {
                        ModelState.AddModelError("Photos", "Sekil secin");
                        return View(dbRooms);
                    }
                    string folder = Path.Combine(_env.WebRootPath, "assets", "img");
                    RoomsImage roomsImage = new RoomsImage();
                    roomsImage.Image = await photos.SaveFileAsync(folder);
                    roomsImages.Add(roomsImage);
                }
                dbRooms.RoomsImages.AddRange(roomsImages);
            }
            dbRooms.RoomTypeId = typeid;
            dbRooms.Capacity = rooms.Capacity;
            dbRooms.Phone = rooms.Phone;
            dbRooms.RoomNumber = rooms.RoomNumber;
           
            dbRooms.Rent = rooms.Rent;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Rooms Activity......
        public async Task<IActionResult> Activity(int? id)
        {
            #region Activity Rooms
            Rooms dbRooms = await _db.Rooms.FirstOrDefaultAsync(x => x.Id == id);
            if (id == null)
            {
                return NotFound();
            }
            if (dbRooms == null)
            {
                return BadRequest();
            }
            if (dbRooms.IsDeactive)
            {
                dbRooms.IsDeactive = false;
            }
            else
            {
                dbRooms.IsDeactive = true;
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
            #endregion
        }
        #endregion
        #region Delete Rooms Images
        public async Task<IActionResult> DeleteRoomsImage(int imgId,int count)
        {
            if (count == 1)
            {
                return Content("error");
            }
            RoomsImage roomsImage = await _db.RoomsImage.FirstOrDefaultAsync(x=>x.Id==imgId);
            string path = Path.Combine(_env.WebRootPath, "assets", "img",roomsImage.Image);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            _db.RoomsImage.Remove(roomsImage);
            await _db.SaveChangesAsync();
            return Ok();
        }
        #endregion
        public async Task<IActionResult> Detail(int? id)
        {
            Rooms DbRooms = await _db.Rooms.Include(x => x.RoomsImages).Include(x => x.RoomType).FirstOrDefaultAsync(x => x.Id == id);
            if (id == null)
            {
                return NotFound();
            }
            if (DbRooms == null)
            {
                return BadRequest();
            }
            return View(DbRooms);
        }
    }

}