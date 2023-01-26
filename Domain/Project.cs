using System.Runtime.InteropServices;

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
    public string Image { get; set; }
    public string Attachment { get; set; }
    public string Client { get; set; }

    // One-to-many rel.
    public User Owner { get; set; }
}
