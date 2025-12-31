using VehicleExpenseAPI.Models;

namespace VehicleExpenseAPI.DTOs.Fuel;

public class UpdateFuelEntryDto
{
    public EnergyType EnergyType { get; set; }
    public decimal Amount { get; set; }  // Liters or kWh depending on EnergyType
    public decimal Cost { get; set; }
    public int Odometer { get; set; }
    public DateOnly Date { get; set; }
    public int VehicleId { get; set; }
}
