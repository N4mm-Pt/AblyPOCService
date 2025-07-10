using System.Text.Json;

public class MessageWrapperModel
{
    public string ClassId { get; set; } = "";
    public string From { get; set; } = "";
    public string To { get; set; } = "";
    public string Type { get; set; } = "";      // "chat", "cursor", etc.
    public JsonElement Content { get; set; }    // flexible message body
}