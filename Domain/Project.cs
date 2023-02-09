namespace Domain;

/// <summary>
/// AUTHOR: @Dan
/// </summary>
public class Project
{
    // Identifiers must be kept simple.
    // EF will create primary keys / columns based on 'em.
    // Access modifiers must be public so EF can access 'em.
    public Guid Id { get; set; }
    public string Title { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? CompletedOn { get; set; }
    public string Description { get; set; }

    // One-to-many rel.
    public User Owner { get; set; }
    public ICollection<Photo> Photos { get; set; } = new List<Photo>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
