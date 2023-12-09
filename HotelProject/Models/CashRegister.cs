using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelProject.Models
{
    public class CashRegister
    { 
        public int Id { get; set; }
        public DateTime LastChangeTime { get;set; }
        public int Amount { get; set; }
        public string Executor { get; set; }
        public string ChangeInformation { get; set; }
    }
}
