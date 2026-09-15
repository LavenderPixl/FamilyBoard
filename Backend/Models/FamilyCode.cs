namespace Backend.Models;

public class FamilyCode
{
    public int Id { get; set; }
    public string Code { get; set; }
    public DateTime Expiration { get; set; }
    public int CreatedBy { get; set; } 
    public int FamilyId { get; set; }

    public bool IsValid()
    {
        if (Expiration < DateTime.UtcNow)
        {
            return false;
        }
        return true;
    }
}