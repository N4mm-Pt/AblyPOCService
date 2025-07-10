using System.Text.Json;
using AblyPOCService.Models;
using IO.Ably;

namespace AblyPOCService.Services;

public class ChatMessageHandler(AblyRest ably) : IMessageHandler
{
    public async Task HandleAsync(MessageWrapperModel wrapper)
    {
        var chat = wrapper.Content.Deserialize<ChatMessage>();

        var payload = new
        {
            from = wrapper.From,
            to = wrapper.To,
            type = "chat",
            content = chat
        };

        var channel = ably.Channels.Get($"class-{wrapper.ClassId}");
        await channel.PublishAsync("chat", payload);
    }
}