using System;
using System.Collections.Generic;
using FirstEncounterDDD.SharedKernel;

namespace FrontDesk.Core.ValueObjects
{
    public class AnimalType : ValueObject
    {
        public string Species { get; private set; }
        public string Breed { get; private set; }

        private AnimalType()
        {
            // EF
        }
        public AnimalType(string species, string breed)
        {
            Species = species;
            Breed = breed;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Breed;
            yield return Species;
        }

        // <summary>
        //you can use records  for value objects
        // but there's some downsides to them uh value objects should be
        // immutable and records but you can make a copy of them using the with keyword that can bypass
        //some of your validation if you have that inside your constructor 
        //like that so that is why we prefer to use a base class and actually use classes for the
        // value objects but but records are continuing to evolve and we think we'll
        // be able to get the the protection that we want for value
        // objects to be able to make sure that not only are they immutable but also there's
        //no way to construct one that bypasses the constructor and the rules that are
        // in it uh in hopefully a future version of c sharp 
        // </summary>
        public void CleanAnimalTypeDDD()
        {

        }
    }
}
