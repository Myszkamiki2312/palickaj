using System.ComponentModel.DataAnnotations;

namespace Palickaj.Models;

public sealed class ServiceTicket
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public Client? Client { get; set; }

    [Required]
    [StringLength(30)]
    public string Number { get; set; } = string.Empty;

    [Required]
    [StringLength(160)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(40)]
    public string Module { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string Status { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Priority { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string AssignedEngineer { get; set; } = string.Empty;

    [Range(0, 500)]
    public decimal PlannedHours { get; set; }

    [Range(0, 500)]
    public decimal SpentHours { get; set; }

    [Required]
    [StringLength(30)]
    public string SourceChannel { get; set; } = string.Empty;

    [Required]
    [StringLength(60)]
    public string AffectedVersion { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime DueAt { get; set; }

    public bool RequiresDeployment { get; set; }

    public bool IsBillable { get; set; }
}
