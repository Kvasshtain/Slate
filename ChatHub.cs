using Microsoft.AspNetCore.SignalR;
 
namespace SignalRApp
{
    public class ChatHub : Hub
    {
        public async Task Send(DrawingAction drawingAction)
        {
            await this.Clients.All.SendAsync("Receive", drawingAction);
        }
    }

    public class DrawingAction
    {
        public double X {get; set;}
        public double Y {get; set;}
    }
}