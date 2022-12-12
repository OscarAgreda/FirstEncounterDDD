using Ardalis.Specification;
using BusinessManagement.Core.Aggregates;

namespace BusinessManagement.Core.Specifications
{
  public class RoomSpecification : Specification<Room>
  {
    public RoomSpecification()
    {
      Query.OrderBy(room => room.Name);
    }
  }
}
