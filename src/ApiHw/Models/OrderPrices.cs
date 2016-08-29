using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiHw.Models
{
    public class OrderPrices
    {
        public string OrderId { get; set; }
        public decimal OrderTotal { get; set; }
        public List<OrderItemPrices> OrderItemPrices { get; set; }

        public OrderPrices()
        {
            OrderItemPrices = new List<OrderItemPrices>();
        }
    }


    public class OrderItemPrices
    {
        public decimal Price { get; set; }

        public string ItemName { get; set; }
    }
}
