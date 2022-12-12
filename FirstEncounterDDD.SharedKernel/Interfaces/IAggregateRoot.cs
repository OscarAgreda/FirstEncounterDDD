namespace FirstEncounterDDD.SharedKernel.Interfaces
{
        // <summary>
        // Apply this marker interface only to aggregate root entities
        // Repositories will only work with aggregate roots, not their children
        //   there's nothing to it it's simply a marker it's a way that we tell
        // the compiler that our intent for a particular class or entity is that it
        // should be treated as an aggregate root we use that marker to enforce our design
        // and our encapsulation to make it so that we don't accidentally just load up a
        // child entity out of an aggregate when instead we've made a design choice that
        // we want to work with that entire aggregate as a unit
        // </summary>
        public interface IAggregateRoot { }
}
