using Hangfire;
using HangfireSpa.Server.Helpers;
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
        private readonly ApplicationShutdownService _shutdownService;

        public JobsController(IBackgroundJobClient backgroundJobClient, ILogger<JobsController> logger, HangfireJobService jobService, ApplicationShutdownService shutdownService)
        {
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
            _jobService = jobService;
            _shutdownService = shutdownService;
        }


        [HttpPost]
        [Route("[action]")]
        public IActionResult ScheduleMultipleJobs(int jobCount)
        {
            _jobService.ScheduleMultipleJobs(jobCount, () => JobCallbackAction(), _shutdownService.Token);
            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult EnqueueMultipleJobs(int jobCount)
        {
            _jobService.EnqueueMultipleJobs(jobCount, () => JobCallbackAction(), _shutdownService.Token);
            return Ok();
        }

        // Test Method to ensure the activator can invoke the method from another class
        [ApiExplorerSettings(IgnoreApi = true)]
        [JobNotification]
        public void JobCallbackAction()
        {
            //TODO: Simulate a long running job
            Thread.Sleep(TimeSpan.FromSeconds(10));
            Console.WriteLine("Job Callback Action Invoked!");
        }


    }
}
