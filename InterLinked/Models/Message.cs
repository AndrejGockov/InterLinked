namespace InterLinked.Models;

public class Message:BaseEntity
{
    
    public string? MessageText { get; set; }
    public DateTime DateTimeSent { get; set; } = DateTime.Now;

    // Foreign Keys
    public string SenderId { get; set; } = null!;
    public string ReceiverId { get; set; } = null!;

    // Navigation
    public InterlinkedAppUser Sender { get; set; } = null!;
    public InterlinkedAppUser Receiver { get; set; } = null!;
}