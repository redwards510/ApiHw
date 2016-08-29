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
    [Route("api/Order")]
    public class OrderController : Controller
    {
        public IOrderRepository OrderRepository;

        public OrderController(IOrderRepository orderRespository)
        {
            OrderRepository = orderRespository;
        }
       
        [HttpPost]
        public IEnumerable<OrderPrices> GetPrices([FromBody] List<Order> orders)
        {

            List<OrderPrices> orderPrices = new List<OrderPrices>();
            foreach (var order in orders)
            {
                var op = new OrderPrices { OrderId = order.order_number };
                foreach(var orderItem in order.order_items)
                {
                    // calculate price for the item
                    op.OrderItemPrices.Add(new OrderItemPrices { ItemName = orderItem.type, Price = LookupFee(orderItem.type, orderItem.pages) });
                }

                op.OrderTotal = op.OrderItemPrices.Sum(x => x.Price);
                orderPrices.Add(op);
            }

            return orderPrices;

        }

        private decimal LookupFee(string itemType, int pages)
        {
            JsonSerializer serializer = new JsonSerializer();            
            

            using (StreamReader reader = System.IO.File.OpenText(@".\fees.json"))
            using(JsonReader jr = new JsonTextReader(reader))
            {
                List<Fees> fees = serializer.Deserialize<List<Fees>>(jr);
                var specificType = fees.FirstOrDefault(x => x.order_item_type == itemType);
                var flatFees = specificType.fees.Where(x => x.type == "flat").Sum(x => x.amount);
                var perPageFees = (pages > 1) ? specificType.fees.Where(x => x.type == "per-page").Sum(x => x.amount) * (pages - 1) : 0.0m;
                return flatFees + perPageFees;
            }
        }
        

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
