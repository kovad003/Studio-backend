namespace Domain;

public class Comment
{
    public int Id { get; set; }
    public string Body { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // One-to-one Rel.:
    public User Author { get; set; }
    public Project Project { get; set; }
}