using Cloud.Messaging;

namespace Cloud.CommandStack.Commands
{
    public class CreateOrderCommand : ICommand
    {
        public CreateOrderCommand(string description, decimal amount)
        {
            Description = description;
            Amount = amount;
        }

        public string Description { get; }

        public decimal Amount { get; }
    }
}
