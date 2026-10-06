using consumerrabbit.Maping;
using consumerrabbit.Models;
using consumerrabbit.Models.consumerrabbit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace consumerrabbit.Repository
{
    public class AlertRepository
    {
        private readonly AlertDbContext _dbcontext;
        public AlertRepository (AlertDbContext dbContext)
        {
            _dbcontext = dbContext;
        } 
        public async Task SaveAsync(Alert alert)
        {


            _dbcontext.Alerts.Add(alert);

            await _dbcontext.SaveChangesAsync();
        }
        public async Task<List<Alert>> GetNewAlertsAsync()
        {
            return _dbcontext.Alerts
                .Where(a => a.Status == "WAITING")
                .ToList();
        }
    }
}
