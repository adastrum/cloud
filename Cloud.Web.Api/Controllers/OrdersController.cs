using Cloud.Core;
using Cloud.Infrastructure;
using Cloud.Web.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Cloud.Web.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;

        public OrdersController(OrderDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> FindAll()
        {
            var orders = await _context.Orders
                .AsQueryable()
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> FindOne(string id)
        {
            var order = await _context.Orders
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrder model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = new Order(model.Description, model.Amount.Value);

            await _context.Orders.AddAsync(order);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Create), order);
        }

        [HttpPost("{id}/pay")]
        public async Task<IActionResult> Pay(string id, [FromBody] PayOrder model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Orders
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (!order.TryPay(model.Amount.Value, out var errorMessage))
            {
                return BadRequest(errorMessage);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(string id, [FromBody] CancelOrder model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Orders
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (!order.TryCancel(out var errorMessage))
            {
                return BadRequest(errorMessage);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
