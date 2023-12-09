using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HotelProject.Models
{
    public class Staffs
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Zəhmət olmasa işçinin adını daxil edin")]
        public string FullName { get; set; }
        public string Image { get; set; }
        public string Position { get; set; }
        public int Salary { get; set; }
        public string Number{ get; set; }
        
        public string Email { get; set; }
        public string Adress { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsDeactive { get; set; }
        public Positions Positionn { get; set; }
        public int PositionnId { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
        List<Salary>Salaries { get; set; }
    }
}
