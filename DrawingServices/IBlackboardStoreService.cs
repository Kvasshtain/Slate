namespace DrawingServices
{
    public interface IBlackboardStoreService
    {
        IEnumerable<BlackboardObjectData> BlackboardObjects { get; }
        Task<bool> TryAddObject(BlackboardObjectData blackboardObjectData);
        Task<bool> TryDeleteObjectById(string deletedFromCanvasObjectId);
        Task<bool> DragObject(DragObjectData dragObjectData);
        Task<bool> ScaleObject(ScaleObjectData scaleObjectData);
        Task<bool> RotateObject(RotateObjectData rotateObjectData);
    }
}
