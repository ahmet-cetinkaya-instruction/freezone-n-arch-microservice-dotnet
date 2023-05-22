using Newtonsoft.Json;

namespace Core.EventBus.Events;

public class IntegrationEvent
{
    [JsonProperty]
    public Guid Id { get; }

    [JsonProperty]
    public DateTime CreatedDate { get; }

    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTime.UtcNow;
    }

    [JsonConstructor]
    public IntegrationEvent(Guid id, DateTime createdDate)
    {
        Id = id;
        CreatedDate = createdDate;
    }
}
