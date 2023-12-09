using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelProject.Models
{
    public class Payments
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Ödənişin miqdarını daxil edin")]
        public int Amount { get; set; }
        public Guests Guests { get; set; }
        public int GuestsId { get; set; }
        public DateTime Time { get; set; }
        public string Executor { get; set; }
    }
}
