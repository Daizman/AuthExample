using AuthExample.Abstractions;
using AuthExample.Contracts;
using AuthExample.Database;
using Microsoft.EntityFrameworkCore;

namespace AuthExample.Services;

public class PostRepository(AppDbContext dbContext) : IPostRepository
{
    public async Task<int> CreatePostAsync(CreatePostDto dto, Guid userId)
    {
        var post = dto.ToPost(userId);
        await dbContext.Posts.AddAsync(post);

        await dbContext.SaveChangesAsync();

        return post.Id;
    }

    public async Task DeletePostAsync(int id, Guid userId)
    {
        var post = await dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        if (post is null)
        {
            return;
        }

        dbContext.Posts.Remove(post);
        await dbContext.SaveChangesAsync();
    }

    public async Task<PostDetailedVm?> GetPostAsync(int id, Guid userId)
    {
        var post = await dbContext.Posts
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        if (post is null)
        {
            return null;
        }

        return post.ToDetailedVm();
    }

    public async Task<PostListVm> GetPostsAsync()
    {
        var posts = await dbContext.Posts
            .Include(p => p.User)
            .Select(p => p.ToVm())
            .ToListAsync();

        return new PostListVm(posts);
    }
}
