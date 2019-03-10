using Cloud.CommandStack.Commands;
using Cloud.Infrastructure;
using Cloud.Messaging;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Cloud.CommandStack.CommandHandlers
{
    public class PayOrderCommandHandler : ICommandHandler<PayOrderCommand>
    {
        private readonly OrderDbContext _context;
        private readonly ILogger<PayOrderCommandHandler> _logger;

        public PayOrderCommandHandler(OrderDbContext context, ILogger<PayOrderCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Handle(PayOrderCommand command)
        {
            var order = _context.Orders
                .AsQueryable()
                .FirstOrDefault(x => x.Id == command.Id);

            if (order == null)
            {
                _logger.LogInformation($"order with id={command.Id} not found");

                return;
            }

            if (!order.TryPay(command.Amount, out var errorMessage))
            {
                _logger.LogInformation($"can't pay for order with id={command.Id} because {errorMessage}");

                return;
            }

            _context.SaveChanges();
        }
    }
}
