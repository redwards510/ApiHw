using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApiHw.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace ApiHw.Controllers
{
    [Route("api/Prices")]
    public class PricesController : Controller
    {
        // for when we have a db.. 
        public IOrderRepository OrderRepository;

        public PricesController(IOrderRepository orderRespository)
        {
            OrderRepository = orderRespository;
        }
       
        [HttpPost]
        public IEnumerable<OrderPrices> GetPrices([FromBody] List<Order> orders)
        {

            List<OrderPrices> orderPrices = new List<OrderPrices>();
            // loop over each order in incoming JSON, calculate totals, and add to collection model we'll return automatically as JSON
            foreach (var order in orders)
            {
                // initialize the top level of our output object
                var op = new OrderPrices { OrderId = order.order_number };

                // loop over each item in the order and calculate price
                foreach (var orderItem in order.order_items)
                {                    
                    op.OrderItemPrices.Add(new OrderItemPrices { ItemName = orderItem.type, Price = LookupFee(orderItem.type, orderItem.pages) });
                }

                // get the overall order total
                op.OrderTotal = op.OrderItemPrices.Sum(x => x.Price);
                orderPrices.Add(op);
            }
            // .Net handles the conversion of the POCO class to JSON
            return orderPrices;
        }

        /// <summary>
        /// Reading the fees.json on every call isn't very efficient, but it would normally be in a database anyway.. 
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="pages"></param>
        /// <returns></returns>
        private decimal LookupFee(string itemType, int pages)
        {
            JsonSerializer serializer = new JsonSerializer();                        
            using (StreamReader reader = System.IO.File.OpenText(@".\fees.json"))
            using(JsonReader jr = new JsonTextReader(reader))
            {
                // read the fees.json file into our POCO object
                List<Fees> fees = serializer.Deserialize<List<Fees>>(jr);
                var specificType = fees.FirstOrDefault(x => x.order_item_type == itemType);
                var flatFees = specificType.fees.Where(x => x.type == "flat").Sum(x => x.amount);
                var perPageFees = (pages > 1) ? specificType.fees.Where(x => x.type == "per-page").Sum(x => x.amount) * (pages - 1) : 0.0m;
                return flatFees + perPageFees;
            }
        }                
    }
}
