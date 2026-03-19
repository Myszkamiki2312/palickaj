using System.ComponentModel.DataAnnotations;

namespace ErpSupportDesk.Models;

public sealed class Client
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Industry { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string ErpEnvironment { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string ContactPerson { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(120)]
    public string ContactEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(40)]
    public string SupportPlan { get; set; } = string.Empty;

    [Range(1, 5000)]
    public int ActiveUsers { get; set; }

    [Required]
    [StringLength(80)]
    public string DatabaseName { get; set; } = string.Empty;

    public DateTime LastReviewAt { get; set; }

    public List<ServiceTicket> Tickets { get; set; } = [];

    public List<IntegrationFlow> Integrations { get; set; } = [];
}
