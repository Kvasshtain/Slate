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

        public async Task AddImage(RecImage image)
        {
            imageStoreService.Images.Add(image);

            await Clients.All.SendAsync("AddImageOnCanvas", image);

            Debug.WriteLine("Create image");
        }

        public async Task Drag(DragObjectData payload)
        {
            //imageStoreService.Images.Add(image); //Add image moving code!!!

            await Clients.Others.SendAsync("MoveObjectOnCanvas", payload);

            Debug.WriteLine("Drag object");
        }
    }
}