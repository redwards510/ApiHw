using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiHw.Models
{
    public interface IOrderRepository
    {
        void Add(Order order);
        IEnumerable<Order> GetAll();
        Order Find(string key);
        Order Remove(string key);
        void Update(Order order);

    }
}
