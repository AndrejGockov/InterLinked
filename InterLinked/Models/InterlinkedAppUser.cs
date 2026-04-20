using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace InterLinked.Models
{
    public class InterlinkedAppUser : IdentityUser
    {
        public enum UserType
        {
            Personal,
            Company
        }
        public string? CompanyDescription { get; set; } 
        public required UserType organizationType { get; set; }

        [NotMapped]
        public IFormFile? profilePicture { get; set; }
        public string? ProfilePicturePath { get; set; }


        //add everything for each account type

        public ICollection<Post>? MyPosts { get; set; }
        public ICollection<Application>? MyApplications { get; set; }


        public string? InstagramUrl { get; set; }
        public string? FacebookUrl { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? TwitterUrl { get; set; }

        public string? CvPath { get; set; }
    }
}