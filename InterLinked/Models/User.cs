using Microsoft.AspNetCore.Identity;

namespace InterLinked.Models
{
    public class User : IdentityUser
    {
        internal string? username;
        internal string? phoneNumber;
        //internal bool isCompany;
        internal IFormFile? profilePicture;

        internal UserType organizationType;

        public enum UserType
        {
            Individual,
            StudentOrganization,
            Company
        }

        //public bool isCompany { get; internal set; }
    }
}