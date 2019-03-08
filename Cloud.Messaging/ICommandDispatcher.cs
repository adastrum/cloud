namespace Cloud.Messaging
{
    public interface ICommandDispatcher
    {
        void Publish<TCommand>(TCommand command)
            where TCommand : ICommand;
    }
}
