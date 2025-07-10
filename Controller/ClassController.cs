using AblyPOCService.Services;
using AblyPOCService.Models;
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

    [HttpPost("join")]
    public async Task<IActionResult> StudentJoin([FromBody] StudentJoinRequest request)
    {
        try
        {
            // Create a message to notify all users (especially teacher) about the new student
            var joinMessage = new MessageWrapperModel
            {
                ClassId = request.ClassId,
                From = request.StudentId,
                To = "all",
                Type = "student-join",
                Content = System.Text.Json.JsonSerializer.SerializeToElement(new {
                    studentId = request.StudentId,
                    studentName = request.StudentName,
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                })
            };

            // Use chat handler to send the join notification via Ably
            await chatHandler.HandleAsync(joinMessage);
            
            return Ok(new { message = "Student joined successfully", studentId = request.StudentId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("leave")]
    public async Task<IActionResult> StudentLeave([FromBody] StudentLeaveRequest request)
    {
        try
        {
            // Create a message to notify all users about the student leaving
            var leaveMessage = new MessageWrapperModel
            {
                ClassId = request.ClassId,
                From = request.StudentId,
                To = "all",
                Type = "student-leave",
                Content = System.Text.Json.JsonSerializer.SerializeToElement(new {
                    studentId = request.StudentId,
                    studentName = request.StudentName,
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                })
            };

            // Use chat handler to send the leave notification via Ably
            await chatHandler.HandleAsync(leaveMessage);
            
            return Ok(new { message = "Student left successfully", studentId = request.StudentId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}