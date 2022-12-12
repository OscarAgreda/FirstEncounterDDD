using FirstEncounterDDD.SharedKernel;
using FirstEncounterDDD.SharedKernel.Interfaces;

namespace FrontDesk.Core.SyncedAggregates
{
    public class Doctor : BaseEntity<int>, IAggregateRoot
    {
        public Doctor(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Name { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string EmailAddress { get; private set; }

        public string LicenseNumber { get; private set; }

        public string MobilePhoneNumber { get; private set; }

        public string SpecialtyTypeId { get; private set; }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}


// Doctor is a simple entity that the bounded context needs

// doctor inherits from base entity as well but in

// Doctor uses an Integer  for its key and the only other property it has is a

// string name property this is a minimal implementation of the

// doctor type that satisfies the scheduling bounded context it's

// essentially no more than a reference type doctor and the other similar types

// patient room Etc are all organized into this folder called synced aggregates



// let's dig a little more into how these reference types and the scheduling

// bounded context are getting their data from the  management app solution

// remember that we are using two databases one per solution

//  from seeing the class descriptions of all of these classes the

// appointment type, client, doctor, patient and room we had explicitly decided that

// these are reference entities where we're actually doing their maintenance

// elsewhere so they're not adding any unneeded complexity to the front desk

// application  and they're just read only - read only  so we're never having to create or modify them

// and reusing the INTS that were created by the database when we

// persisted them with a crud context in a different application (remember that is the our management apps that does that) ) but there's still

// but they are entities here, in the front desk app,  just entities of type integer

// the  management bounded context (Solution) is responsible for updating these types (Appintment Type, Client, Doctor, Patient and Room)

// when changes are made application events are published by Business Management (Solution) and

// this front desk bounded context (Solution) subscribes to those events and updates

// its copies of the entities


