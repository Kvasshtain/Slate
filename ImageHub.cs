using System.Diagnostics;
using DrawingServices;
using Microsoft.AspNetCore.SignalR;

namespace SignalRApp
{
    public class ImageHub(IBlackboardStoreService imageStoreService) : Hub
    {
        private readonly IBlackboardStoreService objectStoreService =
            imageStoreService ?? throw new ArgumentNullException(nameof(imageStoreService));

        public async Task GetAllBoardObjects()
        {
            foreach (var boardObject in objectStoreService.BlackboardObjects)
            {
                await Clients.Caller.SendAsync("AddObjectOnCanvas", boardObject);

                Debug.WriteLine($"Send object {boardObject.Id} from store");
            }
        }

        public async Task AddObject(BlackboardObjectData boardObject)
        {
            if (objectStoreService.TryAddObject(boardObject))
            {
                await Clients.All.SendAsync("AddObjectOnCanvas", boardObject);
                Debug.WriteLine("Create object");

                return;
            }

            await Clients.Caller.SendAsync("AddObjectError", boardObject);
            Debug.WriteLine("Create object error");
        }

        public async Task DeleteObjectsByIds(string[] deletedObjectsIds)
        {
            if (objectStoreService.TryDeleteObjectsByIds(deletedObjectsIds))
            {
                await Clients.All.SendAsync("DeleteObjectsOnCanvas", deletedObjectsIds);
                Debug.WriteLine("Delete objects by Ids");

                return;
            }

            await Clients.Caller.SendAsync("DeleteObjectsError", deletedObjectsIds);
             
        }

        public async Task Drag(DragObjectData payload)
        {
            objectStoreService.DragObject(payload);

            await Clients.Others.SendAsync("MoveObjectOnCanvas", payload);

            Debug.WriteLine("Drag object");
        }

        public async Task Scale(ScaleObjectData payload)
        {
            objectStoreService.ScaleObject(payload);

            await Clients.Others.SendAsync("ScaleObjectOnCanvas", payload);

            Debug.WriteLine("Scale object");
        }

        public async Task Rotate(RotateObjectData payload)
        {
            objectStoreService.RotateObject(payload);

            await Clients.Others.SendAsync("RotateObjectOnCanvas", payload);

            Debug.WriteLine("Rotate object");
        }
    }
}
