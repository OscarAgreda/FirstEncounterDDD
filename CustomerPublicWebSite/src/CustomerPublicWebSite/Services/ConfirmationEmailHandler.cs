using Microsoft.Extensions.Logging;
using CustomerPublicWebSite.Web.Interfaces;
using CustomerPublicWebSite.Web.Models;
using MediatR;
using System.Threading.Tasks;
using System.Threading;

namespace CustomerPublicWebSite.Web.Services
{
  public class ConfirmationEmailHandler : IRequestHandler<SendAppointmentConfirmationCommand>
  {
    readonly ILogger<ConfirmationEmailHandler> _logger;
    private readonly ISendConfirmationEmails _emailSender;

    public ConfirmationEmailHandler(ILogger<ConfirmationEmailHandler> logger,
      ISendConfirmationEmails emailSender)
    {
      _logger = logger;
      _emailSender = emailSender;
    }

    public Task<Unit> Handle(SendAppointmentConfirmationCommand request, 
      CancellationToken cancellationToken)
    {
      _logger.LogInformation("Message Received - Sending Email!");

      _emailSender.SendConfirmationEmail(request);

      return Task.FromResult(Unit.Value);
    }
  }
}
