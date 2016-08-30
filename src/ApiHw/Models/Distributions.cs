using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiHw.Models
{
    public class Distributions
    {
        public Distributions()
        {
            Funds = new List<Fund>();
        }
        public string OrderId { get; set; }
        public List<Fund> Funds { get; set; }
    }

    public class Fund
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }
}
