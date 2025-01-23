using System.Diagnostics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace slate.DrawingServices
{
    public class RenderingService(
        IPointStoreService pointStoreService,
        IBlackboardStoreService imageStoreService
    ) : IRenderingService
    {
        private readonly IPointStoreService pointStoreService =
            pointStoreService ?? throw new ArgumentNullException(nameof(pointStoreService));
        private IBlackboardStoreService imageStoreService =
            imageStoreService ?? throw new ArgumentNullException(nameof(imageStoreService));

        public string Render()
        {
            //Debug.WriteLine($"Vector.IsHardwareAccelerated = {Vector.IsHardwareAccelerated}");
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            string resultData;

            int width = 1000;
            int height = 1000;

            using (var stream = new MemoryStream())
            using (Image<Rgba32> image = new(width, height))
            {
                DrawingOptions options =
                    new()
                    {
                        GraphicsOptions = new()
                        {
                            ColorBlendingMode = PixelColorBlendingMode.Multiply
                        }
                    };

                //PatternBrush brush = Brushes.Horizontal(Color.Red, Color.Blue);
                SolidPen pen = Pens.Solid(Color.Green, 5);

                //var star = new Star(x: 100.0f, y: 100.0f, prongs: 5, innerRadii: 20.0f, outerRadii:30.0f);

                // foreach (var imageData in imageStoreService.Images)
                // {
                //     using (SixLabors.ImageSharp.Image smallImage = SixLabors.ImageSharp.Image.Load(imageData.Path))
                //     {
                //         // probably it's needed to add image resizing
                //         image.Mutate(x => x
                //                      .DrawImage(smallImage, imageData.Origin, new GraphicsOptions()));
                //     }
                // }

                foreach (var point in pointStoreService.Points)
                {
                    var location = new PointF((float)point.X, (float)point.Y);

                    var ellipsePolygon = new EllipsePolygon(location, new SizeF(1, 1));
                    image.Mutate(x =>
                        x /*.Fill(options, brush, ellipsePolygon)*/
                        .Draw(options, pen, ellipsePolygon)
                    );
                }

                // image.Mutate(x=> x.Fill(options, brush, star)
                //                .Draw(options, pen, star));

                image.SaveAsJpeg(stream); //Save("test.jpg");

                byte[] bytes = stream.ToArray();

                resultData = Convert.ToBase64String(bytes);

                // string path = "test.jpg";
                // byte[] bytes = await File.ReadAllBytesAsync(path);  // считываем файл в массив байтов

                // var data = Convert.ToBase64String(bytes);

                // await this.Clients.All.SendAsync("Receive", data);
            }

            stopWatch.Stop();
            Debug.WriteLine($"stopWatch.ElapsedMilliseconds = {stopWatch.ElapsedMilliseconds}");

            return resultData;
        }

        public void TestDraw()
        {
            const int width = 640;
            const int height = 480;

            using Image<Rgba32> image = new(width, height);

            DrawingOptions options =
                new()
                {
                    GraphicsOptions = new GraphicsOptions { ColorBlendingMode = PixelColorBlendingMode.Multiply }
                };

            var brush = Brushes.Horizontal(Color.Red, Color.Blue);
            var pen = Pens.DashDot(Color.Green, 5);
            var star = new Star(
                x: 100.0f,
                y: 100.0f,
                prongs: 5,
                innerRadii: 20.0f,
                outerRadii: 30.0f
            );

            image.Mutate(x => x.Fill(options, brush, star).Draw(options, pen, star));

            image.Save("test.jpg");
        }
    }
}
