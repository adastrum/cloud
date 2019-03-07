using System;

namespace Cloud.Core
{
    public class Order
    {
        public string Id { get; protected set; }
        public string Description { get; protected set; }
        public decimal Amount { get; protected set; }
        public OrderStatus Status { get; protected set; }

        protected Order()
        {
            Status = OrderStatus.New;
        }

        public Order(string description, decimal amount) : this()
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException(nameof(description));
            }

            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            Description = description;
            Amount = amount;
        }

        public void Pay(decimal amount)
        {
            if (amount < Amount)
            {
                throw new ArgumentException("insufficient amount", nameof(Amount));
            }

            Status = OrderStatus.Paid;
        }

        public void Cancel()
        {
            Status = OrderStatus.Cancelled;
        }
    }
}
