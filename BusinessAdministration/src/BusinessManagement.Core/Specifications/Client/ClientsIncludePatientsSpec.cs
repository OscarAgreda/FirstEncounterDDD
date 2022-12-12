using Ardalis.Specification;
using BusinessManagement.Core.Aggregates;

namespace BusinessManagement.Core.Specifications
{
  public class ClientsIncludePatientsSpec : Specification<Client>
  {
    public ClientsIncludePatientsSpec()
    {
      Query
        .Include(client => client.Patients)
        .OrderBy(client => client.FullName);
    }
  }
}
