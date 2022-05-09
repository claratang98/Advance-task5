using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using orderconsumerapi.Models;

namespace orderconsumerapi.Data
{
    public static class DataSeed
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Orderconsumer>().HasData(
                new Orderconsumer
                {
                     CartId = 1,
                     ProductId = "123abc",
                     ProductPrices = 0,
                     Total = 0,
                     OrderStatus = "NIL",
                     OrderId = "456def"
                });
        }
    }
}

