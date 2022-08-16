using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealership.Models.Queries
{
    // also returns corresponding color name for ease of use in some views
    public class InteriorColors
    {
        public int InteriorId { get; set; }
        public int ColorId { get; set; }
        public string ColorName { get; set; }
    }
}
