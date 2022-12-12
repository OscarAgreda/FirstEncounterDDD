using System.Threading.Tasks;

namespace BusinessManagement.Core.Interfaces
{
  public interface ITokenClaimsService
  {
    Task<string> GetTokenAsync(string userName);
  }
}
