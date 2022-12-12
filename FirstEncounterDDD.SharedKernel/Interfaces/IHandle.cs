using System.Threading.Tasks;

namespace FirstEncounterDDD.SharedKernel.Interfaces
{
  public interface IHandle<T> where T : BaseDomainEvent
  {
    Task HandleAsync(T args);
  }
}
