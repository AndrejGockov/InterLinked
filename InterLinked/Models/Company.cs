namespace InterLinked.Models;

public class Company:BaseEntity
{
    

    public string UserId { get; set; } = null!;
    public InterlinkedAppUser User { get; set; } = null!;

    public string? CompanyName { get; set; }

    public ICollection<Post> Posts { get; set; } = new List<Post>();
}