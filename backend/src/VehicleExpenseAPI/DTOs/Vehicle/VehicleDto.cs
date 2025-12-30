namespace VehicleExpenseAPI.DTOs.Vehicle;

public class VehicleDto
{
    public int Id { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal PurchasePrice { get; set; }
    public DateOnly OwnershipStart { get; set; }
    public DateOnly? OwnershipEnd { get; set; }
    public string UserId { get; set; } = string.Empty;
}
