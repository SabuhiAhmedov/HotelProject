using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelProject.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public int Quantity { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime DepartDate { get; set; }
        public bool IsDeactive { get; set; }
        public Rooms Rooms { get; set; }
        public int RoomsId { get; set; }
        public string Executor { get; set; }
      
    }
}
