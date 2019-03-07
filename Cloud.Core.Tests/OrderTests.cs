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
        [InlineData(42.01)]
        [InlineData(43)]
        public void Should_pay_order(decimal payAmount)
        {
            const string description = "test";
            const decimal amount = 42m;
            var order = new Order(description, amount);

            var payResult = order.TryPay(payAmount, out var errorMessage);

            Assert.Equal(OrderStatus.Paid, order.Status);
            Assert.True(payResult);
            Assert.Equal(string.Empty, errorMessage);
        }

        [Theory]
        [InlineData(41.99)]
        [InlineData(41)]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_not_pay_order_if_insufficient_payment_amount(decimal payAmount)
        {
            const string description = "test";
            const decimal amount = 42m;
            var order = new Order(description, amount);

            var payResult = order.TryPay(payAmount, out var errorMessage);

            Assert.Equal(OrderStatus.New, order.Status);
            Assert.False(payResult);
            Assert.Equal("Insufficient payment amount", errorMessage);
        }

        [Fact]
        public void Should_cancel_order()
        {
            const string description = "test";
            const decimal amount = 42m;
            var order = new Order(description, amount);

            var cancelResult = order.TryCancel(out var errorMessage);

            Assert.Equal(OrderStatus.Cancelled, order.Status);
            Assert.True(cancelResult);
            Assert.Equal(string.Empty, errorMessage);
        }

        [Fact]
        public void Should_not_cancel_paid_order()
        {
            const string description = "test";
            const decimal amount = 42m;
            var order = new Order(description, amount);

            order.TryPay(amount, out _);

            var cancelResult = order.TryCancel(out var errorMessage);

            Assert.Equal(OrderStatus.Paid, order.Status);
            Assert.False(cancelResult);
            Assert.Equal("Can't cancel order of status Paid", errorMessage);
        }

        [Fact]
        public void Should_not_cancel_cancelled_order()
        {
            const string description = "test";
            const decimal amount = 42m;
            var order = new Order(description, amount);

            order.TryCancel(out _);

            var cancelResult = order.TryCancel(out var errorMessage);

            Assert.Equal(OrderStatus.Cancelled, order.Status);
            Assert.False(cancelResult);
            Assert.Equal("Can't cancel order of status Cancelled", errorMessage);
        }
    }
}
