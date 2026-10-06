using consumerrabbit.Repository;

namespace consumerrabbit.Services
{
    public class AlertScannerService
    {
        private readonly AlertRepository _repository;
        private readonly TaskManagerService _taskManager;

        public AlertScannerService(
            AlertRepository repository,
            TaskManagerService taskManager)
        {
            _repository = repository;
            _taskManager = taskManager;
        }

        public async Task StartAsync()
        {
            Console.WriteLine("AlertScannerService started scanning DB for new alerts...");

            while (true)
            {
                try
                {
                    var alerts = await _repository.GetNewAlertsAsync();

                    foreach (var alert in alerts)
                    {
                        await _taskManager.HandleAlertAsync(alert);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in AlertScannerService: {ex.Message}");
                }

                await Task.Delay(1000);
            }
        }
    }
}