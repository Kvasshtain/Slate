namespace slate.DrawingServices
{
    public record DragObjectData: IActionData
    {
        public required int Id { get; init; }
        public double Left { get; init; }
        public double Top { get; init; }
    }
}
