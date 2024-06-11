namespace HangfireSpa.Server.Services
{
    public class ApplicationShutdownService
    {
        public CancellationTokenSource CancellationTokenSource { get; private set; }
        public CancellationToken Token => CancellationTokenSource.Token;

        public ApplicationShutdownService(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime)
        {
            CancellationTokenSource = new CancellationTokenSource();
            
            hostApplicationLifetime.ApplicationStopping.Register(() =>
            {
                CancellationTokenSource.Cancel();
            });
        }

        public void ManualCancel()
        {
            CancellationTokenSource.Cancel();
        }



    }
}
