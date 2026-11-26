using TaskFlow.API.DTOs;

namespace TaskFlow.API.Services.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectResponseDto> CreateAsync(CreateProjectDto dto, int userId);
        Task<List<ProjectResponseDto>> GetProjectsAsync(int userId);
        Task<ProjectResponseDto?> GetProjectAsync(int projectId,int userId);
    }
}
