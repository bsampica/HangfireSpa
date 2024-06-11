namespace HangfireSpa.Server.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Threading.Tasks;

    public interface INotifyClientHub
    {
        public Task ReceiveJobUpdate(string message);
        public Task ReceiveJobsList(string message);
    }

    public class JobDataHub(ILogger logger) : Hub<INotifyClientHub>
    {

        public async Task SendJobUpdate(string message)
        {
            await Clients.All.ReceiveJobUpdate(message);
        }

        public async Task SendJobsList(string message)
        {
            await Clients.All.ReceiveJobsList(message);
        }

        public override Task OnConnectedAsync()
        {
            logger.LogInformation($"Client connected, {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            logger.LogInformation($"Client disconnected, {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
