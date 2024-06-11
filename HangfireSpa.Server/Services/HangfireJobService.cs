using Hangfire;
using Hangfire.Storage;
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
            switch (jobType)
            {
                case JobType.Scheduled:
                    ScheduleMultipleJobs(jobCount, jobMethod, token);
                    break;
                case JobType.Enqueued:
                    break;
            }
        }

        public IEnumerable<string> GetAllJobs(bool includeDeleted = false, bool includeFailed = false)
        {
            var jobsList = new List<string>();
            jobsList.AddRange(GetScheduledJobs());
            jobsList.AddRange(GetEnqueuedJobs());
            jobsList.AddRange(GetCompletedJobs());
            jobsList.AddRange(GetProcessingJobs());
            jobsList.AddRange(GetReoccuringJobs());
            if (includeDeleted) jobsList.AddRange(GetDeletedJobs());
            if (includeFailed) jobsList.AddRange(GetFailedJobs());
            return jobsList;
        }

        public IEnumerable<string> GetScheduledJobs()
        {
            var jobsList = new List<string>();

            jobsList = JobStorage.Current.GetMonitoringApi().ScheduledJobs(0, int.MaxValue)
                        .Select(x => x.Key).ToList();

            return jobsList;
        }

        public IEnumerable<string> GetEnqueuedJobs()
        {
            var jobsList = new List<string>();
            jobsList = JobStorage.Current.GetMonitoringApi().EnqueuedJobs("default", 0, int.MaxValue)
                        .Select(x => x.Key).ToList();
            return jobsList;
        }

        public IEnumerable<string> GetCompletedJobs()
        {
            var jobsList = new List<string>();
            jobsList = JobStorage.Current.GetMonitoringApi().SucceededJobs(0, int.MaxValue)
                        .Select(x => x.Key).ToList();
            return jobsList;
        }

        public IEnumerable<string> GetFailedJobs()
        {
            var jobsList = new List<string>();
            jobsList = JobStorage.Current.GetMonitoringApi().FailedJobs(0, int.MaxValue)
                        .Select(x => x.Key).ToList();
            return jobsList;
        }

        public IEnumerable<string> GetProcessingJobs()
        {
            var jobsList = new List<string>();
            jobsList = JobStorage.Current.GetMonitoringApi().ProcessingJobs(0, int.MaxValue)
                        .Select(x => x.Key).ToList();
            return jobsList;
        }

        public IEnumerable<string> GetReoccuringJobs()
        {
            var jobsList = new List<string>();
            jobsList = JobStorage.Current.GetConnection().GetRecurringJobs()
                        .Select(x => x.Id).ToList();

            return jobsList;
        }

        public IEnumerable<string> GetDeletedJobs()
        {
            var jobsList = new List<string>();
            jobsList = JobStorage.Current.GetMonitoringApi().DeletedJobs(0, int.MaxValue)
                        .Select(x => x.Key).ToList();
            return jobsList;
        }
    }
}
