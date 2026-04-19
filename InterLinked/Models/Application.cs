using System.ComponentModel.DataAnnotations;

namespace InterLinked.Models
{
    public class Application
    {
        [Key]
        public int ApplicationId { get; set; }

        // FK (user who applied)
        public string UserId { get; set; } = null!;
        public InterlinkedAppUser? User { get; set; }

        // FK (post applied to)
        public int PostId { get; set; }
        public Post? Post { get; set; }

        public DateTime AppliedAt { get; set; } = DateTime.Now;
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
    }

    public enum ApplicationStatus
    {
        Pending,
        Approved,
        Rejected
    }

}