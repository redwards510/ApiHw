using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiHw.Models;
using Newtonsoft.Json;
using System.IO;

namespace ApiHw
{
    public static class Helpers
    {
        /// <summary>
        /// Deserialize fees.json into Fees POCO object
        /// </summary>
        /// <returns></returns>
        public static List<Fees> GetFees()
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader reader = File.OpenText(@".\fees.json"))
            using (JsonReader jr = new JsonTextReader(reader))
            {
                // read the fees.json file into our POCO object
                List<Fees> fees = serializer.Deserialize<List<Fees>>(jr);
                return fees;
            }
        }

        /// <summary>
        /// Reading the fees.json on every call isn't very efficient, but it would normally be in a database anyway.. 
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="pages"></param>
        /// <returns></returns>
        public static decimal LookupFee(string itemType, int pages)
        {
            List<Fees> fees = GetFees();
            var specificType = fees.FirstOrDefault(x => x.order_item_type == itemType);
            var flatFees = specificType.fees.Where(x => x.type == "flat").Sum(x => x.amount);
            var perPageFees = (pages > 1) ? specificType.fees.Where(x => x.type == "per-page").Sum(x => x.amount) * (pages - 1) : 0.0m;
            return flatFees + perPageFees;
        }

    }
}
