using Hangfire;
using HangfireSpa.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HangfireSpa.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<JobsController> _logger;
        private readonly HangfireJobService _jobService;

        public JobsController(IBackgroundJobClient backgroundJobClient, ILogger<JobsController> logger, HangfireJobService jobService)
        {
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
            _jobService = jobService;
        }
       

        [HttpPost]
        [Route("[action]")]
        public IActionResult ScheduleMultipleJobs(int jobCount)
        {
            _jobService.ScheduleMultipleJobs(jobCount, () => JobCallbackAction(), CancellationToken.None);
            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult EnqueueMultipleJobs(int jobCount)
        {
            _jobService.EnqueueMultipleJobs(jobCount, () => JobCallbackAction(), CancellationToken.None);
            return Ok();
        }

        // Test Method to ensure the activator can invoke the method from another class
        [ApiExplorerSettings(IgnoreApi = true)]
        public void JobCallbackAction()
        {
            //TODO: Simulate a long running job
            Thread.Sleep(TimeSpan.FromSeconds(10));
            Console.WriteLine("Job Callback Action Invoked!");
        }

       
    }
}
