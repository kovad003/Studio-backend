namespace Domain;

public class Photo
{
    public string Id { get; set; }
    public string Url { get; set; }

    // One-to-many
    public virtual Project Project { get; set; }
    public virtual User Uploader { get; set; }
}