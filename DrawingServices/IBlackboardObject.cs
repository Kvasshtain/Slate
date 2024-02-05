using SixLabors.ImageSharp;

namespace DrawingServices
{
    public interface IBlackboardObject
    {
        Point Origin { get; set; }
        float Width { get; set; }
        float Height { get; set; }
        int zIndex { get; set; }
        string Path { get; set; } // replace with something else
    }
}