using System;
using MediatR;

namespace FirstEncounterDDD.SharedKernel
{
  public abstract class BaseDomainEvent : INotification
  {
    public DateTimeOffset DateOccurred { get; protected set; } = DateTimeOffset.UtcNow;
  }
}
