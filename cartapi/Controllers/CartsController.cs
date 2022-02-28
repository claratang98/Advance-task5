using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cartapi.Models;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;

namespace cartapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly CartDbContext _context;
        private readonly IConfiguration env;
        public CartsController(CartDbContext context, IConfiguration env)
        {
            _context = context;
            this.env = env;
        }

        // GET: api/Carts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCart()
        {
            return await _context.Cart.ToListAsync();
        }

        // GET: api/Carts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> GetCart(int id)
        {
            var cart = await _context.Cart.FindAsync(id);

            if (cart == null)
            {
                return NotFound();
            }

            return cart;
        }

        // PUT: api/Carts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCart(int id, Cart cart)
        {
            if (id != cart.CartId)
            {
                return BadRequest();
            }

            _context.Entry(cart).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Carts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cart>> PostCart(Cart cart)
        {
            cart.OrderStatus = "INITITATED";
            _context.Cart.Add(cart);
            await _context.SaveChangesAsync();

            var factory = new ConnectionFactory()
            {
                HostName = env.GetSection("RABBITMQHOST").Value,
                Port = Convert.ToInt32(env.GetSection("RABBITMQPORT").Value),
                UserName = env.GetSection("RABBITUSER").Value,
                Password = env.GetSection("RABBITPASSWORD").Value
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "orders", durable: false, exclusive: false, autoDelete: false, arguments: null);

                string message = "Type: ORDER_CREATED | Cart ID:" + cart.CartId + "|Product Id:" + cart.ProductId + "|Product Prices:" + cart.ProductPrices + "|Total:" + cart.Total + "|Order Status:" + cart.OrderStatus + "|Order Id:" + cart.OrderId; 
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "", routingKey: "orders", basicProperties: null, body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }

            return CreatedAtAction("GetCart", new { id = cart.CartId }, cart);
        }

        // DELETE: api/Carts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            var cart = await _context.Cart.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            _context.Cart.Remove(cart);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CartExists(int id)
        {
            return _context.Cart.Any(e => e.CartId == id);
        }
    }
}
