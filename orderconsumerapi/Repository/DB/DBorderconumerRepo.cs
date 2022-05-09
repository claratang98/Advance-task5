using orderconsumerapi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using orderconsumerapi.Data;

namespace orderconsumerapi.Repository.DB
{
    public class DBorderconsumerRepo : IOrderconsumerRepoe
    {
        OrderconsumerDbContext _dbContext = null;
        public DBorderconsumerRepo(OrderconsumerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Orderconsumer CreateOrderconsumer(Orderconsumer cartid)
        {
            _dbContext.Orderconsumers.Add(cartid); // it actualy add the data into Disconnected database
            _dbContext.SaveChanges(); // commit
            return cartid;
        }

        public Orderconsumer DeleteOrderconsumer(int cartid)
        {
            Orderconsumer order = _dbContext.Orderconsumers.FirstOrDefault(e => e.CartId == cartid);
            _dbContext.Orderconsumers.Remove(order);
            _dbContext.SaveChanges();
            return order;
        }

        public Orderconsumer GetOrderconsumer(int cartid)
        {
            return _dbContext.Orderconsumers.FirstOrDefault(e => e.CartId == cartid);
        }

        public Orderconsumer UpdateOrderconsumer(Orderconsumer cartid)
        {
            _dbContext.Orderconsumers.Update(cartid);
            _dbContext.SaveChanges();
            return cartid;
        }
    }
}
