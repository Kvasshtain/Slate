namespace DrawingServices
{
    public class BlackboardObjectData
    {
        public required string Id { get; set; }
        public required string Data { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public double ScaleX { get; set; }
        public double ScaleY { get; set; }
        public double Angle { get; set; }
    }
}
