using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using BlazorShared.Models.Appointment;
using FrontDesk.Core.SyncedAggregates;
using FrontDesk.Core.ScheduleAggregate.Specifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FirstEncounterDDD.SharedKernel;
using FirstEncounterDDD.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using FrontDesk.Core.ScheduleAggregate;

namespace FrontDesk.Api.AppointmentEndpoints
{
    public class Create : EndpointBaseAsync
      .WithRequest<CreateAppointmentRequest>
      .WithActionResult<CreateAppointmentResponse>
    {
        private readonly IRepository<Schedule> _scheduleRepository;
        private readonly IReadRepository<AppointmentType> _appointmentTypeReadRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Create> _logger;

        public Create(IRepository<Schedule> scheduleRepository,
          IReadRepository<AppointmentType> appointmentTypeReadRepository,
          IMapper mapper,
          ILogger<Create> logger)
        {
            _scheduleRepository = scheduleRepository;
            _appointmentTypeReadRepository = appointmentTypeReadRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost(template: CreateAppointmentRequest.Route)]
        [SwaggerOperation(
            Summary = "Creates a new Appointment",
            Description = "Creates a new Appointment",
            OperationId = "appointments.create",
            Tags = new[] { "AppointmentEndpoints" })
        ]
        public override async Task<ActionResult<CreateAppointmentResponse>> HandleAsync(CreateAppointmentRequest request,
          CancellationToken cancellationToken)

        {
            var response = new CreateAppointmentResponse(correlationId: request.CorrelationId());
            string policyNumber = RandomString(length: 6);
            string medicalInsuranceApprovedNumber = "RDE32f";

            Guid _medicalInsuranceId = Guid.Parse(input: "efef5654-71ff-3234-1215-fdfe451fdsdf");

            var spec = new ScheduleByIdWithAppointmentsSpec(scheduleId: request.ScheduleId); // TODO: Just get that day's appointments
            var schedule = await _scheduleRepository.FirstOrDefaultAsync(specification: spec);

            var appointmentType = await _appointmentTypeReadRepository.GetByIdAsync(id: request.AppointmentTypeId);
            var appointmentStart = request.DateOfAppointment;
            var timeRange = new DateTimeOffsetRange(start: appointmentStart, duration: TimeSpan.FromMinutes(value: appointmentType.Duration));

            var newAppointment = new Appointment(
                id: Guid.NewGuid(),
                appointmentTypeId: request.AppointmentTypeId,
                scheduleId: request.ScheduleId,
                clientId: request.ClientId,
                doctorId: request.SelectedDoctor,
                patientId: request.PatientId,
                roomId: request.RoomId,
                timeRange: timeRange,
                title: request.Title,
                insuranceId: _medicalInsuranceId,
                insurancePolicyNumber: policyNumber,
                medicalInsuranceApprovedNumber: medicalInsuranceApprovedNumber
                );

            schedule.AddNewAppointment(appointment: newAppointment);

            await _scheduleRepository.UpdateAsync(entity: schedule);
            _logger.LogInformation(message: $"Appointment created for patient {request.PatientId} with Id {newAppointment.Id}");

            var dto = _mapper.Map<AppointmentDto>(source: newAppointment);
            _logger.LogInformation(message: dto.ToString());
            response.Appointment = dto;

            return Ok(value: response);
        }
        private string RandomString(int length)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            const string pool = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var chars = Enumerable.Range(0, length)
                .Select(x => pool[random.Next(0, pool.Length)]);
            return new string(chars.ToArray());
        }
    }
}
