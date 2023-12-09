using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelProject.Models
{
    public class Department
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Zəhmət olmasa Departamentin adını daxil edin")]
        public String Name { get; set; }
        public bool IsDeactive { get; set; }
        public List<Positions> Positions { get; set; }
   

    }
}
