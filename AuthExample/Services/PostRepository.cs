using AuthExample.Abstractions;
using AuthExample.Contracts;
using AuthExample.Database;
using Microsoft.EntityFrameworkCore;

namespace AuthExample.Services;

public class PostRepository(AppDbContext dbContext) : IPostRepository
{
    public int CreatePost(CreatePostDto dto)
    {
        var post = dto.ToPost();
        dbContext.Posts.Add(post);

        dbContext.SaveChanges();

        return post.Id;
    }

    public void DeletePost(int id, Guid userId)
    {
        var post = dbContext.Posts.FirstOrDefault(p => p.Id == id && p.UserId == userId);
        if (post is null)
            return;

        dbContext.Posts.Remove(post);
        dbContext.SaveChanges();
    }

    public PostDetailedVm? GetPost(int id, Guid userId)
    {
        var post = dbContext.Posts
            .Include(p => p.User)
            .FirstOrDefault(p => p.Id == id && p.UserId == userId);

        if (post is null)
            return null;

        return post.ToDetailedVm();
    }

    public PostListVm GetPosts()
    {
        var posts = dbContext.Posts
            .Include(p => p.User)
            .Select(p => p.ToVm())
            .ToList();

        return new PostListVm(posts);
    }
}
