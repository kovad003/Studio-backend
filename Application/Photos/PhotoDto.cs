namespace Application.Photos;

public class PhotoDto
{
    public string Id { get; set; }
    public string Url { get; set; }

    // One-to-many Props
    // Project table
    public Guid ProjectId { get; set; }
    public string ProjectOwner { get; set; }
    // User table
    public string UploaderId { get; set; }
    public string UploaderName { get; set; }
    public string UploaderEmail { get; set; }
}