using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelProject.Models
{
    public class Salary
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Zəhmət olmasa pulun miqdarını daxil edin")]
        public int Amount { get; set; }
        
        public DateTime Time { get; set; }

        public Staffs Staffs { get; set; }
        public int StaffsId { get; set; }
        public string Executor { get; set; }
    }
}
