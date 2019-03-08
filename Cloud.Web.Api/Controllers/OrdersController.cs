using Cloud.Infrastructure;
using Cloud.Web.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace Cloud.Web.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly CloudQueue _queue;

        public OrdersController(
            OrderDbContext context,
            CloudStorageAccount cloudStorageAccount
        )
        {
            _context = context;
            var cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
            _queue = cloudQueueClient.GetQueueReference("commandqueue");
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

            await _queue.CreateIfNotExistsAsync();

            var message = new CloudQueueMessage(JsonConvert.SerializeObject(model));

            await _queue.AddMessageAsync(message);

            return CreatedAtAction(nameof(Create), model);
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
