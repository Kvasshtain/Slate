using System.Diagnostics;
using DrawingServices;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace SignalRApp
{
    [Authorize]
    public class ImageHub(IBlackboardStoreService imageStoreService) : Hub
    {
        private readonly IBlackboardStoreService objectStoreService =
            imageStoreService ?? throw new ArgumentNullException(nameof(imageStoreService));

        public async Task MoveCursor(CursorData cursorData)
        {
            ArgumentNullException.ThrowIfNull(cursorData);

            await Clients.All.SendAsync("MoveCursorOnCanvas", cursorData);
            Debug.WriteLine("Move cursor");
            return;
        }
         
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
            ArgumentNullException.ThrowIfNull(boardObject);

            if (await objectStoreService.TryAddObject(boardObject))
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
            ArgumentNullException.ThrowIfNull(deletedObjectsIds);

            foreach (var id in deletedObjectsIds)
            {
                if (!await objectStoreService.TryDeleteObjectById(id))
                {
                    await Clients.Caller.SendAsync("DeleteObjectsError", id);
                    continue;
                }

                await Clients.All.SendAsync("DeleteObjectsOnCanvas", id);
            }
        }

        public async Task Drag(DragObjectData payload)
        {
            ArgumentNullException.ThrowIfNull(payload);

            if (await objectStoreService.DragObject(payload))
            {
                await Clients.Others.SendAsync("MoveObjectOnCanvas", payload);
                Debug.WriteLine("Drag object");

                return;
            }

            await Clients.Caller.SendAsync("DragObjectError", payload);
            Debug.WriteLine("Drag object error");
        }

        public async Task Scale(ScaleObjectData payload)
        {
            ArgumentNullException.ThrowIfNull(payload);

            if (await objectStoreService.ScaleObject(payload))
            {
                await Clients.Others.SendAsync("ScaleObjectOnCanvas", payload);
                Debug.WriteLine("Scale object");

                return;
            }

            await Clients.Caller.SendAsync("ScaleObjectError", payload);
            Debug.WriteLine("Scale object error");
        }

        public async Task Rotate(RotateObjectData payload)
        {
            ArgumentNullException.ThrowIfNull(payload);
            
            if(await objectStoreService.RotateObject(payload))
            {
                await Clients.Others.SendAsync("RotateObjectOnCanvas", payload);
                Debug.WriteLine("Rotate object");

                return;
            }

            await Clients.Caller.SendAsync("RotateObjectError", payload);
            Debug.WriteLine("Rotate object error");
        }
    }
}
