using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterLinked.Models
{
    public class Post
    {

        [Key]
        public int PostId { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateTime PostedAt { get; set; } = DateTime.Now;
        public DateTime? ValidTo { get; set; }
        public bool IsActive() {
            return ValidTo == null || ValidTo > DateTime.Now;
        }

        //[NotMapped]
        //public IFormFile? Thumbnail { get; set; }
        //public string? ThumbnailPicturePath { get; set; }

        public string? WebsiteLink { get; set; }

        public string? UserId { get; set; } = default!;
        public InterlinkedAppUser? User { get; set; } = default!;

        public ICollection<Application>? Applications { get; set; }


    }
}
