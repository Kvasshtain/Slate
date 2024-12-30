using System.Text.Json.Serialization;
using BlackboardsServices;

namespace slate.UsersServices
{
    public record class User
    {
        public int Id { get; init; }
        public string? Email { get; init; } = null;
        public string? Password { get; init; } = null;
        public string? Name { get; init; } = null;

        [JsonIgnore]
        public List<Blackboard> Blackboards { get; set; } = [];
    }
}
