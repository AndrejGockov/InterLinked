using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace InterLinked.Models
{
    public class InterlinkedAppUser : IdentityUser
    {
        public enum UserType
        {
            Individual,
            StudentOrganization,
            Company
        }

        public required UserType organizationType { get; set; }

        [NotMapped]
        public IFormFile? profilePicture { get; set; }
        public string? ProfilePicturePath { get; set; }


        //add everything for each account type

        public ICollection<Post>? MyPosts { get; set; }
        //public ICollection<Application>? MyApplications { get; set; }

    }
}