using CustomerPublicWebSite.Web.Models;

namespace CustomerPublicWebSite.Web.Interfaces
{
  public interface ISendConfirmationEmails
    {
        void SendConfirmationEmail(SendAppointmentConfirmationCommand appointment);
    }
}
