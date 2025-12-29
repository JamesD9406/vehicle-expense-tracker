using System.ComponentModel.DataAnnotations;

namespace VehicleExpenseAPI.Models;

public class Receipt
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? Merchant { get; set; }
    
    public decimal? ParsedAmount { get; set; }
    
    public DateTime? ParsedDate { get; set; }
    
    // Foreign key to Expense (nullable - can upload receipt before creating expense)
    public int? ExpenseId { get; set; }
    
    // Navigation property
    public Expense? Expense { get; set; }
    
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
