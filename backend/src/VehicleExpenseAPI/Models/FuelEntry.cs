using System.ComponentModel.DataAnnotations;

namespace VehicleExpenseAPI.Models;

public class FuelEntry
{
    public int Id { get; set; }
    
    public EnergyType EnergyType { get; set; }
    
    // Amount of energy added (liters for fuel, kWh for electricity)
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
    
    [Range(0.01, double.MaxValue)]
    public decimal Cost { get; set; }
    
    [Range(0, int.MaxValue)]
    public int Odometer { get; set; }
    
    [Required]
    public DateOnly Date { get; set; }
    
    // Foreign key to Vehicle
    [Required]
    public int VehicleId { get; set; }
    
    // Navigation property
    public Vehicle? Vehicle { get; set; }
    
    // Foreign key to Expense (nullable - FuelEntry can exist without an Expense)
    public int? ExpenseId { get; set; }
    
    // Navigation property to the linked Fuel Expense
    public Expense? Expense { get; set; }
}
