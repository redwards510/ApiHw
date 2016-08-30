using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApiHw.Models;
using Newtonsoft.Json;
using System.IO;

namespace ApiHw.Controllers
{
    [Route("api/Distributions")]
    public class DistributionsController : Controller
    {        
        [HttpPost]
        public List<Distributions> Post([FromBody] List<Order> orders)
        {
            List<Distributions> distributions = new List<Distributions>();
            List<Fees> fees = GetFees();

            foreach (var order in orders)
            {
                // create top level distribution object
                var d = new Distributions { OrderId = order.order_number };

                // get the unique order types in case of duplicates
                var uniqueTypes = order.order_items.Select(x => x.type).Distinct();                

                // loop over the unique types
                foreach (var orderItemType in uniqueTypes)
                {                    
                    var countOfItem = order.order_items.Where(x => x.type == orderItemType).Count();
                    var totalPages = order.order_items.Where(x => x.type == orderItemType).Sum(x => x.pages);
                    // loop over each distribution under the order item type and add to output DTO                                           
                    foreach (var dist in fees.FirstOrDefault(x => x.order_item_type == orderItemType).distributions)
                    {
                        d.Funds.Add(new Fund { Amount = dist.amount * countOfItem, Name = dist.name });
                    }                    
                }

                // calculate order total from prices so we can determine Other fund
                decimal orderTotal = 0m;
                foreach (var orderItem in order.order_items)
                {
                    orderTotal += LookupFee(orderItem.type, orderItem.pages);
                }

                // check if we need to add to the Other fund
                var other = orderTotal - d.Funds.Sum(x => x.Amount);
                if (other > 0)
                {
                    d.Funds.Add(new Fund { Amount = other, Name = "Other" });
                }

                distributions.Add(d);
            }
            return distributions;
        }

        /// <summary>
        /// Deserialize fees.json into Fees POCO object
        /// </summary>
        /// <returns></returns>
        private List<Fees> GetFees()
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader reader = System.IO.File.OpenText(@".\fees.json"))
            using (JsonReader jr = new JsonTextReader(reader))
            {
                // read the fees.json file into our POCO object
                List<Fees> fees = serializer.Deserialize<List<Fees>>(jr);
                return fees;
            }
        }

        /// <summary>
        /// this should be refactored into a helper class so we aren't duplicating methods. 
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="pages"></param>
        /// <returns></returns>
        private decimal LookupFee(string itemType, int pages)
        {
            List<Fees> fees = GetFees();
            var specificType = fees.FirstOrDefault(x => x.order_item_type == itemType);
            var flatFees = specificType.fees.Where(x => x.type == "flat").Sum(x => x.amount);
            var perPageFees = (pages > 1) ? specificType.fees.Where(x => x.type == "per-page").Sum(x => x.amount) * (pages - 1) : 0.0m;
            return flatFees + perPageFees;            
        }
    }
}
