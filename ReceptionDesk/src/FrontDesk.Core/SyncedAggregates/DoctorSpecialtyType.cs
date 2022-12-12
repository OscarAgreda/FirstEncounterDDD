using FirstEncounterDDD.SharedKernel;
using FirstEncounterDDD.SharedKernel.Interfaces;

namespace FrontDesk.Core.SyncedAggregates
{
    public class DoctorSpecialtyType : BaseEntity<int>, IAggregateRoot
    {
        public DoctorSpecialtyType(int id, string name)
        {
            Id = id;
            Name = name;

        }

        public string Name { get; private set; }
        public string Description { get; private set; }


        public override string ToString()
        {
            return Name;
        }


    }
}
