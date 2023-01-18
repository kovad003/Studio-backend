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
    public DateTime CreatedOn { get; set; }
    public string Description { get; set; }
    public string Client { get; set; }
    public string Image { get; set; }
}
