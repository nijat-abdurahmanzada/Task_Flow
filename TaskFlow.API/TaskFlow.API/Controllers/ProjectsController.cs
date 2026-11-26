using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.API.Data;
using TaskFlow.API.DTOs;
using TaskFlow.API.Services.Interfaces;
namespace TaskFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public int? GetCurrentUserId()
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdValue, out var userId))
                return null;

            return userId;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject(CreateProjectDto dto)
        {
            var userId = GetCurrentUserId();

            if (userId == null) return Unauthorized();

            var result = await _projectService.CreateAsync(dto, userId.Value);

            return StatusCode(201, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var userId = GetCurrentUserId();

            if (userId == null) return Unauthorized();

            var result = await _projectService.GetProjectsAsync(userId.Value);

            return Ok(result);
        }

        [HttpGet("{projectId:int}")]
        public async Task<IActionResult> GetProject(int projectId)
        {
            var userId = GetCurrentUserId();

            if(userId == null) return Unauthorized();

            var result = await _projectService.GetProjectAsync(projectId, userId.Value);

            if(result == null) return NotFound();

            return Ok(result);
        }
    }
}
