namespace Cloud.Messaging
{
    public interface ICommandDispatcher
    {
        void Publish(ICommand command);
    }
}
