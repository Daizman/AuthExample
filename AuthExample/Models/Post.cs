namespace AuthExample.Models;

public partial class Post
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public DateTime PublicationDateTime { get; set; }

    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
}
