using FrontDesk.Core.ScheduleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FrontDesk.Infrastructure.Data.Config
{
    public class MedicalInsuranceConfiguration : IEntityTypeConfiguration<MedicalInsurance>
    {
        // <summary>
        ///the configuration of the aggregate's persistent details is done
        ///in the infrastructure  here is where every
        ///entities EF core specific mappings and configuration is performed which keeps
        ///these details out of our domain model you can see here that we're also letting
        ///EF core know that we don't want the database to supply an ID when we create
        ///a new schedule we've marked that property as value generated never
        // </summary>
        public void Configure(EntityTypeBuilder<MedicalInsurance> builder)
        {

            builder.Property(p => p.Id).ValueGeneratedNever();
            builder.Ignore(s => s.DateRange);
        }


    }
}
