using System;

namespace BlazorShared.Models.Appointment
{
    public class CreateAppointmentRequest : BaseRequest
    {
        public const string Route = "api/schedule/{ScheduleId}/appointments";

        public int PatientId { get; set; }
        public Guid ScheduleId { get; set; }
        public int AppointmentTypeId { get; set; }
        public int ClientId { get; set; }
        public int RoomId { get; set; }
        public DateTimeOffset DateOfAppointment { get; set; }
        public int SelectedDoctor { get; set; }
        public string Title { get; set; }
        public Guid InsuranceId { get; set; }
        public string InsurancePolicyNumber { get; set; }
        // public string MedicalInsuranceApprovedNumber { get;  set; } may not have it at the time of creating the appointment 
    }
}
