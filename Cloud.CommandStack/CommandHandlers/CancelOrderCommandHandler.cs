using Cloud.CommandStack.Commands;
using Cloud.Infrastructure;
using Cloud.Messaging;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Cloud.CommandStack.CommandHandlers
{
    public class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand>
    {
        private readonly OrderDbContext _context;
        private readonly ILogger<PayOrderCommandHandler> _logger;

        public CancelOrderCommandHandler(OrderDbContext context, ILogger<PayOrderCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Handle(CancelOrderCommand command)
        {
            var order = _context.Orders
                .AsQueryable()
                .FirstOrDefault(x => x.Id == command.Id);

            if (order == null)
            {
                _logger.LogInformation($"order with id={command.Id} not found");

                return;
            }

            if (!order.TryCancel(out var errorMessage))
            {
                _logger.LogInformation($"can't cancel order with id={command.Id} because {errorMessage}");

                return;
            }

            _context.SaveChanges();
        }
    }
}
