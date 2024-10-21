using SixLabors.ImageSharp;

namespace DrawingServices
{
    public class BlackboardStoreService : IBlackboardStoreService
    {
        private readonly Dictionary<string, BlackboardObjectData> images = []; // Replace by data base!!!

        public IEnumerable<BlackboardObjectData> BlackboardObjects => images.Values;

        public bool TryAddObject(BlackboardObjectData blackboardObjectData)
        {
            ArgumentNullException.ThrowIfNull(blackboardObjectData);

            if (images.ContainsKey(blackboardObjectData.Id))
                return false;

            images.Add(blackboardObjectData.Id, blackboardObjectData);

            return true;
        }

        public bool TryDeleteObjectsByIds(string[] deletedFromCanvasObjectsIds)
        {
            ArgumentNullException.ThrowIfNull(deletedFromCanvasObjectsIds);

            bool result = true;

            foreach (var id in deletedFromCanvasObjectsIds)
            {
                if (!images.Remove(id))
                {
                    result = false;
                }
            }

            return result;
        }

        public void DragObject(DragObjectData dragObjectData)
        {
            ArgumentNullException.ThrowIfNull(dragObjectData);

            var blackboardObject = images[dragObjectData.Id];

            blackboardObject.Left = dragObjectData.Left;
            blackboardObject.Top = dragObjectData.Top;
        }

        public void ScaleObject(ScaleObjectData scaleObjectData)
        {
            ArgumentNullException.ThrowIfNull(scaleObjectData);

            var blackboardObject = images[scaleObjectData.Id];

            blackboardObject.Left = scaleObjectData.Left;
            blackboardObject.Top = scaleObjectData.Top;
            blackboardObject.ScaleX = scaleObjectData.ScaleX;
            blackboardObject.ScaleY = scaleObjectData.ScaleY;
        }

        public void RotateObject(RotateObjectData rotateObjectData)
        {
            ArgumentNullException.ThrowIfNull(rotateObjectData);

            var blackboardObject = images[rotateObjectData.Id];

            blackboardObject.Angle = rotateObjectData.Angle;
        }
    }
}
