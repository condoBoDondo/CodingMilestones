using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealership.Models.Tables
{
    public class Interior
    {
        public int InteriorId { get; set; }
        public int ColorId { get; set; } // refs Color.ColorId
    }
}
