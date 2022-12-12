using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using FrontDesk.Core.Events;
using FirstEncounterDDD.SharedKernel;
using FirstEncounterDDD.SharedKernel.Interfaces;
// me quede en 43:21

// esa imagen de Specifications
// 40:46
namespace FrontDesk.Core.ScheduleAggregate
{
    public class MedicalInsurance : BaseEntity<Guid>, IAggregateRoot
    {
        public MedicalInsurance(Guid id,

          int corporationId)
        {
            Id = Guard.Against.Default(id, nameof(id));

            CorporationId = Guard.Against.NegativeOrZero(corporationId, nameof(corporationId));
        }



        public int CorporationId { get; private set; }
        private readonly List<MedicalInsurance> _medicalInsurances = new List<MedicalInsurance>();
        public IEnumerable<MedicalInsurance> MedicalInsurances => _medicalInsurances.AsReadOnly();

        public DateTimeOffsetRange DateRange { get; private set; }

        // <summary>

        // </summary>
        //public void AddNewMedicalInsurance(MedicalInsurance medicalInsurance)
        //{
        //    Guard.Against.Null(medicalInsurance, nameof(medicalInsurance));
        //    Guard.Against.Default(medicalInsurance.Id, nameof(medicalInsurance.Id));
        //    Guard.Against.DuplicateMedicalInsurance(_medicalInsurances, medicalInsurance, nameof(medicalInsurance));

        //    _medicalInsurances.Add(medicalInsurance);

        //    MarkConflictingMedicalInsurances();

        //    //after marking any conflicts an medicalInsurance medicalInsuranced event is added to he Aggregates event collection
        //    var medicalInsuranceMedicalInsurancedEvent = new MedicalInsuranceMedicalInsurancedEvent(medicalInsurance);
        //    Events.Add(medicalInsuranceMedicalInsurancedEvent);
        //}

        // <summary>

        // </summary>
        public void DeleteMedicalInsurance(MedicalInsurance medicalInsurance)
        {
            Guard.Against.Null(medicalInsurance, nameof(medicalInsurance));
            var medicalInsuranceToDelete = _medicalInsurances
                                      .Where(a => a.Id == medicalInsurance.Id)
                                      .FirstOrDefault();

            if (medicalInsuranceToDelete != null)
            {
                _medicalInsurances.Remove(medicalInsuranceToDelete);
            }

            MarkConflictingMedicalInsurances();

            // TODO: Add medicalInsurance deleted event and show delete message in Blazor client app
        }

        // Oscar , create an event to notify the insurance company API , make a push when the medicalInsurance is created 

        // <summary>

        // </summary>
        private void MarkConflictingMedicalInsurances()
        {
            foreach (var medicalInsurance in _medicalInsurances)
            {
                //// same patient cannot have two medicalInsurances at same time
                //var potentiallyConflictingMedicalInsurances = _medicalInsurances
                //    .Where(a => a.PatientId == medicalInsurance.PatientId &&
                //    a.TimeRange.Overlaps(medicalInsurance.TimeRange) &&
                //    a != medicalInsurance)
                //    .ToList();

                //// TODO: Add a rule to mark overlapping medicalInsurances in same room as conflicting
                //// TODO: Add a rule to mark same doctor with overlapping medicalInsurances as conflicting

                //potentiallyConflictingMedicalInsurances.ForEach(a => a.IsPotentiallyConflicting = true);

                //medicalInsurance.IsPotentiallyConflicting = potentiallyConflictingMedicalInsurances.Any();
            }
        }

        // <summary>

        // </summary>
        public void MedicalInsuranceUpdatedHandler()
        {
            // TODO: Add MedicalInsuranceHandler calls to UpdateDoctor, UpdateRoom to complete additional rules described in MarkConflictingMedicalInsurances
            MarkConflictingMedicalInsurances();
        }
    }
}
