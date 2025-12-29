using System.ComponentModel.DataAnnotations;

namespace VehicleExpenseAPI.Models;

public class FuelEntry
{
    public int Id { get; set; }
    
    [Range(0.01, double.MaxValue)]
    public decimal Liters { get; set; }
    
    [Range(0.01, double.MaxValue)]
    public decimal Cost { get; set; }
    
    [Range(0, int.MaxValue)]
    public int Odometer { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    // Foreign key to Vehicle
    [Required]
    public int VehicleId { get; set; }
    
    // Navigation property
    public Vehicle? Vehicle { get; set; }
}
