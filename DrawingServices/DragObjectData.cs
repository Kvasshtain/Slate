namespace DrawingServices
{
    public record DragObjectData
    {
        public required string Id {get; init;}
        public double Left {get; init;}
        public double Top {get; init;}
    }
}