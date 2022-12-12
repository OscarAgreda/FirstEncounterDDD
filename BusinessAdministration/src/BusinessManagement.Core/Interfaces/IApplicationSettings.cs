using System;

namespace BusinessManagement.Core.Interfaces
{
  public interface IApplicationSettings
  {
    int ClinicId { get; }
    DateTime TestDate { get; }
  }
}