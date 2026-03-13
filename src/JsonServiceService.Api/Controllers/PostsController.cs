using JsonServiceService.Core.Interfaces;
using JsonServiceService.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace JsonServiceService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IJsonPlaceholderService _jsonPlaceholderService;
    private readonly ILogger<PostsController> _logger;

    public PostsController(IJsonPlaceholderService jsonPlaceholderService, ILogger<PostsController> logger)
    {
        _jsonPlaceholderService = jsonPlaceholderService;
        _logger = logger;
    }

    /// <summary>
    /// Get all posts from JSONPlaceholder API
    /// </summary>
    /// <returns>List of posts</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<PostDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PostDto>>> GetAllPosts()
    {
        try
        {
            _logger.LogInformation("Fetching all posts from JSONPlaceholder API");
            var posts = await _jsonPlaceholderService.GetAllPostsAsync();
            
            var postDtos = posts.Select(p => new PostDto
            {
                Id = p.Id,
                UserId = p.UserId,
                Title = p.Title,
                Body = p.Body
            }).ToList();
            
            return Ok(postDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching posts");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error fetching posts" });
        }
    }

    /// <summary>
    /// Get a specific post by ID from JSONPlaceholder API
    /// </summary>
    /// <param name="id">Post ID</param>
    /// <returns>Post details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostDto>> GetPostById(int id)
    {
        try
        {
            _logger.LogInformation("Fetching post with ID {PostId}", id);
            var post = await _jsonPlaceholderService.GetPostByIdAsync(id);
            
            if (post == null)
            {
                _logger.LogWarning("Post with ID {PostId} not found", id);
                return NotFound(new { message = $"Post with ID {id} not found" });
            }
            
            var postDto = new PostDto
            {
                Id = post.Id,
                UserId = post.UserId,
                Title = post.Title,
                Body = post.Body
            };
            
            return Ok(postDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching post with ID {PostId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error fetching post" });
        }
    }
}