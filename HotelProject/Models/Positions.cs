using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelProject.Models
{
    public class Positions
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Zəhmət olmasa vəzifənin adını daxil edin")]
        public string Name { get; set; }
        public bool IsDeactive { get; set; }
        public List<Staffs> Staffs { get; set; }
        public Department Department { get; set; }
        public int DepartmentId { get; set; }

    }
}
