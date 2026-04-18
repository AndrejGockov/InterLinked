using InterLinked.Enums;

namespace InterLinked.Models;

public class Application : BaseEntity
{
    public string ContactName { get; set; } = null!;
    public string ContactEmail { get; set; } = null!;
    public string? ContactPhone { get; set; }
    public string? ContactCity { get; set; }
    public string? ContactAddress { get; set; }
    public string? CoverLetter { get; set; }
    public string? CVFileName { get; set; }
    public string? CVFilePath { get; set; }
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Foreign Keys
    public Guid ClientId { get; set; }
    public Guid PostId { get; set; }

    // Navigation
    public Client Client { get; set; } = null!;
    public Post Post { get; set; } = null!;
}