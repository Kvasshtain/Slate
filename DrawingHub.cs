using System.Diagnostics;
using DrawingServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SignalRApp
{
    [Authorize]
    public class DrawingHub(IPointStoreService pointService, IRenderingService renderingService)
        : Hub
    {
        private IPointStoreService pointService =
            pointService ?? throw new ArgumentNullException(nameof(pointService));
        private IRenderingService renderingService =
            renderingService ?? throw new ArgumentNullException(nameof(renderingService));

        public async Task Send(DrawingAction drawingAction)
        {
            pointService.Points.Add(drawingAction);

            var data = renderingService.Render();

            await Clients.All.SendAsync("Receive", data);

            Debug.WriteLine("Send data");
        }
    }
}
