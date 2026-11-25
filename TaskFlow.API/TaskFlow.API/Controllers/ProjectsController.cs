using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Data;
using TaskFlow.API.DTOs;
using TaskFlow.API.Models;
namespace TaskFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public ProjectsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CreateProject(CreatingProjectDto dto)
        {
            //var project = new Project
            //{
            //    Name = dto.Name,
            //    Description = dto.Description
            //};

            return Ok();
        }
    }
}
