namespace Backend.Models;

public class Task
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Reward { get; set; }
    public bool Completed { get; set; }
    // Get serialized as ISO 8601 (YYYY-MM-DD)
    public DateOnly ExpireDate { get; set; }
    public int FamilyId { get; set; }
    public int? UserId { get; set; }
    public string? Username { get; set; }
    public int? GoalId { get; set; }
}