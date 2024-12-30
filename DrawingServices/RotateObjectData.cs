namespace DrawingServices
{
    public record RotateObjectData
    {
        public required string Id { get; init; }
        public double Angle { get; init; }
    }
}
