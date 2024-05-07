namespace DrawingServices
{
    public interface IBlackboardStoreService
    {
        IEnumerable<BlackboardObjectData> BlackboardObjects { get; }
        bool TryAddObject(BlackboardObjectData blackboardObjectData);
        bool TryDeleteObjectsByIds(string[] deletedFromCanvasObjectsIds);
        void DragObject(DragObjectData dragObjectData);
        void ScaleObject(ScaleObjectData scaleObjectData);
        void RotateObject(RotateObjectData rotateObjectData);
    }
}
