using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace orderconsumerapi.Models
{
    public class Orderconsumer
    { 
        public Orderconsumer()
        {
        }
        public Orderconsumer(int cartid, string productid, double productprices, double total, string orderstatus, string orderid)
        {
            CartId = cartid;
            ProductId = productid;
            ProductPrices = productprices;
            Total = total;
            OrderStatus = orderstatus;
            OrderId = orderid;
        }
        [Key]
        public int CartId { get; set; }
        public string ProductId { get; set; }

        public double ProductPrices { get; set; }

        public double Total { get; set; }

        public string OrderStatus { get; set; }
        public string OrderId { get; set; }
    }
}
