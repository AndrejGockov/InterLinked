using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterLinked.Models
{
    public class Post
    {
        public enum WorkplaceTypes{
            InPerson,
            Remote,
            Hybrid
        }
        public enum JobTypes{
            FullTime,
            PartTime,
            Internship,
            Contract,
            Freelance,
            Volunteer
        }
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
        public float? Salary { get; set; }
        public string? Location { get; set; }
        public string? LocationLink { get; set; }
        public WorkplaceTypes? WorkplaceType { get; set; }
        public JobTypes? jobType;

        public string? UserId { get; set; } = default!;
        public InterlinkedAppUser? User { get; set; } = default!;
        public ICollection<Application>? Applications { get; set; }

    }
}
