using AuthExample.Abstractions;
using AuthExample.Contracts;
using AuthExample.Database;
using Microsoft.EntityFrameworkCore;

namespace AuthExample.Services;

public class PostRepository(AppDbContext dbContext) : IPostRepository
{
    public int CreatePostAsync(CreatePostDto dto, Guid userId)
    {
        var post = dto.ToPost(userId);
        dbContext.Posts.Add(post);

        dbContext.SaveChanges();

        return post.Id;
    }

    public void DeletePostAsync(int id, Guid userId)
    {
        var post = dbContext.Posts.FirstOrDefault(p => p.Id == id && p.UserId == userId);
        if (post is null)
        {
            return;
        }

        dbContext.Posts.Remove(post);
        dbContext.SaveChanges();
    }

    public PostDetailedVm? GetPostAsync(int id, Guid userId)
    {
        var post = dbContext.Posts
            .Include(p => p.User)
            .FirstOrDefault(p => p.Id == id && p.UserId == userId);

        if (post is null)
        {
            return null;
        }

        return post.ToDetailedVm();
    }

    public PostListVm GetPostsAsync()
    {
        var posts = dbContext.Posts
            .Include(p => p.User)
            .Select(p => p.ToVm())
            .ToList();

        return new PostListVm(posts);
    }
}
