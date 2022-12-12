using System;
using Microsoft.AspNetCore.Mvc;
using CustomerPublicWebSite.Web.Interfaces;
using CustomerPublicWebSite.Web.Models;

namespace CustomerPublicWebSite.Web.Controllers
{
  /// <summary>
  /// This controller is only used to confirm appointments. Its URL
  /// is provided in the email sent to the Client.
  /// </summary>
  public class AppointmentController : Controller
  {
    private readonly IMessagePublisher _messagePublisher;

    public AppointmentController(IMessagePublisher messagePublisher)
    {
      _messagePublisher = messagePublisher;
    }

    [HttpGet("appointment/confirm/{id}")]
    public ActionResult Confirm(Guid id)
    {
      var appEvent = new AppointmentConfirmLinkClickedIntegrationEvent(id);
      _messagePublisher.Publish(appEvent);
      return View();
    }
  }
}
