using Cloud.Caching;
using Cloud.CommandStack.Commands;
using Cloud.Core;
using Cloud.Infrastructure;
using Cloud.Messaging;
using Cloud.Web.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Cloud.Web.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly ICommandQueueService _commandQueueService;
        private readonly ICachingService _cachingService;

        public OrdersController(
            OrderDbContext context,
            ICommandQueueService commandQueueService,
            ICachingService cachingService
        )
        {
            _context = context;
            _commandQueueService = commandQueueService;
            _cachingService = cachingService;
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
            if (_cachingService.TryGet<Order>(id, out var cached))
            {
                return Ok(cached);
            }

            var order = await _context.Orders
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            _cachingService.Set(id, order);

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrder model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new CreateOrderCommand(model.Description, model.Amount.Value);

            return await QueueCommandAsync(command);
        }

        [HttpPost("{id}/pay")]
        public async Task<IActionResult> Pay(string id, [FromBody] PayOrder model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new PayOrderCommand(id, model.Amount.Value);

            return await QueueCommandAsync(command);
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new CancelOrderCommand(id);

            return await QueueCommandAsync(command);
        }

        private async Task<IActionResult> QueueCommandAsync(ICommand command)
        {
            var message = JsonConvert.SerializeObject(
                command,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });

            var result = await _commandQueueService.TryAddMessageAsync(message);
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, result.ErrorMessage);
            }

            return Ok();
        }
    }
}
