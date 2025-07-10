using System.Text.Json.Serialization;

namespace AblyPOCService.Models;

public class ChatMessage
{
    public ChatMessage() { }

    [JsonPropertyName("text")]
    public string Text { get; set; } = "";

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    [JsonPropertyName("messageType")]
    public string MessageType { get; set; } = "";
}