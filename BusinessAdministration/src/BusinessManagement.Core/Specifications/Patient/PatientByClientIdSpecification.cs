using Ardalis.Specification;
using BusinessManagement.Core.Aggregates;

namespace BusinessManagement.Core.Specifications
{
  public class PatientByClientIdSpecification : Specification<Patient>
  {
    public PatientByClientIdSpecification(int clientId)
    {
      Query
          .Where(patient => patient.ClientId == clientId);

      Query.OrderBy(patient => patient.Name);
    }
  }
}
