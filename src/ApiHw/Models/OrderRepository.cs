using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace ApiHw.Models
{
    public class OrderRepository : IOrderRepository
    {
        private static ConcurrentDictionary<string, Order> _Orders =
              new ConcurrentDictionary<string, Order>();

        public OrderRepository()
        {
            //Random r = new Random();
            //Add(new Order { Date = DateTime.Now.ToString(), Number = r.Next().ToString() });
        }

        public IEnumerable<Order> GetAll()
        {
            return _Orders.Values;
        }

        public void Add(Order order)
        {                        
            _Orders[order.order_number] = order;
        }

        public Order Find(string number)
        {
            Order order;
            _Orders.TryGetValue(number, out order);
            return order;
        }

        public Order Remove(string number)
        {
            Order order;
            _Orders.TryRemove(number, out order);
            return order;
        }

        public void Update(Order order)
        {
            _Orders[order.order_number] = order;
        }
    }
}
