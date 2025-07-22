using System.ComponentModel.DataAnnotations;
using UseNotesApplication.Models;

public class NoteVersion
{
    public int Id { get; set; }

    public int NoteId { get; set; }
    public Notes Notes { get; set; }

    [MaxLength(200)]
    public string? Title { get; set; }

    public string? Description { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
