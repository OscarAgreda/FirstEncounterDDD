using FirstEncounterDDD.SharedKernel;
using FirstEncounterDDD.SharedKernel.Interfaces;

namespace FrontDesk.Core.SyncedAggregates
{
    public class DoctorAssistant : BaseEntity<int>, IAggregateRoot
    {
        public DoctorAssistant(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Name { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string EmailAddress { get; private set; }

        public string Notes { get; private set; }


        public string MobilePhoneNumber { get; private set; }

        public string SpecialtyTypeId { get; private set; }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}


