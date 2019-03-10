using Cloud.Messaging;

namespace Cloud.CommandStack.Commands
{
    public class CancelOrderCommand : ICommand
    {
        public CancelOrderCommand(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}
