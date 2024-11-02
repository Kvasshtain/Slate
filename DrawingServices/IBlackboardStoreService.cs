namespace DrawingServices
{
    public interface IBlackboardStoreService
    {
        IEnumerable<BlackboardObjectData> BlackboardObjects { get; }
        Task<bool> TryAddObject(BlackboardObjectData blackboardObjectData);
        Task<bool> TryDeleteObjectsByIds(string[] deletedFromCanvasObjectsIds);
        void DragObject(DragObjectData dragObjectData);
        void ScaleObject(ScaleObjectData scaleObjectData);
        void RotateObject(RotateObjectData rotateObjectData);
    }
}
