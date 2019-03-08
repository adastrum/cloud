using Cloud.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Cloud.Infrastructure
{
    public class InMemoryCommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public InMemoryCommandDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Publish(ICommand command)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var commandType = command.GetType();
                var serviceType = typeof(ICommandHandler<>).MakeGenericType(commandType);
                var service = scope.ServiceProvider.GetService(serviceType);
                var handleMethod = service.GetType().GetMethod("Handle");
                handleMethod.Invoke(service, new object[] { command });
            }
        }
    }
}
