namespace slate.DrawingServices
{
    public record ScaleObjectData: IActionData
    {
        public required int Id { get; init; }
        public double Left { get; init; }
        public double Top { get; init; }
        public double Width { get; init; }
        public double Height { get; init; }
        public double ScaleX { get; init; }
        public double ScaleY { get; init; }
    }
}
