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

        public void Publish<TCommand>(TCommand command)
            where TCommand : ICommand
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = (ICommandHandler<TCommand>)scope.ServiceProvider.GetService(typeof(ICommandHandler<TCommand>));
                handler.Handle(command);
            }
        }
    }
}
