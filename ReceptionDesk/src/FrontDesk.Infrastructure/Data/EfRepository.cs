using Ardalis.Specification.EntityFrameworkCore;
using FirstEncounterDDD.SharedKernel.Interfaces;

namespace FrontDesk.Infrastructure.Data
{
        // We are using the EfRepository from Ardalis.Specification
        // https://github.com/ardalis/Specification/blob/v5.1.0/ArdalisSpecificationEF/src/Ardalis.Specification.EF/RepositoryBaseOfT.cs

        // <summary>
        //  there's not much to it most of the behavior we're
        //simply inheriting from the EF repository that exists in the our
        // nuget specification package it's called repository base of t
        // however when we inherited it we were able to add additional constraints and
        // so you'll see here as well that we specify that this only works with IAggregateRoot
        // you can see the definition of the repository base of T in the our Nuget specification package which is
        // available on GitHub the details of it are shown here
        // https://github.com/ardalis/Specification/blob/main/Specification.EntityFrameworkCore/src/Ardalis.Specification.EntityFrameworkCore/RepositoryBaseOfT.cs
        // the list async method simply delegates to dbContext.Set of the appropriateT type
        // and then calls its ToListAsync passing along a cancellation token if one was provided
        // </summary>
        ///
        public class EfRepository<T> : RepositoryBase<T>, IRepository<T> where T : class, IAggregateRoot
        {


                public EfRepository(AppDbContext dbContext) : base(dbContext)
                {
                }
        }
}


// Eric Evans introduces the specification pattern in the original book on domain
// driven design although it's covered in evans's DDD Blue Book the specification
// pattern isn't listed in the book's mind map
// and honestly it doesn't get the attention it deserves factories are in
// the book's mind map but specifications aren't even though in my experience they
// play a much larger role in producing a clean domain model design
// in the book Evan says that specifications mesh smoothly with the
// repositories which are the building block mechanisms for providing query
// access to domain objects and encapsulating the interface to the
// database it's this powerful combination of
// specification and repository patterns that truly result in a clean extensible
// and testable design let's dig a little more into the specification pattern and
// how it integrates with repositories before we show you how we've implemented
// it in the front desk application specifications are used to specify the
// state of an object and as such are primarily used in three ways validation
// selection and querying and creation for a specific purpose in our app we're
// primarily leveraging specifications in our queries create explicit
// predicate-like value objects for specialized purposes a specification is
// a predicate that determines if an object satisfies some criteria according to
// Eric Evans the most basic specification simply
// provides a method typically named is Satisfied by which accepts some object
// and returns a Boolean these methods perform their logic in memory and
// unfortunately in remote data querying scenarios this approach would require
// every row to be transferred to the application before the specification
// logic could be run against it however more sophisticated
// specifications can be used in conjunction with orms like Entity
// framework core to encapsulate the details of a query while still allowing
// if core to translate the query into SQL that executes on the database server our
// sample application uses such a specification in the form of a nuget
// package Ardalis.Specification


//  recall that one of the benefits of using
// the repository pattern and abstraction was that it prevented query logic from
// being spread throughout the application this was also the reason for not
// returning I queryable from repository methods
// the same logic can be applied to repositories that accept arbitrary
// predicates since again that means the complexity of these predicates would
// need to live in the code calling the repository which might be in the user
// interface for example using repository interfaces that accept
// specifications instead of custom predicates addresses this problem very
// elegantly

// what about where generic repositories weren't suited to
// Aggregates with custom query needs so individually typed repository interfaces
// were required and each additional custom query needed to be added to this new
// specific interface well SPECIFICATIONS SOLVES THAT PROBLEM TOO, generic methods
// accepting generic specifications allows for custom queries where needed for any
// given aggregate


// a few more benefits of specifications
// -they're named classes that live in your domain model you can easily unit test
// them in isolation or if necessary integration tests them with a test
// database

// -they're highly reusable
// -they keep persistence logic out of your domain and your user interface
// -they keep business logic out of your database and persistence layer
// -they help your entities and Aggregates follow the single responsibility
// principle by keeping complex filtering or validation logic out of them


// you can easily create your own specification interface and implementation
// look here

// https://github.com/ardalis/Specification/blob/main/Specification/src/Ardalis.Specification/Specification.cs




// or you can reference that package and leverage all of its features and just
// start adding the specifications that your domain needs

// but remember that  you will need to write the specifications themselves these belong
// in your domain model

// hen you don't have many of them you
// might just put them in a root specifications folder

// however as your model grows if you're using Aggregates it may make sense to have each aggregate
// include in its own folder the specifications that go with it this
// makes them easy to locate as they grow in number
