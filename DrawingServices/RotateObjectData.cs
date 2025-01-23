namespace slate.DrawingServices
{
    public record RotateObjectData: IActionData
    {
        public required int Id { get; init; }
        public double Angle { get; init; }
    }
}
