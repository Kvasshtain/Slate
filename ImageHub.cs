using Microsoft.AspNetCore.SignalR;
using DrawingServices;
using System.Diagnostics;

namespace SignalRApp
{
    public class ImageHub : Hub
    {
        private readonly IBlackboardStoreService imageStoreService;

        public ImageHub(IBlackboardStoreService imageStoreService)
        {
            this.imageStoreService = imageStoreService ?? throw new ArgumentNullException(nameof(imageStoreService));
        }
        
        public async Task GetAllDrawingObjects()
        {
            foreach(var image in imageStoreService.BlackboardObjects)
            {
                await Clients.Caller.SendAsync("AddObjectOnCanvas", image);

                Debug.WriteLine($"Send image {image.Id} from store");
            }
        }

        public async Task AddObject(BlackboardObjectData image)
        {
            if (imageStoreService.TryAddObject(image))
            {
                await Clients.All.SendAsync("AddObjectOnCanvas", image);
                Debug.WriteLine("Create image");

                return;
            }
            
            await Clients.Caller.SendAsync("AddObjectError", image);
            Debug.WriteLine("Create image error");
        }

        public async Task Drag(DragObjectData payload)
        {
            imageStoreService.DragObject(payload);

            await Clients.Others.SendAsync("MoveObjectOnCanvas", payload);

            Debug.WriteLine("Drag object");
        }
    
        public async Task Scale(ScaleObjectData payload)
        {
            imageStoreService.ScaleObject(payload);

            await Clients.Others.SendAsync("ScaleObjectOnCanvas", payload);

            Debug.WriteLine("Scale object");
        }
    }
}