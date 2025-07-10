using AblyPOCService.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace AblyPOCService.Controller;

[ApiController]
[Route("api/class")]
public class ClassController(ChatMessageHandler chatHandler, CursorMessageHandler cursorHandler) : ControllerBase
{
    [HttpPost("chat")]
    public async Task<IActionResult> PostChat([FromBody] MessageWrapperModel msg)
    {
        try
        {
            await chatHandler.HandleAsync(msg);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("cursor-stream")]
    public async Task<IActionResult> CursorStream([FromQuery] string classId, [FromQuery] string userId)
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            return BadRequest("WebSocket connection expected.");
        }

        using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                break;
            }

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var cursorContent = JsonDocument.Parse(json).RootElement;
                var msg = new MessageWrapperModel
                {
                    ClassId = classId,
                    From = userId,
                    To = "all",
                    Type = "cursor",
                    Content = cursorContent
                };
                await cursorHandler.HandleAsync(msg);
            }
        }
        return new EmptyResult();
    }
}