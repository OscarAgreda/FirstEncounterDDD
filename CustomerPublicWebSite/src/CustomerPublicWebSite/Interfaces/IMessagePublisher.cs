using CustomerPublicWebSite.Web.Models;

namespace CustomerPublicWebSite.Web.Interfaces
{
  public interface IMessagePublisher
  {
    // for now we only need to publish one event type, so we're using its type specifically here.
    void Publish(AppointmentConfirmLinkClickedIntegrationEvent eventToPublish);
  }
}
