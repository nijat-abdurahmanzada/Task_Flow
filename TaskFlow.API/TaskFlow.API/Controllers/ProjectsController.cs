using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

        private int? GetCurrentUserId()
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

            return CreatedAtAction(
                nameof(GetProject),
                new { projectId = result.Id },
                result
            );
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var userId = GetCurrentUserId();

            if (userId == null) return Unauthorized();

            var result = await _projectService.GetProjectsAsync(userId.Value);

            return Ok(new
            {
                projects = result,
                totalCount = result.Count
            });
        }

        [HttpGet("{projectId:int}")]
        public async Task<IActionResult> GetProject(int projectId)
        {
            var userId = GetCurrentUserId();

            if (userId == null) return Unauthorized();

            var result = await _projectService.GetProjectAsync(projectId, userId.Value);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpPut("{projectId:int}")]
        public async Task<IActionResult> UpdateProject(UpdateProjectDto dto, int projectId)
        {
            var userId = GetCurrentUserId();

            if (userId == null) return Unauthorized();

            var result = await _projectService.UpdateProjectAsync(dto, projectId, userId.Value);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpDelete("{projectId:int}")]
        public async Task<IActionResult> DeleteProject(int projectId)
        {
            var userId = GetCurrentUserId();

            if (userId == null) return Unauthorized();

            var result = await _projectService.DeleteProjectAsync(projectId, userId.Value);

            if (!result) return NotFound();

            return NoContent();
        }
    }
}
