using Ardalis.Specification;
using BusinessManagement.Core.Aggregates;

namespace BusinessManagement.Core.Specifications
{
  public class ClientByIdIncludePatientsSpec : Specification<Client>, ISingleResultSpecification
  {
    public ClientByIdIncludePatientsSpec(int clientId)
    {
      Query
        .Include(client => client.Patients)
        .Where(client => client.Id == clientId);
    }
  }
}
