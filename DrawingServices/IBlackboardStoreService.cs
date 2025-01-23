namespace slate.DrawingServices
{
    public interface IBlackboardStoreService
    {
        IEnumerable<BlackboardObjectData> GetBlackboardObjects(int blackboardId);
        Task<BlackboardObjectData?> GetBlackboardObjectByIdAsync(int id);
        Task<int?> GetObjectBlackboardIdAsync(int id);
        Task<bool> TryAddObjectAsync(BlackboardObjectData blackboardObjectData);
        Task<bool> TryDeleteObjectByIdAsync(int deletedFromCanvasObjectId);
        Task<bool> DragObjectAsync(DragObjectData dragObjectData);
        Task<bool> ScaleObjectAsync(ScaleObjectData scaleObjectData);
        Task<bool> RotateObjectAsync(RotateObjectData rotateObjectData);
    }
}
