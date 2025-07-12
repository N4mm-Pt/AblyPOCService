using AblyPOCService.Services;
using AblyPOCService.Models;
using Microsoft.AspNetCore.Mvc;

namespace AblyPOCService.Controller;

[ApiController]
[Route("api/class")]
public class ClassController(ChatMessageHandler chatHandler) : ControllerBase
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

    [HttpPost("toggle-cursor")]
    public async Task<IActionResult> ToggleCursor([FromBody] ToggleCursorRequest request)
    {
        try
        {
            // Create a message to notify all students about cursor stream toggle
            var toggleMessage = new MessageWrapperModel
            {
                ClassId = request.ClassId,
                From = request.TeacherId,
                To = "all",
                Type = "cursor-toggle",
                Content = System.Text.Json.JsonSerializer.SerializeToElement(new {
                    enabled = request.Enabled,
                    teacherId = request.TeacherId,
                    teacherName = request.TeacherName,
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                })
            };

            // Use chat handler to send the toggle notification via Ably
            await chatHandler.HandleAsync(toggleMessage);
            
            return Ok(new { 
                message = request.Enabled ? "Cursor stream enabled" : "Cursor stream disabled", 
                enabled = request.Enabled 
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
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