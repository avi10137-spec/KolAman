using alertApi.Maping;
using Microsoft.EntityFrameworkCore;
using System;
namespace alertApi.Repository
{
    public class AlertRepository
    {
        private readonly AlertDbContext _context;

        public AlertRepository(AlertDbContext context)
        {
            _context = context;
        }
        public async Task<object> GetAlertsByCommand()
        {
            return await _context.Alerts
                .GroupBy(a => a.Command)
                .Select(g => new
                {
                    Command = g.Key,
                    TotalAlerts = g.Count()
                })
                .OrderByDescending(x => x.TotalAlerts)
                .ToListAsync();
        }
        public async Task<object> GetAlertsByCommandAndSeverity()
        {
            return await _context.Alerts
                .GroupBy(a => new
                {
                    a.Command,
                    a.Priority
                })
                .Select(g => new
                {
                    Command = g.Key.Command,
                    priority = g.Key.Priority,
                    AlertCount = g.Count()
                })
                .OrderBy(x => x.Command)
                .ThenBy(x => x.priority)
                .ToListAsync();
        }
        public async Task<object> GetAlertsByCommandAndStatus()
        {
            return await _context.Alerts
                .GroupBy(a => new
                {
                    a.Command,
                    a.Status
                })
                .Select(g => new
                {
                    Command = g.Key.Command,
                    Status = g.Key.Status,
                    AlertCount = g.Count()
                })
                .OrderBy(x => x.Command)
                .ThenBy(x => x.Status)
                .ToListAsync();
        }
    }
}
