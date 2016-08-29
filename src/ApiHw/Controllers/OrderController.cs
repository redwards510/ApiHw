using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApiHw.Models;
using Newtonsoft.Json.Linq;

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


        //[HttpPost]
        //public Order GetPrices([FromBody] Order order)
        //{
        //    return order;
        //}

        
        [HttpPost]
        public IEnumerable<OrderPrices> GetPrices([FromBody] List<Order> orders)
        {
            //List<Order> orders = new List<Order>();

            //orders.Add(new Order { order_date = jorders.})
            //var ordersArray = jorders.ToObject<List<OrderPrices>>();

            List<OrderPrices> orderPrices = new List<OrderPrices>();
            foreach (var order in orders)
            {
                var op = new OrderPrices { OrderId = order.order_number };
                foreach(var orderItem in order.order_items)
                {
                    // calculate price for the item
                    op.OrderItemPrices.Add(new OrderItemPrices { ItemName = orderItem.type, Price = 5.00 });
                }

                op.OrderTotal = op.OrderItemPrices.Sum(x => x.Price);
                orderPrices.Add(op);
            }

            return orderPrices;

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
