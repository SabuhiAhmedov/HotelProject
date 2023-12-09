using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HotelProject.Models
{
    public class Guests
    {
      
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime EntryDate { get; set; }//default kimi
        public DateTime DepartDate { get; set; }//daxil ederem
        public int Quantity { get; set; }
        public bool IsDeactive { get; set; }
        [Required(ErrorMessage="Zəhmət olmazsa şəxsiyyət vəsiqəsinin nömrəsini daxil edin")]   
        [MaxLength(9)]
        public string PasspNumb { get; set; }
        public Rooms Rooms { get; set; }
        public int RoomsId { get; set; }
        List<Payments> Payments { get; set; }
        public string Executor { get; set; }
    }
}
