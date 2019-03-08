using Cloud.CommandStack.Commands;
using Cloud.Core;
using Cloud.Infrastructure;
using Cloud.Messaging;

namespace Cloud.CommandStack.CommandHandlers
{
    public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand>
    {
        private readonly OrderDbContext _context;

        public CreateOrderCommandHandler(OrderDbContext context)
        {
            _context = context;
        }

        public void Handle(CreateOrderCommand command)
        {
            var order = new Order(command.Description, command.Amount);

            _context.Orders.Add(order);

            _context.SaveChanges();
        }
    }
}
