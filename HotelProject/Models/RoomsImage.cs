using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelProject.Models;

namespace HotelProject.Models
{
    public class RoomsImage
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public Rooms Rooms { get; set; }
        public int RoomsId { get; set; }

    }
}
