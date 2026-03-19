using Microsoft.EntityFrameworkCore;
using Palickaj.Data;

namespace Palickaj.Services;

public sealed class TicketNumberGenerator(AppDbContext db)
{
    public async Task<string> GenerateAsync()
    {
        var currentMonthPrefix = $"SRV-{DateTime.Today:yyMM}";
        var latestNumber = await db.ServiceTickets
            .Where(ticket => ticket.Number.StartsWith(currentMonthPrefix))
            .OrderByDescending(ticket => ticket.Number)
            .Select(ticket => ticket.Number)
            .FirstOrDefaultAsync();

        var nextSequence = 1;

        if (!string.IsNullOrWhiteSpace(latestNumber))
        {
            var parts = latestNumber.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out var parsed))
            {
                nextSequence = parsed + 1;
            }
        }

        return $"{currentMonthPrefix}-{nextSequence:D3}";
    }
}
