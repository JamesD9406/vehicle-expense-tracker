using Microsoft.EntityFrameworkCore;
using VehicleExpenseAPI.Data;
using VehicleExpenseAPI.DTOs.Fuel;
using VehicleExpenseAPI.Models;

namespace VehicleExpenseAPI.Services;

public class FuelService
{
    private readonly ApplicationDbContext _context;

    public FuelService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FuelEntryDto>> GetAllAsync(string userId, int? vehicleId = null, EnergyType? energyType = null)
    {
        var query = _context.FuelEntries
            .Include(f => f.Vehicle)
            .Where(f => f.Vehicle!.UserId == userId);

        if (vehicleId.HasValue)
        {
            query = query.Where(f => f.VehicleId == vehicleId.Value);
        }

        if (energyType.HasValue)
        {
            query = query.Where(f => f.EnergyType == energyType.Value);
        }

        var fuelEntries = await query
            .OrderByDescending(f => f.Date)
            .ThenByDescending(f => f.Odometer)
            .ToListAsync();

        return fuelEntries.Select(f => MapToDto(f));
    }

    public async Task<FuelEntryDto?> GetByIdAsync(int id, string userId)
    {
        var fuelEntry = await _context.FuelEntries
            .Include(f => f.Vehicle)
            .Where(f => f.Id == id && f.Vehicle!.UserId == userId)
            .FirstOrDefaultAsync();

        if (fuelEntry == null)
        {
            return null;
        }

        return MapToDto(fuelEntry);
    }

    public async Task<FuelEntryDto?> CreateAsync(CreateFuelEntryDto createDto, string userId)
    {
        // Verify the vehicle belongs to the user
        var vehicle = await _context.Vehicles
            .Where(v => v.Id == createDto.VehicleId && v.UserId == userId)
            .FirstOrDefaultAsync();

        if (vehicle == null)
        {
            return null;
        }

        // Create the Fuel Expense first (so it gets an ID)
        var fuelNote = createDto.EnergyType == EnergyType.Electricity
            ? $"Charging session: {createDto.Amount} kWh"
            : $"{(createDto.EnergyType == EnergyType.Diesel ? "Diesel" : "Gasoline")} fill-up: {createDto.Amount}L";

        var fuelExpense = new Expense
        {
            Category = ExpenseCategory.Fuel,
            Amount = createDto.Cost,
            Date = createDto.Date,
            Notes = fuelNote,
            VehicleId = createDto.VehicleId
        };

        _context.Expenses.Add(fuelExpense);
        await _context.SaveChangesAsync(); // Save to get the Expense ID

        // Now create the FuelEntry linked to the Expense
        var fuelEntry = new FuelEntry
        {
            EnergyType = createDto.EnergyType,
            Amount = createDto.Amount,
            Cost = createDto.Cost,
            Odometer = createDto.Odometer,
            Date = createDto.Date,
            VehicleId = createDto.VehicleId,
            Vehicle = vehicle,
            ExpenseId = fuelExpense.Id
        };

        _context.FuelEntries.Add(fuelEntry);
        await _context.SaveChangesAsync();

        return MapToDto(fuelEntry);
    }

    public async Task<(bool Success, FuelEntryDto? FuelEntry)> UpdateAsync(int id, UpdateFuelEntryDto updateDto, string userId)
    {
        var fuelEntry = await _context.FuelEntries
            .Include(f => f.Vehicle)
            .Include(f => f.Expense) // Include the linked expense
            .Where(f => f.Id == id && f.Vehicle!.UserId == userId)
            .FirstOrDefaultAsync();

        if (fuelEntry == null)
        {
            return (false, null);
        }

        // If VehicleId is changing, verify the new vehicle belongs to the user
        if (fuelEntry.VehicleId != updateDto.VehicleId)
        {
            var newVehicle = await _context.Vehicles
                .Where(v => v.Id == updateDto.VehicleId && v.UserId == userId)
                .FirstOrDefaultAsync();

            if (newVehicle == null)
            {
                return (false, null);
            }
            
            fuelEntry.VehicleId = updateDto.VehicleId;
            fuelEntry.Vehicle = newVehicle;
            
            // Also update the linked expense's vehicle
            if (fuelEntry.Expense != null)
            {
                fuelEntry.Expense.VehicleId = updateDto.VehicleId;
            }
        }

        // Update FuelEntry fields
        fuelEntry.EnergyType = updateDto.EnergyType;
        fuelEntry.Amount = updateDto.Amount;
        fuelEntry.Cost = updateDto.Cost;
        fuelEntry.Odometer = updateDto.Odometer;
        fuelEntry.Date = updateDto.Date;

        // Update the linked Expense to match
        if (fuelEntry.Expense != null)
        {
            fuelEntry.Expense.Amount = updateDto.Cost;
            fuelEntry.Expense.Date = updateDto.Date;
            fuelEntry.Expense.Notes = updateDto.EnergyType == EnergyType.Electricity
                ? $"Charging session: {updateDto.Amount} kWh"
                : $"{(updateDto.EnergyType == EnergyType.Diesel ? "Diesel" : "Gasoline")} fill-up: {updateDto.Amount}L";
        }

        await _context.SaveChangesAsync();

        return (true, MapToDto(fuelEntry));
    }

    public async Task<bool> DeleteAsync(int id, string userId)
    {
        var fuelEntry = await _context.FuelEntries
            .Include(f => f.Vehicle)
            .Include(f => f.Expense) // Include the linked expense
            .Where(f => f.Id == id && f.Vehicle!.UserId == userId)
            .FirstOrDefaultAsync();

        if (fuelEntry == null)
        {
            return false;
        }

        // Delete the linked Fuel Expense too
        if (fuelEntry.Expense != null)
        {
            _context.Expenses.Remove(fuelEntry.Expense);
        }

        _context.FuelEntries.Remove(fuelEntry);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Calculate fuel efficiency statistics for a specific vehicle (handles hybrid vehicles)
    /// </summary>
    public async Task<FuelEfficiencyDto?> GetEfficiencyAsync(int vehicleId, string userId)
    {
        var vehicle = await _context.Vehicles
            .Where(v => v.Id == vehicleId && v.UserId == userId)
            .FirstOrDefaultAsync();

        if (vehicle == null)
        {
            return null;
        }

        var allEntries = await _context.FuelEntries
            .Where(f => f.VehicleId == vehicleId)
            .OrderBy(f => f.Odometer)
            .ToListAsync();

        if (allEntries.Count < 2)
        {
            // Need at least 2 entries to calculate efficiency
            var fuelEntries = allEntries.Where(e => e.EnergyType != EnergyType.Electricity).ToList();
            var electricEntries = allEntries.Where(e => e.EnergyType == EnergyType.Electricity).ToList();

            return new FuelEfficiencyDto
            {
                VehicleId = vehicleId,
                VehicleMake = vehicle.Make,
                VehicleModel = vehicle.Model,
                VehicleType = vehicle.VehicleType,
                TotalFuelCost = fuelEntries.Sum(f => f.Cost),
                TotalFuelLiters = fuelEntries.Sum(f => f.Amount),
                TotalElectricityCost = electricEntries.Sum(f => f.Cost),
                TotalElectricityKwh = electricEntries.Sum(f => f.Amount),
                TotalEnergyCost = allEntries.Sum(f => f.Cost),
                TotalKilometers = 0,
                NumberOfFuelEntries = fuelEntries.Count,
                NumberOfChargeEntries = electricEntries.Count,
                FirstEntryDate = allEntries.FirstOrDefault()?.Date,
                LastEntryDate = allEntries.LastOrDefault()?.Date
            };
        }

        // Separate fuel and electricity entries
        var fuelOnlyEntries = allEntries.Where(e => e.EnergyType != EnergyType.Electricity).OrderBy(e => e.Odometer).ToList();
        var electricOnlyEntries = allEntries.Where(e => e.EnergyType == EnergyType.Electricity).OrderBy(e => e.Odometer).ToList();

        var totalKilometers = allEntries.Last().Odometer - allEntries.First().Odometer;
        
        // Calculate fuel statistics
        var totalFuelLiters = fuelOnlyEntries.Sum(f => f.Amount);
        var totalFuelCost = fuelOnlyEntries.Sum(f => f.Cost);
        
        // Calculate electricity statistics
        var totalElectricityKwh = electricOnlyEntries.Sum(f => f.Amount);
        var totalElectricityCost = electricOnlyEntries.Sum(f => f.Cost);
        
        // Combined cost per km
        var totalEnergyCost = totalFuelCost + totalElectricityCost;
        var avgCostPerKm = totalKilometers > 0 ? totalEnergyCost / totalKilometers : 0;

        // Fuel efficiency (L/100km)
        var litersPer100Km = totalKilometers > 0 && totalFuelLiters > 0 
            ? (totalFuelLiters / totalKilometers) * 100 
            : 0;
        var fuelCostPerKm = totalKilometers > 0 && totalFuelCost > 0
            ? totalFuelCost / totalKilometers 
            : 0;

        // Electric efficiency (kWh/100km)
        var kwhPer100Km = totalKilometers > 0 && totalElectricityKwh > 0
            ? (totalElectricityKwh / totalKilometers) * 100 
            : 0;
        var electricCostPerKm = totalKilometers > 0 && totalElectricityCost > 0
            ? totalElectricityCost / totalKilometers 
            : 0;

        return new FuelEfficiencyDto
        {
            VehicleId = vehicleId,
            VehicleMake = vehicle.Make,
            VehicleModel = vehicle.Model,
            VehicleType = vehicle.VehicleType,
            
            // Fuel stats
            TotalFuelLiters = totalFuelLiters,
            TotalFuelCost = totalFuelCost,
            AverageLitersPer100Km = Math.Round(litersPer100Km, 2),
            AverageFuelCostPerKm = Math.Round(fuelCostPerKm, 4),
            
            // Electric stats
            TotalElectricityKwh = totalElectricityKwh,
            TotalElectricityCost = totalElectricityCost,
            AverageKwhPer100Km = Math.Round(kwhPer100Km, 2),
            AverageElectricityCostPerKm = Math.Round(electricCostPerKm, 4),
            
            // Combined stats
            TotalEnergyCost = totalEnergyCost,
            AverageCostPerKm = Math.Round(avgCostPerKm, 4),
            TotalKilometers = totalKilometers,
            
            FirstEntryDate = allEntries.First().Date,
            LastEntryDate = allEntries.Last().Date,
            NumberOfFuelEntries = fuelOnlyEntries.Count,
            NumberOfChargeEntries = electricOnlyEntries.Count
        };
    }

    private static FuelEntryDto MapToDto(FuelEntry fuelEntry)
    {
        var unit = fuelEntry.EnergyType == EnergyType.Electricity ? "kWh" : "L";
        var energyTypeDisplay = fuelEntry.EnergyType switch
        {
            EnergyType.Gasoline => "Gasoline",
            EnergyType.Diesel => "Diesel",
            EnergyType.Electricity => "Electricity",
            _ => "Unknown"
        };

        return new FuelEntryDto
        {
            Id = fuelEntry.Id,
            EnergyType = fuelEntry.EnergyType,
            EnergyTypeDisplay = energyTypeDisplay,
            Amount = fuelEntry.Amount,
            Unit = unit,
            Cost = fuelEntry.Cost,
            Odometer = fuelEntry.Odometer,
            Date = fuelEntry.Date,
            VehicleId = fuelEntry.VehicleId,
            VehicleMake = fuelEntry.Vehicle!.Make,
            VehicleModel = fuelEntry.Vehicle!.Model,
            VehicleType = fuelEntry.Vehicle!.VehicleType,
            CostPerUnit = fuelEntry.Amount > 0 ? Math.Round(fuelEntry.Cost / fuelEntry.Amount, 2) : 0
        };
    }
}
