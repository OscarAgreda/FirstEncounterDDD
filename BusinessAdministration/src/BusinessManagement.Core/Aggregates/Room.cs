using FirstEncounterDDD.SharedKernel;
using FirstEncounterDDD.SharedKernel.Interfaces;

namespace BusinessManagement.Core.Aggregates
{
    //Can't delete/update  rooms. It needs a private default constructor on this entity
    public class Room : BaseEntity<int>, IAggregateRoot
    {
        public string Name { get; set; }

        private Room()
        {

        }

        public Room(int id, string name)
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
