using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FrontDesk.Core.ScheduleAggregate;
using FrontDesk.Core.SyncedAggregates;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FirstEncounterDDD.SharedKernel;

// make sure package Microsoft.EntityFrameworkCore.Design
//--RUN THIS FROM FrontDesk.Api project folder   << ---------

//-c	The DbContext class to use. Class name only or fully qualified with namespaces. If this option is omitted, EF Core will find the context class. If there are multiple context classes, this option is required.
//-p	Relative path to the project folder of the target project. Default value is the current folder.
//-s	Relative path to the project folder of the startup project. Default value is the current folder.
//-o	The directory to put files in. Paths are relative to the project directory.


// to drop the database
//dotnet ef database drop -c appdbcontext -p ../FrontDesk.Infrastructure/FrontDesk.Infrastructure.csproj -f -v

//dotnet ef migrations add initialFrontDeskAppMigration -c appdbcontext -p ../FrontDesk.Infrastructure/FrontDesk.Infrastructure.csproj -s FrontDesk.Api.csproj -o Data/Migrations

//dotnet ef database update -c appdbcontext --project ../FrontDesk.Infrastructure/FrontDesk.Infrastructure.csproj -s FrontDesk.Api.csproj

// then look at AppDbContextSeed


namespace FrontDesk.Infrastructure.Data
{
    // <summary>
    // This is our own AppDeContext
    //  we Define the DbSets that we're working with and
    // we also pass in some additional configuration one thing to notice and
    // take away from this example is how many places in our solution we have to
    // reference AppDeContext or Entity Framework it's almost nowhere in the
    // entire code base the only place that we talk about it at all is inside of AppDeContext,
    // EFrepository and some related folders such as configuration and
    // migrations everywhere else and especially in our domain model we're
    // completely persistence ignorant relying only on abstractions that we've defined
    // </summary>
    public class AppDbContext : DbContext
    {
        private readonly IMediator _mediator;

        public AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentType> AppointmentTypes { get; set; }
        public DbSet<DoctorSpecialtyType> DoctorSpecialtyTypes { get; set; }
        public DbSet<DoctorAssistant> DoctorAssistants { get; set; }
        public DbSet<MedicalInsurance> MedicalInsurances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        // TODO: Use DbContext.SavedChanges event and handler to support events
        // https://docs.microsoft.com/en-us/ef/core/logging-events-diagnostics/events
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            // ignore events if no dispatcher provided
            if (_mediator == null) return result;

            var entitiesWithEvents = ChangeTracker
                .Entries()
                .Select(e => e.Entity as BaseEntity<Guid>)
                .Where(e => e?.Events != null && e.Events.Any())
                .ToArray();

            foreach (var entity in entitiesWithEvents)
            {
                var events = entity.Events.ToArray();
                entity.Events.Clear();
                foreach (var domainEvent in events)
                {
                    await _mediator.Publish(domainEvent).ConfigureAwait(false);
                }
            }

            return result;
        }

        public override int SaveChanges()
        {
            return SaveChangesAsync().GetAwaiter().GetResult();
        }
    }
}
