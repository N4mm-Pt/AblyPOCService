using System.Text.Json;
using AblyPOCService.Models;
using IO.Ably;

namespace AblyPOCService.Services;

public class CursorMessageHandler(AblyRest ably) : IMessageHandler
{
    public async Task HandleAsync(MessageWrapperModel wrapper)
    {
        var cursor = wrapper.Content.Deserialize<CursorMessage>();

        var payload = new
        {
            from = wrapper.From,
            to = wrapper.To,
            type = "cursor",
            content = cursor
        };

        var channel = ably.Channels.Get($"class-{wrapper.ClassId}");
        await channel.PublishAsync("cursor", payload);
    }
} 