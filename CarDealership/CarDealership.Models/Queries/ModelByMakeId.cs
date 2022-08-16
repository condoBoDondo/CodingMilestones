using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealership.Models.Queries
{
    // used for vehicles add/edit pages; additionally gets make name
    public class ModelByMakeId
    {
        public int ModelId { get; set; }
        public int MakeId { get; set; }
        public string ModelName { get; set; }
        public string MakeName { get; set; }
    }
}
