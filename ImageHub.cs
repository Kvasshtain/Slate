using Microsoft.AspNetCore.SignalR;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using DrawingServices;
using System.Diagnostics;
using System.Numerics;
 
namespace SignalRApp
{
    public class ImageHub : Hub
    {
        private IImageStoreService imageStoreService;
        
        public ImageHub(IImageStoreService imageStoreService)
        { 
            this.imageStoreService = imageStoreService ?? throw new ArgumentNullException(nameof(imageStoreService));
        }

        public async Task Send(CreateImageAction createImageAction)
        {          
            imageStoreService.Images.Add(createImageAction);

            await this.Clients.All.SendAsync("Receive", createImageAction);
            
            Debug.WriteLine("Send data");
        }
    }
}