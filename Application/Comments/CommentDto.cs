namespace Application.Comments;

public class CommentDto
{
    public int Id { get; set; }
    public string Body { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // One-to-one Rel.:
    public string FirstName { get; set; }
}