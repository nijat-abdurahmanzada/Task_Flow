namespace TaskFlow.API.Models
{
    public class Project
    {
        public int Id { get; set; }
        public String Name { get; set; } = String.Empty;
        public String? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int OwnerId {get; set; }
        public User Owner { get; set; } = null!;

    }
}
