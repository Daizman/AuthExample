namespace AuthExample.Models;

public class User
{
    public Guid Id { get; set; }
    public required string UserName { get; set; }
    public required byte[] Password { get; set; }

    public ICollection<Post> Posts { get; set; } = [];
}
