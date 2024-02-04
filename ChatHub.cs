using Microsoft.AspNetCore.SignalR;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using DrawingServices;
using System.Diagnostics;
using System.Numerics;
 
namespace SignalRApp
{
    public class ChatHub : Hub
    {
        private IPointService pointService;

        public ChatHub(IPointService pointService) => this.pointService = pointService ?? throw new ArgumentNullException(nameof(pointService));

        public async Task Send(DrawingAction drawingAction)
        {
            
            Debug.WriteLine($"Vector.IsHardwareAccelerated = {Vector.IsHardwareAccelerated}");

            Stopwatch stopWatch = new Stopwatch();

            stopWatch.Start();
            
            pointService.Points.Add(drawingAction);

            




            int width = 1000;
            int height = 1000;
            
            using (var stream = new MemoryStream())
            using (Image<Rgba32> image = new(width, height)) 
            {
                DrawingOptions options = new()
                {
                    GraphicsOptions = new()
                    {
                        ColorBlendingMode  = PixelColorBlendingMode.Multiply
                    }
                };
                
                PatternBrush brush = Brushes.Horizontal(Color.Red, Color.Blue);
                PatternPen pen = Pens.DashDot(Color.Green, 5);
                
                //var star = new Star(x: 100.0f, y: 100.0f, prongs: 5, innerRadii: 20.0f, outerRadii:30.0f);

                foreach (var point in pointService.Points)
                {
                    var location = new PointF((float)point.X, (float)point.Y);
                    
                    var ellipsePolygon = new EllipsePolygon(location, new SizeF(1, 1));
                    image.Mutate(x=> x/*.Fill(options, brush, ellipsePolygon)*/
                               .Draw(options, pen, ellipsePolygon));
                }


                // image.Mutate(x=> x.Fill(options, brush, star)
                //                .Draw(options, pen, star));
            
                image.SaveAsJpeg(stream);//Save("test.jpg");

                byte[] bytes = stream.ToArray();

                var data = Convert.ToBase64String(bytes);

                await this.Clients.All.SendAsync("Receive", data);
                Debug.WriteLine("Send data");         
            }




            stopWatch.Stop();

            Debug.WriteLine($"stopWatch.ElapsedMilliseconds = {stopWatch.ElapsedMilliseconds}");




            // string path = "test.jpg";
            // byte[] bytes = await File.ReadAllBytesAsync(path);  // считываем файл в массив байтов
            
            // var data = Convert.ToBase64String(bytes);

            // await this.Clients.All.SendAsync("Receive", data);
        }
    }
}