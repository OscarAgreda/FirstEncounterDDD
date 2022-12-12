using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BusinessManagement.Core.Aggregates;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FirstEncounterDDD.SharedKernel;


// make sure package Microsoft.EntityFrameworkCore.Design
//--RUN THIS FROM BusinessManagement.Api project folder   << ---------

//-c	The DbContext class to use. Class name only or fully qualified with namespaces. If this option is omitted, EF Core will find the context class. If there are multiple context classes, this option is required.
//-p	Relative path to the project folder of the target project. Default value is the current folder.
//-s	Relative path to the project folder of the startup project. Default value is the current folder.
//-o	The directory to put files in. Paths are relative to the project directory.


// to drop the database
//dotnet ef database drop -c appdbcontext -p ../BusinessManagement.Infrastructure/BusinessManagement.Infrastructure.csproj -f -v

//dotnet ef migrations add initialClinicAppMigration -c appdbcontext -p  ../BusinessManagement.Infrastructure/BusinessManagement.Infrastructure.csproj -s BusinessManagement.Api.csproj -o Data/Migrations

//dotnet ef database update -c appdbcontext --project ../BusinessManagement.Infrastructure/BusinessManagement.Infrastructure.csproj -s BusinessManagement.Api.csproj

// then look at AppDbContextSeed

namespace BusinessManagement.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IMediator _mediator;

        public AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<AppointmentType> AppointmentTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

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
