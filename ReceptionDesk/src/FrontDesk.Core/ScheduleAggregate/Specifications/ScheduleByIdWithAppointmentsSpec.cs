using System;
using Ardalis.Specification;

namespace FrontDesk.Core.ScheduleAggregate.Specifications
{
        // <summary>
        /// // each specification class is a value object so it should be immutable
        // generally they do all of their work in their Constructor any variable part of
        // the specification should be supplied as a Constructor argument and once
        // constructed the specification needs to be supplied to your query implementation
        // you can use specifications directly with ef core or you can use a repository
        // abstraction that supports them in either case pass the specification to the query
        // object and it will be used to build the query which is then executed and results
        // are returned the resulting code for most queries turns into one line to create
        // the specification and another line to execute the query by passing the
        // specification to a repository or a DBContext method
        // </summary>

        public class ScheduleByIdWithAppointmentsSpec : Specification<Schedule>, ISingleResultSpecification
        {
                public ScheduleByIdWithAppointmentsSpec(Guid scheduleId)
                {
                        Query
                          .Where(schedule => schedule.Id == scheduleId)
                          .Include(schedule => schedule.Appointments); // NOTE: Includes *all* appointments
                }
        }
}


