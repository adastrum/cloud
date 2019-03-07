using Xunit;

namespace Cloud.Core.Tests
{
    public class OrderTests
    {
        [Fact]
        public void Should_create_order()
        {
            const string description = "test";
            const decimal amount = 42m;
            var order = new Order(description, amount);

            Assert.Equal(description, order.Description);
            Assert.Equal(amount, order.Amount);
            Assert.Equal(OrderStatus.New, order.Status);
        }

        [Theory]
        [InlineData(42)]
        [InlineData(43)]
        public void Should_pay_order(decimal payAmount)
        {
            const string description = "test";
            const decimal amount = 42m;
            var order = new Order(description, amount);

            order.Pay(payAmount);

            Assert.Equal(OrderStatus.Paid, order.Status);
        }

        [Fact]
        public void Should_cancel_order()
        {
            const string description = "test";
            const decimal amount = 42m;
            var order = new Order(description, amount);

            order.Cancel();

            Assert.Equal(OrderStatus.Cancelled, order.Status);
        }
    }
}
