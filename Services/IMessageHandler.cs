namespace AblyPOCService.Services;

public interface IMessageHandler
{
    Task HandleAsync(MessageWrapperModel message);
}