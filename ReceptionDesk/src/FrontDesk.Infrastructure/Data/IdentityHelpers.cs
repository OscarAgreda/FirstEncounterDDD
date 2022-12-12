using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FrontDesk.Infrastructure.Data
{
  public static class IdentityHelpers
  {
    //https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-7.0&tabs=visual-studio
    //https://learn.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-7.0
    public static Task EnableIdentityInsert<T>(this DbContext context) => SetIdentityInsert<T>(context, enable: true);
    public static Task DisableIdentityInsert<T>(this DbContext context) => SetIdentityInsert<T>(context, enable: false);

    private static Task SetIdentityInsert<T>(DbContext context, bool enable)
    {
      var entityType = context.Model.FindEntityType(typeof(T));
      var value = enable ? "ON" : "OFF";
      return context.Database.ExecuteSqlRawAsync(
          $"SET IDENTITY_INSERT {entityType.GetSchema()}.{entityType.GetTableName()} {value}");
    }

    public static async Task SaveChangesWithIdentityInsert<T>(this DbContext context)
    {
      if (!context.IsRealDatabase())
      {
        await context.SaveChangesAsync();
        return;
      }

      using var transaction = context.Database.BeginTransaction();
      await context.EnableIdentityInsert<T>();
      await context.SaveChangesAsync();
      await context.DisableIdentityInsert<T>();
      transaction.Commit();
    }

    public static bool IsRealDatabase(this DbContext context)
    {
      return context.Database.ProviderName.Contains("SqlServer");
    }

  }
}
