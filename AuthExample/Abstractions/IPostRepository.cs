using AuthExample.Contracts;

namespace AuthExample.Abstractions;

public interface IPostRepository
{
    int CreatePost(CreatePostDto dto, Guid userId);
    PostDetailedVm? GetPost(int id, Guid userId);
    PostListVm GetPosts();
    void DeletePost(int id, Guid userId);
}
