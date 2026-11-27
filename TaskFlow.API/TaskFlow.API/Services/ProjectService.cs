using Mapster;
using Microsoft.EntityFrameworkCore;
using TaskFlow.API.Data;
using TaskFlow.API.DTOs;
using TaskFlow.API.Models;
using TaskFlow.API.Services.Interfaces;

namespace TaskFlow.API.Services
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _context;
        public ProjectService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ProjectResponseDto> CreateAsync(CreateProjectDto dto, int userId)
        {
            var owner = await _context.Users.FindAsync(userId);

            if (owner == null) throw new Exception("User not found");


            var project = new Project { Name = dto.Name, Description = dto.Description, OwnerId = userId };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return new ProjectResponseDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                Owner = new ProjectOwnerDto
                {
                    Id = owner.Id,
                    FullName = owner.FullName,
                    Email = owner.Email,
                }
            };
        }

        public async Task<List<ProjectResponseDto>> GetProjectsAsync(int userId)
        {
            var projects = await _context.Projects
                .Include(p => p.Owner)
                .Where(p => p.OwnerId == userId)
                .ToListAsync();

            return projects.Adapt<List<ProjectResponseDto>>();
        }

        public async Task<ProjectResponseDto?> GetProjectAsync(int projectId, int userId)
        {
            var project = await _context.Projects
                .Where(p => p.OwnerId == userId)
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null) return null;

            return project.Adapt<ProjectResponseDto>();
        }

        public async Task<ProjectResponseDto?> UpdateProjectAsync(UpdateProjectDto dto, int projectId, int userId)
        {
            var project = await _context.Projects
                .Where(p => p.OwnerId == userId)
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null) return null;

            project.Name = dto.Name;
            project.Description = dto.Description;

            await _context.SaveChangesAsync();

            return project.Adapt<ProjectResponseDto>();
        }

        public async Task<bool> DeleteProjectAsync(int projectId, int userId)
        {
            var project = await _context.Projects
                .Where(p => p.OwnerId == userId)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null) return false;

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
