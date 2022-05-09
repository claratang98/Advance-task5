using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orderconsumer.Models
{
    public class OrderConsumer
    {
        public int CartId { get; set; }

        public string ProductId { get; set; }

        public double ProductPrices { get; set; }

        public double Total { get; set; }

        public string OrderStatus { get; set; }
        public string OrderId { get; set; }
    }
}