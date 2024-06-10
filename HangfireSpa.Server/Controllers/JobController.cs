using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HangfireSpa.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<JobController> _logger;

        public JobController(IBackgroundJobClient backgroundJobClient, ILogger<JobController> logger)
        {
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetJob(string jobId)
        {
            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult EnqueueJob()
        {
            return Ok();
        }

       
    }
}
