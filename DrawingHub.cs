using Microsoft.AspNetCore.SignalR;
using DrawingServices;
using System.Diagnostics;
 
namespace SignalRApp
{
    public class DrawingHub : Hub
    {
        private IPointStoreService pointService;
        private IRenderingService renderingService;

        public DrawingHub(IPointStoreService pointService, IRenderingService renderingService)
        { 
            this.pointService = pointService ?? throw new ArgumentNullException(nameof(pointService));
            this.renderingService = renderingService ?? throw new ArgumentNullException(nameof(renderingService));
        }

        public async Task Send(DrawingAction drawingAction)
        {          
            pointService.Points.Add(drawingAction);

            var data = renderingService.Render();

            await Clients.All.SendAsync("Receive", data);
            
            Debug.WriteLine("Send data");
        }
    }
}