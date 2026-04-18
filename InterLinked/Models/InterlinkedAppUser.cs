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

        public required UserType organizationType { get; set; }

        [NotMapped]
        public IFormFile? profilePicture { get; set; }
        public string? ProfilePicturePath { get; set; }


        //add everything for each account type



    }
}