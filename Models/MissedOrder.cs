using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmacyControl.Models
{
    public class MissedOrder
    {
        public double OrderNumber { get; set; }
        public string PharmacyName { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid StoreId { get; set; }
        public string Source { get; set; }

    }
}
