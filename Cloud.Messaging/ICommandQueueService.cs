using System.Threading.Tasks;

namespace Cloud.Messaging
{
    public interface ICommandQueueService
    {
        Task<CommandQueueServiceResponse> TryAddMessageAsync(string message);

        Task<CommandQueueServiceResponse> TryDeleteMessageAsync();
    }

    public class CommandQueueServiceResponse
    {
        private CommandQueueServiceResponse(bool isSuccess, string content, string errorMessage)
        {
            IsSuccess = isSuccess;
            Content = content;
            ErrorMessage = errorMessage;
        }

        public static CommandQueueServiceResponse Success(string content)
        {
            return new CommandQueueServiceResponse(true, content, string.Empty);
        }

        public static CommandQueueServiceResponse Failure(string errorMessage)
        {
            return new CommandQueueServiceResponse(false, string.Empty, errorMessage);
        }

        public bool IsSuccess { get; }

        public string Content { get; }

        public string ErrorMessage { get; }
    }
}