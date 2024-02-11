using SixLabors.ImageSharp;

namespace DrawingServices
{
    public class ImageStoreService : IImageStoreService
    {
        private List<Image> images = new List<Image>();

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

        public List<Image> Images => images;
    }
}