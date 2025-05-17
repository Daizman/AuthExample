using AuthExample.Models;

namespace AuthExample.Contracts;

public record CreatePostDto(Guid UserId, string Content)
{
    public Post ToPost() => new()
    {
        UserId = UserId,
        Content = Content,
        PublicationDateTime = DateTime.UtcNow,
    };
}

public record PostDetailedVm(int Id, string Content, DateTime PublicationDateTime, string UserName);

public record PostVm(int Id, string Content, string UserName);
public record PostListVm(IReadOnlyList<PostVm> Posts);
