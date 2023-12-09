using HotelProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelProject.DAL
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Staffs> Staffs { get; set; }
        public DbSet<Positions> Positions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Rooms> Rooms { get; set; }
        public DbSet<RoomType> RoomType { get; set; }
        public DbSet<RoomsImage> RoomsImage { get; set; }
         public DbSet<Reservation> Reservation { get; set; }
        public DbSet<Guests> Guests { get; set; }

        public DbSet<Payments> Payments { get; set; }
        public DbSet<Salary> Salary { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<Expensesies> Expensesies { get; set; }
      public DbSet<CashRegister> CashRegister { get; set; }



    }
}