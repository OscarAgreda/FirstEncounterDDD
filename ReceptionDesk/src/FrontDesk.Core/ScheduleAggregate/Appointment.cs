using System;
using Ardalis.GuardClauses;
using FrontDesk.Core.Events;
using FrontDesk.Core.SyncedAggregates;
using FirstEncounterDDD.SharedKernel;
using Microsoft.Extensions.Logging;

namespace FrontDesk.Core.ScheduleAggregate
{
    // <summary>
    ///the appointment class inherits from base entity of T which is a generic Base
    ///Class in this case it's base entity of GUID
    ///this GUID is defining the type of our identity property our ID
    ///we wanted appointment to have a good because we're creating new appointments
    ///on the Fly
    // </summary>
    public class Appointment : BaseEntity<Guid>
    {

        //!**/*.css,
        //**/*.cs,

        // <summary>
        ///with the Constructor we need to do our best to keep our domain
        ///model in a consistent state so the rest of the application can count on it being
        ///correct right because otherwise somebody could satisfy the requirement that they
        ///pass in the room ID but they might pass it in as zero which would be invalid so
        ///we're further constraining that they don't do that either the appointment
        ///would be invalid if it had a room ID that didn't correspond to an actual room
        ///entity and in any case the database wouldn't let that fly since there's a
        ///foreign key relationship between appointment and room yes but we want to
        ///make at least some effort to catch such problems in our code rather than relying
        ///on the persistence store to inform us of a user error overall using guard Clauses
        ///like the ones you've seen here help us ensure our entities aren't partially
        ///constructed and inconsistent once we've created an appointment we
        ///need to record it as part of the schedule which involves some
        ///additional Rich Behavior
        // </summary>

        public Appointment(Guid id,
        int appointmentTypeId,
        Guid scheduleId,
        int clientId,
        int doctorId,
        int patientId,
        int roomId,
        DateTimeOffsetRange timeRange,
        string title,
        Guid insuranceId, string insurancePolicyNumber,
        string? medicalInsuranceApprovedNumber = null,
        DateTime? dateTimeConfirmed = null)
        {

            Id = Guard.Against.Default(id, nameof(id));
            AppointmentTypeId = Guard.Against.NegativeOrZero(appointmentTypeId, nameof(appointmentTypeId));
            InsuranceId = Guard.Against.Default(insuranceId, nameof(insuranceId));
            InsurancePolicyNumber = Guard.Against.NullOrEmpty(insurancePolicyNumber, nameof(insurancePolicyNumber));
            ScheduleId = Guard.Against.Default(scheduleId, nameof(scheduleId));
            ClientId = Guard.Against.NegativeOrZero(clientId, nameof(clientId));
            DoctorId = Guard.Against.NegativeOrZero(doctorId, nameof(doctorId));
            PatientId = Guard.Against.NegativeOrZero(patientId, nameof(patientId));
            RoomId = Guard.Against.NegativeOrZero(roomId, nameof(roomId));
            TimeRange = Guard.Against.Null(timeRange, nameof(timeRange));
            Title = Guard.Against.NullOrEmpty(title, nameof(title));
            DateTimeConfirmed = dateTimeConfirmed;
        }

        private Appointment() { } // EF required

        public Guid ScheduleId { get; private set; }
        public int ClientId { get; private set; }
        public int PatientId { get; private set; }
        public int RoomId { get; private set; }
        public int DoctorId { get; private set; }
        public int AppointmentTypeId { get; private set; }

        public Guid InsuranceId { get; private set; }

        public DateTimeOffsetRange TimeRange { get; private set; }
        public string MedicalInsuranceApprovedNumber { get; private set; }
        public string Title { get; private set; }
        public string InsurancePolicyNumber { get; private set; }
        public DateTimeOffset? DateTimeConfirmed { get; set; }
        public bool IsPotentiallyConflicting { get; set; }

        // <summary>
        ///value objects they're not immutable so we can
        ///change them when we need to modify an appointment we're going to do that
        ///through methods and so for instance if we decide we want to modify what room in
        ///appointment is scheduled in we're going to do that through a method rather than
        ///just a Setter we do this because there's additional Behavior we may want to do in
        ///this case we have some guards again to ensure a valid value is being passed
        ///these guards are a set of reusable we also want to raise an appointment updated event
        ///that we might handle and send a notification or perform some other
        ///action as a result of what happened and that also gives us the flexibility in
        ///the future to change what type of logic we want to trigger and that's something
        ///we can't do very easily if we've just let anybody in the application set the
        ///value right by providing a method to use to update
        ///room explicitly and otherwise making the setter private we force all interaction
        ///with the model to use this method which gives us just one place to Model
        ///Behavior that should be associated with this operation
        // </summary>
        public void UpdateRoom(int newRoomId)
        {
            Guard.Against.NegativeOrZero(newRoomId, nameof(newRoomId));
            if (newRoomId == RoomId) return;

            RoomId = newRoomId;

            var appointmentUpdatedEvent = new AppointmentUpdatedEvent(this);
            Events.Add(appointmentUpdatedEvent);
        }

        // <summary>
        /// Important Note: Our bounded contexts (Business Management and front desk Solution) have separate databases
        /// synchronize changes between these two apps
        /// this is one of the simplest and most common approaches one app is responsible
        /// for updates and the other apps just subscribe to the changes and are
        /// notified when they occur this is an example of eventual consistency the two
        /// systems aren't immediately kept in sync using a transaction or something similar
        /// but through message queues eventually the different bounded contacts are
        /// updated to the new state When a change is made
        // </summary>
        public void UpdateDoctor(int newDoctorId)
        {
            Guard.Against.NegativeOrZero(newDoctorId, nameof(newDoctorId));
            if (newDoctorId == DoctorId) return;

            DoctorId = newDoctorId;

            var appointmentUpdatedEvent = new AppointmentUpdatedEvent(this);
            Events.Add(appointmentUpdatedEvent);



        }

        // <summary>
        //when the application needs to update the start time for an appointment it will
        // call this method because appointment is part of a scheduling aggregate we know
        // the app will already have loaded the schedule before calling this method so
        // the second parameter in the method asks for the Handler on the schedule that
        // will be called the call to update the schedule is made
        // after updating the time range property on the appointment so when marked
        // conflicting appointments is called it will use the new value for the time
        // range there are a lot of other ways you can
        // set up this communication using c-sharp events, static domain events, or some kind
        // of double dispatch approach they all have trade-offs and when you
        // need to do this in your apps you should choose the one that works best for your
        // app and your team
        // </summary>
        public void UpdateStartTime(DateTimeOffset newStartTime,
        Action scheduleHandler)
        {
            if (newStartTime == TimeRange.Start) return;

            TimeRange = new DateTimeOffsetRange(newStartTime, TimeSpan.FromMinutes(TimeRange.DurationInMinutes()));

            scheduleHandler?.Invoke();

            var appointmentUpdatedEvent = new AppointmentUpdatedEvent(this);
            Events.Add(appointmentUpdatedEvent);
        }

        public void UpdateTitle(string newTitle)
        {
            if (newTitle == Title) return;

            Title = newTitle;

            var appointmentUpdatedEvent = new AppointmentUpdatedEvent(this);
            Events.Add(appointmentUpdatedEvent);
        }

        public void UpdateAppointmentType(AppointmentType appointmentType,
        Action scheduleHandler)
        {
            Guard.Against.Null(appointmentType, nameof(appointmentType));
            if (AppointmentTypeId == appointmentType.Id) return;

            AppointmentTypeId = appointmentType.Id;
            TimeRange = TimeRange.NewEnd(TimeRange.Start.AddMinutes(appointmentType.Duration));

            scheduleHandler?.Invoke();

            var appointmentUpdatedEvent = new AppointmentUpdatedEvent(this);
            Events.Add(appointmentUpdatedEvent);
        }

        public void Confirm(DateTimeOffset dateConfirmed)
        {
            if (DateTimeConfirmed.HasValue) return; // no need to reconfirm

            DateTimeConfirmed = dateConfirmed;

            var appointmentConfirmedEvent = new AppointmentConfirmedEvent(this);
            Events.Add(appointmentConfirmedEvent);
        }

        // <summary>
        //frequently you're going
        //to have some health problems with that app when it comes to maintainability
        // once it gets to a certain size
        //once you have a big application of this N-Tier/N-layer Type 
        // that you may have been building it for 5 years or more and it's organized in this manner 
        // you may be already experiencing some pain and having problems like there's you know lots of sore procedures there's
        // lots of logic that's in the database there's things that you're afraid to
        // touch because you don't know what else is depending on it you've got a database
        //that's shared by multiple applications that are all using similar architecture
        //to this and so you can't even rename a table or a column without fear that it's
        // going to break some other app and yeah they're usually nodding your
        //head like yes yes those are the things right and so clean architecture if you
        // apply it the nice thing is is that you're not coupled to that low-level
        //database stuff right your application is free from it you can swap it out right
        // it's a plug-able architecture and so if you're using
        // sql server right now but in certain environments it would be nice if you
        // could use something else like MongoDb , with clean architecture you
        //can plug in those different implementations to the architecture
        //without having to change the ui or the business layer
        // </summary>
        public void CleanAppointmentDDD()
        {

        }
    }


}
