using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelProject.Models
{
    public class Income
    {
        public int Id { get; set; }
        public string Source { get; set; }
        [Required(ErrorMessage ="Zəhmət olmasa məbləği daxil edin")]
        public int Amount { get; set; } 
        public DateTime Time { get; set; }
        public string Executor { get; set; }
    }
}
