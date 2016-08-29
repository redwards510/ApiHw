using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiHw.Models
{
    public class Order
    {
        public Order()
        {
            order_items = new List<OrderItem>();
        }

        public string order_number { get; set; }
        public string order_date { get; set; }

        public List<OrderItem> order_items {get; set;}

        
    }

    public class OrderItem
    {
        public string type { get; set; }
        public int pages { get; set; }
    }
}
