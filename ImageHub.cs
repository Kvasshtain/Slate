using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using slate.DrawingServices;

namespace slate
{
    [Authorize]
    public class ImageHub(IBlackboardStoreService imageStoreService) : Hub
    {
        private readonly IBlackboardStoreService objectStoreService =
            imageStoreService ?? throw new ArgumentNullException(nameof(imageStoreService));

        public async Task EnterBlackboard( /*string userName, */
            int blackboardId
        )
        {
            //ДОБАВИТЬ ПРОВЕРКУ ЧТО ПОЛЬЗОВАТЕЛЬ И ДОСКА СУЩЕСТВУЮТ

            await Groups.AddToGroupAsync(Context.ConnectionId, blackboardId.ToString());
            //await Clients.All.SendAsync("Notify", $"{userName} вошел в чат в группу {blackboardId}");
        }

        public async Task MoveCursor(CursorData cursorData)
        {
            ArgumentNullException.ThrowIfNull(cursorData);

            await Clients
                .Group(cursorData.BlackboardId.ToString())
                .SendAsync("MoveCursorOnCanvas", cursorData);
            Debug.WriteLine("Move cursor");
        }

        public async Task GetAllBoardObjects(int blackboardId)
        {
            foreach (var boardObject in objectStoreService.GetBlackboardObjects(blackboardId))
            {
                await Clients.Caller.SendAsync("AddObjectOnCanvas", boardObject);

                Debug.WriteLine($"Send object {boardObject.Id} from store");
            }
        }

        public async Task AddObject(BlackboardObjectData boardObject)
        {
            ArgumentNullException.ThrowIfNull(boardObject);

            if (await objectStoreService.TryAddObjectAsync(boardObject))
            {
                await Clients
                    .Group(boardObject.BlackboardId.ToString())
                    .SendAsync("AddObjectOnCanvas", boardObject);
                Debug.WriteLine("Create object");

                return;
            }

            await Clients.Caller.SendAsync("AddObjectError", boardObject);
            Debug.WriteLine("Create object error");
        }

        public async Task DeleteObjectsByIds(int[] deletedObjectsIds)
        {
            ArgumentNullException.ThrowIfNull(deletedObjectsIds);

            foreach (var id in deletedObjectsIds)
            {
                int? blackboardId = await GetObjectBlackboardIdAsyncAndSendErrorIfNull(id);

                if (!await objectStoreService.TryDeleteObjectByIdAsync(id) || blackboardId is null)
                {
                    await Clients.Caller.SendAsync("DeleteObjectsError", id);
                    continue;
                }

                await Clients.Group(blackboardId.ToString() ?? string.Empty).SendAsync("DeleteObjectsOnCanvas", id);
            }
        }

        private async Task PerformAction<TActionData>(TActionData payload, Func<TActionData, Task<bool>> action, string actionName) where TActionData: IActionData
        {
            int? blackboardId = await GetObjectBlackboardIdAsyncAndSendErrorIfNull(payload.Id);

            if (!await action(payload) || blackboardId is null)
            {
                await Clients.Caller.SendAsync($"{actionName}ObjectError", payload);
                Debug.WriteLine($"{actionName} object error");

                return;
            }

            await Clients
                .OthersInGroup(blackboardId.ToString() ?? string.Empty)
                .SendAsync($"{actionName}ObjectOnCanvas", payload);
            Debug.WriteLine($"{actionName} object");
        }
        
        public async Task Drag(DragObjectData payload)
        {
            ArgumentNullException.ThrowIfNull(payload);
            await PerformAction(payload, objectStoreService.DragObjectAsync, "Drag");
        }

        public async Task Scale(ScaleObjectData payload)
        {
            ArgumentNullException.ThrowIfNull(payload);
            await PerformAction(payload, objectStoreService.ScaleObjectAsync, "Scale");
        }

        public async Task Rotate(RotateObjectData payload)
        {
            ArgumentNullException.ThrowIfNull(payload);
            await PerformAction(payload, objectStoreService.RotateObjectAsync, "Rotate");
        }

        private async Task<int?> GetObjectBlackboardIdAsyncAndSendErrorIfNull(int id)
        {
            int? blackboardId = await objectStoreService.GetObjectBlackboardIdAsync(id);

            if (blackboardId is null)
                await Clients.Caller.SendAsync("DeleteObjectsError", id);

            return blackboardId;
        }
    }
}
