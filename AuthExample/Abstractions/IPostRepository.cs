using AuthExample.Contracts;

namespace AuthExample.Abstractions;

public interface IPostRepository
{
    Task<int> CreatePostAsync(CreatePostDto dto, Guid userId);
    Task<PostDetailedVm?> GetPostAsync(int id, Guid userId);
    Task<PostListVm> GetPostsAsync();
    Task DeletePostAsync(int id, Guid userId);
}
