using consumerrabbit.Models.consumerrabbit.Models;
using consumerrabbit.Repository;



using consumerrabbit.Repository;

namespace consumerrabbit.Services
{
    public class AlertScannerService
    {
        private readonly AlertRepository _repository;

        public AlertScannerService(AlertRepository repository)
        {
            _repository = repository;
        }

        public async Task StartAsync()
        {
            while (true)
            {
                var alerts = await _repository.GetNewAlertsAsync();

                foreach (var alert in alerts)
                {
                    
                    if (alert.Status == "WAITING")
                    Console.WriteLine($"New alert: {alert.AlertId}");
                    alert.Status = "INPROGRES";
                }

                await Task.Delay(1000);
            }
        }
    }
}




