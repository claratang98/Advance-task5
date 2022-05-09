using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using orderconsumerapi.Models;

namespace orderconsumerapi.Repository
{
    public interface IOrderconsumerRepoe
    {
        // CRUD on data store

        Orderconsumer GetOrderconsumer(int CartId);

        Orderconsumer CreateOrderconsumer(Orderconsumer order);

        Orderconsumer UpdateOrderconsumer(Orderconsumer order);

        Orderconsumer DeleteOrderconsumer(int CartId);

    }
}

