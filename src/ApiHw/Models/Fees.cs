using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ApiHw.Models
{
    public class Fees
    {
        public string order_item_type { get; set; }
        public List<Fee> fees { get; set; }

        public List<Distribution> distributions { get; set; }

    }


    public class Fee
    {
        public string name { get; set; }
        public decimal amount { get; set; }
        public string type { get; set; }
    }

    public class Distribution
    {
        public string name { get; set; }
        public decimal amount { get; set; }
        public string type { get; set; }
    }

    public enum FeeType
    {
        [Description("per-page")]
        PerPage,
        [Description("flat")]
        Flat
    }
}
