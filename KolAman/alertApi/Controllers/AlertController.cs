
using Microsoft.AspNetCore.Mvc;
using alertApi.Repository;
namespace alertApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AlertsController : ControllerBase
    {
        private readonly AlertRepository _repository;

        public AlertsController(AlertRepository repository)
        {
            _repository = repository;
        }



        [HttpGet("by-command")]
        public async Task<IActionResult> GetAlertsByCommand()
        {
            var result = await _repository.GetAlertsByCommand();

            return Ok(result);
        }
        [HttpGet("by-command-piority")]
        public async Task<IActionResult> GetAlertsByCommandAndpriority()
        {
            var result =
                await _repository.GetAlertsByCommandAndPriority();

            return Ok(result);
        }
        [HttpGet("by-command-status")]
        public async Task<IActionResult> GetAlertsByCommandAndStatus()
        {
            var result =
                await _repository.GetAlertsByCommandAndStatus();

            return Ok(result);
        }
    }
}
        
