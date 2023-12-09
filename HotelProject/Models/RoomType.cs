using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelProject.Models
{
    public class RoomType
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Bu giriş boş ola bilməz ")]
        public string Type { get; set; }
        public List<Rooms> Rooms { get; set; }
    }
}
