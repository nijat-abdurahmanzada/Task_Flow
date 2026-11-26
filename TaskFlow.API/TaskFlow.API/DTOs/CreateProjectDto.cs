using System.ComponentModel.DataAnnotations;

namespace TaskFlow.API.DTOs
{
    public class CreateProjectDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
