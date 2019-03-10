using Cloud.Messaging;

namespace Cloud.CommandStack.Commands
{
    public class PayOrderCommand : ICommand
    {
        public PayOrderCommand(string id, decimal amount)
        {
            Id = id;
            Amount = amount;
        }

        public string Id { get; }

        public decimal Amount { get; }
    }
}
