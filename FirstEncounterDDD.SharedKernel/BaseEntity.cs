using System.Collections.Generic;

namespace FirstEncounterDDD.SharedKernel
{
    // <summary>
    /// BaseEntity is an abstract plus so we can't just create a base entity
    ///object we have to create something that is a base entity such as an appointment
    ///and using generics we're saying that the base entity is going to use whatever
    ///type we ask it to and that type is for defining the ID
    ///so for appointment we said base entity is going to be using a GUID as its
    ///identity in our app we would need GUID for appointment in this
    ///context because we need to be able to create new appointments in this context
    ///and we are not going to be waiting for the database to generate that ID for us so
    ///using a GUIDS lets us create that ID right up front as we are creating that new
    ///appointment so at the same time we are giving it its ID
    // </summary>

    public abstract class BaseEntity<TId>
    {
        public TId Id { get; set; }


        // <summary>
        /// the base entity class also has a
        ///property to hold a list of domain events that will Define explicitly for each of
        ///the types that inherit from this base entity
        // </summary>
        public List<BaseDomainEvent> Events = new List<BaseDomainEvent>();
    }
}
