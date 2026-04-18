using System.ComponentModel.DataAnnotations;

namespace InterLinked.Models
{
    public class Application
    {
        [Key]
        public int ApplicationId { get; set; }

        // FK (this is what you actually save)
        public string UserId { get; set; } = null!;
        public InterlinkedAppUser? User { get; set; }

        public int PostId { get; set; }
        public Post? Post { get; set; }

        public DateTime AppliedAt { get; set; } = DateTime.Now;
    }
}