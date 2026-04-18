namespace InterLinked.Models;

public class Post : BaseEntity
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime PostedAt { get; set; } = DateTime.Now;
    public DateTime? ValidTo { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ThumbnailUrl { get; set; }
    public string? ApplyExternalLink { get; set; }  // redirects outside the site
    public bool ApplyInternalLink { get; set; } = false;
    
    
    // Foreign Key
    public Guid CompanyId { get; set; }

    // Navigation
    public virtual Company Company { get; set; } = null!;
    public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}
public class Tag :BaseEntity
{
    public string? Name { get; set; }

    public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
}

public class PostTag : BaseEntity
{
    public Guid TagId { get; set; }

    public Post Post { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}