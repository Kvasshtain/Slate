using slate.DrawingServices;
using slate.UsersServices;

namespace slate.BlackboardsServices
{
    public record Blackboard
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }
        public List<User> Users { get; init; } = [];
        public List<BlackboardObjectData> BlackboardObjects { get; init; } = [];
    }
}
