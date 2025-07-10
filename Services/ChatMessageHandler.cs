using System.Text.Json;
using AblyPOCService.Models;
using IO.Ably;

namespace AblyPOCService.Services;

public class ChatMessageHandler(AblyRest ably) : IMessageHandler
{
    public async Task HandleAsync(MessageWrapperModel wrapper)
    {
        object content;
        
        // For join/leave messages, convert JsonElement to object for proper serialization
        if (wrapper.Type is "student-join" or "student-leave")
        {
            // Deserialize the JsonElement to a dynamic object to preserve the structure
            content = JsonSerializer.Deserialize<object>(wrapper.Content.GetRawText());
        }
        else
        {
            // For regular chat messages, deserialize as ChatMessage
            content = wrapper.Content.Deserialize<ChatMessage>();
        }

        var payload = new
        {
            from = wrapper.From,
            to = wrapper.To,
            type = wrapper.Type, // Use the original message type instead of hardcoding "chat"
            content = content
        };

        var channel = ably.Channels.Get($"class-{wrapper.ClassId}");
        await channel.PublishAsync("chat", payload);
    }
}