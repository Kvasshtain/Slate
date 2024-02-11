using Microsoft.AspNetCore.SignalR;
using DrawingServices;
using System.Diagnostics;
 
namespace SignalRApp
{
    public class ImageHub : Hub
    {
        private readonly IImageStoreService imageStoreService;
        
        public ImageHub(IImageStoreService imageStoreService)
        { 
            this.imageStoreService = imageStoreService ?? throw new ArgumentNullException(nameof(imageStoreService));
        }

        public async Task Send(Image image)
        {          
            imageStoreService.Images.Add(image);

            await Clients.All.SendAsync("Receive", image);
            
            Debug.WriteLine("Send data");
        }
    }
}