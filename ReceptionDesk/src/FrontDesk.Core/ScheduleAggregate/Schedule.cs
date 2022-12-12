using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using FrontDesk.Core.Events;
using FirstEncounterDDD.SharedKernel;
using FirstEncounterDDD.SharedKernel.Interfaces;
// me quede en 43:21

// esa imagen de Specifications
// 40:46
namespace FrontDesk.Core.ScheduleAggregate
{
    public class Schedule : BaseEntity<Guid>, IAggregateRoot
    {
        public Schedule(Guid id,
          DateTimeOffsetRange dateRange,
          int clinicId)
        {
            Id = Guard.Against.Default(id, nameof(id));
            DateRange = dateRange;
            ClinicId = Guard.Against.NegativeOrZero(clinicId, nameof(clinicId));
        }

        private Schedule(Guid id, int clinicId) // used by EF
        {
            Id = id;
            ClinicId = clinicId;
        }

        public int ClinicId { get; private set; }
        private readonly List<Appointment> _appointments = new List<Appointment>();
        public IEnumerable<Appointment> Appointments => _appointments.AsReadOnly();

        public DateTimeOffsetRange DateRange { get; private set; }

        // <summary>
        //adding new appointments our design forces all new
        // appointments to come through this method so we don't have to have duplicate
        // Behavior anywhere else in the application to take care of whatever
        // should happen when a new appointment is added it's all right here in one place
        // easy to understand and easy to test
        // the method validates the input to ensure
        // we're not adding bad data to our Aggregate and then it adds the
        // appointment when a new appointment is added the
        // schedule is responsible for marking any appointments that might be conflicting
        // it's the right place for this Behavior to live since the schedule knows about
        // all the appointments and knows anytime appointments are added or removed
        // </summary>
        public void AddNewAppointment(Appointment appointment)
        {
            Guard.Against.Null(appointment, nameof(appointment));
            Guard.Against.Default(appointment.Id, nameof(appointment.Id));
            Guard.Against.DuplicateAppointment(_appointments, appointment, nameof(appointment));

            _appointments.Add(appointment);

            MarkConflictingAppointments();

            //after marking any conflicts an appointment scheduled event is added to he Aggregates event collection
            var appointmentScheduledEvent = new AppointmentScheduledEvent(appointment);
            Events.Add(appointmentScheduledEvent);
        }

        // <summary>
        //the delete appointment method is similar after deleting an
        // appointment the schedule needs to once more Mark any appointments that might be conflicting
        // </summary>
        public void DeleteAppointment(Appointment appointment)
        {
            Guard.Against.Null(appointment, nameof(appointment));
            var appointmentToDelete = _appointments
                                      .Where(a => a.Id == appointment.Id)
                                      .FirstOrDefault();

            if (appointmentToDelete != null)
            {
                _appointments.Remove(appointmentToDelete);
            }

            MarkConflictingAppointments();

            // TODO: Add appointment deleted event and show delete message in Blazor client app
        }



        // <summary>
        // this method is responsible for detecting and
        // marking appointments that might conflict the basic rules shown here just checks
        // whether the patient has two appointments that overlap
        // if any such appointments are found they are updated to set their conflicting
        // property to true then the current appointments property
        // is set based on whether there are any other appointments that conflict with it
        // this is an important part of the business Logic for this application and
        // it's encapsulated right in our schedule aggregate in a lot of data-driven
        // applications this kind of logic might be in a stored procedure or perhaps just
        // implemented in the user interface but in a domain-driven application we want
        // these rules to be explicit and defined in our domain model
        // </summary>
        private void MarkConflictingAppointments()
        {
            foreach (var appointment in _appointments)
            {
                // same patient cannot have two appointments at same time
                var potentiallyConflictingAppointments = _appointments
                    .Where(a => a.PatientId == appointment.PatientId &&
                    a.TimeRange.Overlaps(appointment.TimeRange) &&
                    a != appointment)
                    .ToList();

                // TODO: Add a rule to mark overlapping appointments in same room as conflicting
                // TODO: Add a rule to mark same doctor with overlapping appointments as conflicting

                potentiallyConflictingAppointments.ForEach(a => a.IsPotentiallyConflicting = true);

                appointment.IsPotentiallyConflicting = potentiallyConflictingAppointments.Any();
            }
        }

        // <summary>
        /// Call any time this schedule's appointments are updated directly
        /// provides a hook for its appointments to use to notify it when
        // changes are made to one of them because we don't have navigation properties from
        // appointment back to schedule we can't directly call methods on the aggregate
        // root from appointment methods there are a few different patterns you can use to
        // accomplish this task for this sample we chose this one because it's simple and
        // easy to follow this Handler simply calls Mark conflicting appointments but it's
        // exposed as its own separate method because it could do other things as well
        // and we don't want to expose the internal behavior of the schedule to the rest of the app
        // </summary>
        public void AppointmentUpdatedHandler()
        {
            // TODO: Add ScheduleHandler calls to UpdateDoctor, UpdateRoom to complete additional rules described in MarkConflictingAppointments
            MarkConflictingAppointments();
        }
    }
}





// the schedule aggregate
// appointments no longer need to know anything about other appointments the responsibility for
// ensuring that appointments are not double booked and similar invariants can
// be performed by the schedule which is the aggregate root
// schedule will certainly help us ensure that appointments don't overlap one
// another when we save changes to a schedule does it make sense to update
// any changed appointments yes it does make sense and if we were to delete an
// entire schedule would it make sense to delete all of its appointments. yeah
//  that would make sense also
//   this is the schedule for a particular clinic at the moment we only have one clinic but if we imagine a
// scenario in which multiple clinics (multi tenant) each have their own schedule it wouldn't make
// sense to delete a Clinic's schedule but then keep its appointments floating
// around .And if a schedule exists for each Clinic
// then it makes sense to persist the schedule which means that it needs an ID
// and therefore is truly an entity and when we retrieve a schedule we'll
// most likely be filtering which related appointments we want to look at for
// example today's schedule or this week's schedule that would mean we want all of
// today's or all of this week's appointments from a particular Clinic's schedule
// it really makes more sense to tie the appointments to a schedule rather than connect the appointment directly
// to a clinic now let's see
// how this affects our design
// now , how the schedule aggregate gets implemented in our application
// the schedule aggregate folder only includes schedule and appointment as
// well as related guards and specifications , and for your larger applications it
// can help to organize your domain model by grouping everything related to a
// particular aggregate in its folder looking at the schedule Aggregates code, here as a guide on how we have done it
// you can see that it inherits from our common base entity type and uses a GUID
// for its ID key just like appointment this lets us set the key ourselves
// rather than relying on a database to do it for us
// the class is also marked as an aggregate root with an interface
// The aggregate root interface is used to protect the Integrity of our Aggregates
// note that is also very important  in our repository and specification
// implementations next the schedules Constructor just
// takes in its ID, its date range, and its Associated Clinic ID,
// for this sample the clinic ID is always hard coded but in a real multi tenant application might be several
// clinics using the same software and they would each have their own IDs
// The Constructor is responsible for ensuring that the incoming values are
// valid so that it's always created in a consistent State schedule has just a few
// properties there is the clinic ID, the associated set (List) of appointments and the date range
//  note that we're careful to only expose a read-only iEnumerable of
// appointments because our aggregate must encapsulate its internal State we don 't
// want other parts of our application to add or delete appointments without going
// through the schedule's explicit methods designed for this purpose
// also the date range isn't persisted since it can vary with any given instantiation of the schedule
// and for performance reasons you
// wouldn't really want to load the schedule aggregate with every
// appointment that had ever been made included in it by using a property DateRange
//  we make it clear to the rest of the domain what set of dates this instance
// of the aggregate holds the actual population of the appointments that
// match this range is left as a responsibility of the
// SPECIFICATIONS  and repository classes that are used to retrieve the schedule from the database