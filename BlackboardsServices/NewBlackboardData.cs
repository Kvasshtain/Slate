namespace BlackboardsServices
{
    public record NewBlackboardData
    {
        public string? Name { get; init; } = null;
        public string? Description { get; init; } = null;
        public required string OwnerId { get; init; }
    }
}
