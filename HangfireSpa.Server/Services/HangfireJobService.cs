using Hangfire;
using Hangfire.Storage.Monitoring;
using HangfireSpa.Server.Data;
using System.Linq.Expressions;

namespace HangfireSpa.Server.Services
{
    public class HangfireJobService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<HangfireJobService> _logger;

        public HangfireJobService(IBackgroundJobClient backgroundJobClient, ILogger<HangfireJobService> logger)
        {
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
        }

        public void ScheduleMultipleJobs(int jobCount, Expression<Action> jobMethod, CancellationToken token)
        {
            foreach (var job in Enumerable.Range(1, jobCount))
            {
                if (!token.IsCancellationRequested)
                {
                    _logger.LogDebug($"Scheduled a Job! {job}");
                    _backgroundJobClient.Schedule(jobMethod, TimeSpan.FromSeconds(30));
                }
                else
                {
                    _logger.LogDebug("Cancellation requested!");
                    break;
                }
            }
        }

        public void EnqueueMultipleJobs(int jobCount, Expression<Action> jobMethod, CancellationToken token)
        {
            foreach (var job in Enumerable.Range(1, jobCount))
            {
                if (!token.IsCancellationRequested)
                {
                    _logger.LogDebug($"Scheduled a Job! {job}");
                    _backgroundJobClient.Enqueue(jobMethod);
                }
                else
                {
                    _logger.LogDebug("Cancellation requested!");
                    break;
                }
            }
        }

        public void ScheduleOrEnqueueJob(int jobCount, JobType jobType, Expression<Action> jobMethod, CancellationToken token)
        {
            switch(jobType)
            {
                case JobType.Scheduled:
                    ScheduleMultipleJobs(jobCount, jobMethod, token);
                    break;
                case JobType.Enqueued:
                    break;
            }
        }

        public object GetJobs()
        {
            var jobsList = new List<string>();

            jobsList = JobStorage.Current.GetMonitoringApi().ScheduledJobs(0, int.MaxValue)
                        .Select(x => x.Key).ToList();

            return jobsList;
        }
    }
}
