using SixLabors.ImageSharp;

namespace DrawingServices
{
    public class ImageStoreService : IBlackboardStoreService
    {
        private readonly Dictionary<string, BlackboardObjectData> images = []; // Replace by data base!!!

        // public ImageStoreService()
        // {
        //     var jpgFiles = Directory.GetFiles(@"./", "*.jpg");
        //     var jpegFiles = Directory.GetFiles(@"./", "*.jpeg");
        //     var pngFiles = Directory.GetFiles(@"./", "*.png");

        //     var imgFiles = new List<string>(jpgFiles);
        //     imgFiles.AddRange(jpegFiles);
        //     imgFiles.AddRange(pngFiles);

        //     //int zIndex = 0;
        //     int originX = 0;
        //     int originY = 0;

        //     foreach(var file in imgFiles)
        //     {
        //         ImageInfo imageInfo = SixLabors.ImageSharp.Image.Identify(file);

        //         var blackboardObject = new BlackboardObject()
        //         {
        //             Origin = new Point(originX, originY),
        //             Width = imageInfo.Width,
        //             Height = imageInfo.Height,
        //             //zIndex = zIndex++,
        //             Path = file,
        //         };

        //         originX += 100;
        //         originY += 100;

        //         blackboardObjects.Add(blackboardObject);
        //     }
        // }

        public IEnumerable<BlackboardObjectData> BlackboardObjects => images.Values;

        public bool TryAddObject(BlackboardObjectData blackboardObjectData)
        {
            ArgumentNullException.ThrowIfNull(blackboardObjectData);

            if(images.ContainsKey(blackboardObjectData.Id))
                return false;

            images.Add(blackboardObjectData.Id, blackboardObjectData);

            return true;
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
    }
}