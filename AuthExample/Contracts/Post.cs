using AuthExample.Models;

namespace AuthExample.Contracts;

public record CreatePostDto(string Content)
{
    public Post ToPost(Guid userId) => new()
    {
        UserId = userId,
        Content = Content,
        PublicationDateTime = DateTime.UtcNow,
    };
}

public record PostDetailedVm(int Id, string Content, DateTime PublicationDateTime, string UserName);

public record PostVm(int Id, string Content, string UserName);
public record PostListVm(IReadOnlyList<PostVm> Posts);
