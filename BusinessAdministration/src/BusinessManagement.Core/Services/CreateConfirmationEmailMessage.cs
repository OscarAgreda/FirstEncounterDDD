using System;
using FirstEncounterDDD.SharedKernel.Interfaces;

namespace BusinessManagement.Core.Services
{
  public class CreateConfirmationEmailMessage : IApplicationEvent
  {
    public string ClientName { get; set; }
    public string ClientEmailAddress { get; set; }
    public string PatientName { get; set; }
    public string DoctorName { get; set; }
    public string ProcedureName { get; set; }
    public DateTime AppointmentDateTime { get; set; }

    public string EventType => nameof(CreateConfirmationEmailMessage);
  }
}