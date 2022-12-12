using FirstEncounterDDD.SharedKernel.Interfaces;

namespace BusinessManagement.Core.Interfaces
{
  public interface IMessagePublisher
  {
    void Publish(IApplicationEvent applicationEvent);
  }
}