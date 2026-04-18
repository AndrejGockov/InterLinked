using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;

namespace InterLinked.Models;

public class Client:BaseEntity
{
    

    public string UserId { get; set; } = null!;
    public InterlinkedAppUser User { get; set; } = null!;

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ResumeUrl { get; set; }

    public ICollection<Application> Applications { get; set; } = new List<Application>();
}