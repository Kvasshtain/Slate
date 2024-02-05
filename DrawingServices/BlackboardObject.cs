using SixLabors.ImageSharp;

namespace DrawingServices
{
    public class BlackboardObject : IBlackboardObject
    {
        public Point Origin { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public int zIndex { get; set; }
        public string Path { get; set; } // replace with something else
    }
}