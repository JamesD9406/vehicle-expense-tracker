using VehicleExpenseAPI.Models;

namespace VehicleExpenseAPI.DTOs.Fuel;

public class FuelEfficiencyDto
{
    public int VehicleId { get; set; }
    public string VehicleMake { get; set; } = string.Empty;
    public string VehicleModel { get; set; } = string.Empty;
    public VehicleType VehicleType { get; set; }
    
    // Gasoline/Diesel statistics
    public decimal TotalFuelLiters { get; set; }
    public decimal TotalFuelCost { get; set; }
    public decimal AverageLitersPer100Km { get; set; }
    public decimal AverageFuelCostPerKm { get; set; }
    
    // Electricity statistics
    public decimal TotalElectricityKwh { get; set; }
    public decimal TotalElectricityCost { get; set; }
    public decimal AverageKwhPer100Km { get; set; }
    public decimal AverageElectricityCostPerKm { get; set; }
    
    // Combined statistics for hybrids
    public decimal TotalEnergyCost { get; set; }
    public decimal AverageCostPerKm { get; set; }
    public int TotalKilometers { get; set; }
    
    // Date range
    public DateOnly? FirstEntryDate { get; set; }
    public DateOnly? LastEntryDate { get; set; }
    public int NumberOfFuelEntries { get; set; }
    public int NumberOfChargeEntries { get; set; }
}
