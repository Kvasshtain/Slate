using slate.UsersServices;

namespace BlackboardsServices
{
    public record Blackboard
    {
        public int Id { get; init; }
        public string? Name { get; init; } = null;
        public string? Description { get; init; } = null;
        public List<User> Users { get; set; } = [];
    }
}
