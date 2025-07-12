namespace AblyPOCService.Models;

public class ToggleCursorRequest
{
    public string ClassId { get; set; } = "";
    public string TeacherId { get; set; } = "";
    public string TeacherName { get; set; } = "";
    public bool Enabled { get; set; }
}
