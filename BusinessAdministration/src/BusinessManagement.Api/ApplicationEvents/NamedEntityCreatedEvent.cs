using FirstEncounterDDD.SharedKernel.Interfaces;

namespace BusinessManagement.Api.ApplicationEvents
{
  public class NamedEntityCreatedEvent : IApplicationEvent
  {
    public string EventType { get; set; }
    public NamedEntity Entity { get; set; }

    public NamedEntityCreatedEvent(NamedEntity entity, string eventType)
    {
      Entity = entity;
      EventType = eventType;
    }
  }
}
