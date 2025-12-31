using FluentValidation;
using VehicleExpenseAPI.DTOs.Fuel;
using VehicleExpenseAPI.Models;

namespace VehicleExpenseAPI.Validators;

public class CreateFuelEntryDtoValidator : AbstractValidator<CreateFuelEntryDto>
{
    public CreateFuelEntryDtoValidator()
    {
        RuleFor(x => x.EnergyType)
            .IsInEnum()
            .WithMessage("Invalid energy type");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0")
            .LessThanOrEqualTo(1000)
            .WithMessage("Amount seems unreasonably high (max 1000 L or kWh)");

        RuleFor(x => x.Cost)
            .GreaterThan(0)
            .WithMessage("Cost must be greater than 0")
            .LessThanOrEqualTo(100000)
            .WithMessage("Cost seems unreasonably high");

        RuleFor(x => x.Odometer)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Odometer reading cannot be negative")
            .LessThanOrEqualTo(10000000)
            .WithMessage("Odometer reading seems unreasonably high");

        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date is required")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Date cannot be in the future");

        RuleFor(x => x.VehicleId)
            .GreaterThan(0)
            .WithMessage("Valid Vehicle ID is required");
    }
}
