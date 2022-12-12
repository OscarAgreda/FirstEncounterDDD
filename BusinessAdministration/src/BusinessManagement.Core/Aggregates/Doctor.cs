using FirstEncounterDDD.SharedKernel;
using FirstEncounterDDD.SharedKernel.Interfaces;

namespace BusinessManagement.Core.Aggregates
{
    //Can't delete/update doctors . It needs a private default constructor on this entity
    public class Doctor : BaseEntity<int>, IAggregateRoot
    {
        public string Name { get; set; }


        private Doctor()
        {

        }

        public Doctor(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
