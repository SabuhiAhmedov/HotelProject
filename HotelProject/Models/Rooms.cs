using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HotelProject.Models
{
    public class Rooms
    {
        public int Id { get; set; }

        public int Rent { get; set; }
        public bool IsDeactive { get; set; }
        public bool IsBusy { get; set; }
        public int Capacity { get; set; }
        public int Phone { get; set; }
        public int RoomNumber { get; set; }
        
        public RoomType RoomType { get; set; }
        public int RoomTypeId { get; set; }
        [NotMapped]
      
        public IFormFile[] Photos { get; set; }
        public List<RoomsImage> RoomsImages { get; set; }
        public List<Reservation> Reservations { get; set; }
        //public List<Guests> Guests { get; set; }
    }
}
