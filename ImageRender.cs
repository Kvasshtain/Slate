using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;


namespace Render
{
    public class ImageRender
    {
        public void TestDraw()
        {
            int width = 640;
            int height = 480;

            using(Image<Rgba32> image = new(width, height)) 
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
                var star = new Star(x: 100.0f, y: 100.0f, prongs: 5, innerRadii: 20.0f, outerRadii:30.0f);

                image.Mutate(x=> x.Fill(options, brush, star)
                               .Draw(options, pen, star));
            
                image.Save("test.jpg");            
            }
        }
    }
}





