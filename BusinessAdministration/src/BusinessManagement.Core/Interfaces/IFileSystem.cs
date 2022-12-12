using System.Threading.Tasks;

namespace BusinessManagement.Core.Interfaces
{
  public interface IFileSystem
  {
    Task<bool> SavePicture(string pictureName, string pictureBase64);
  }
}
