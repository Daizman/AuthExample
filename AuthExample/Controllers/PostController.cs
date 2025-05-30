using AuthExample.Abstractions;
using AuthExample.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace AuthExample.Controllers;

public class PostController(IPostRepository postRepository) : BaseController
{
    [HttpPost]
    public ActionResult<int> CreatePost([FromBody] CreatePostDto dto)
    {
        var postId = postRepository.CreatePost(dto);
        return CreatedAtAction(nameof(GetPost), new { id = postId }, postId);
    }

    [HttpGet("{id:int}")]
    public ActionResult<PostDetailedVm> GetPost(int id, [FromQuery] Guid userId)
    {
        var post = postRepository.GetPost(id, userId);
        if (post is null)
        {
            return NotFound();
        }
        return Ok(post);
    }

    [HttpGet]
    public ActionResult<PostListVm> GetPosts()
    {
        var posts = postRepository.GetPosts();
        return Ok(posts);
    }

    [HttpDelete("{id:int}")]
    public ActionResult DeletePost(int id, [FromQuery] Guid userId)
    {
        postRepository.DeletePost(id, userId);
        return NoContent();
    }
}
