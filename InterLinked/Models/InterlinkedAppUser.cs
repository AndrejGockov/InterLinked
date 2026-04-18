using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

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

        //public string? PhoneNum { get; set; }

        //public required string Username { get; set; }
        public required UserType organizationType { get; set; }

        [NotMapped]
        public IFormFile? profilePicture { get; set; }

        public string? ProfilePicturePath { get; set; }

        //internal bool isCompany;
        //public bool isCompany { get; internal set; }
    }
}