namespace Backend.Models;

public class Goal
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Progress  { get; set; }
    public int Cost { get; set; }
    public int UserId { get; set; }
}