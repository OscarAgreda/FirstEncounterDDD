using FirstEncounterDDD.SharedKernel.Interfaces;

namespace BusinessManagement.Api.ApplicationEvents
{
  public class NamedEntityUpdatedEvent : IApplicationEvent
  {
    public string EventType { get; set; }
    public NamedEntity Entity { get; set; }

    public NamedEntityUpdatedEvent(NamedEntity entity, string eventType)
    {
      Entity = entity;
      EventType = eventType;
    }
  }
}
