using Cloud.Messaging;
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
            var handler = (ICommandHandler<TCommand>)_serviceProvider.GetService(typeof(ICommandHandler<TCommand>));
            handler.Handle(command);
        }
    }
}
