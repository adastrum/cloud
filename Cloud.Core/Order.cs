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

        public bool TryPay(decimal amount, out string errorMessage)
        {
            if (amount < Amount)
            {
                errorMessage = "Insufficient payment amount";

                return false;
            }

            Status = OrderStatus.Paid;

            errorMessage = string.Empty;

            return true;
        }

        public bool TryCancel(out string errorMessage)
        {
            if (Status != OrderStatus.New)
            {
                errorMessage = $"Can't cancel order of status {Status}";

                return false;
            }

            Status = OrderStatus.Cancelled;

            errorMessage = string.Empty;

            return true;
        }
    }
}
