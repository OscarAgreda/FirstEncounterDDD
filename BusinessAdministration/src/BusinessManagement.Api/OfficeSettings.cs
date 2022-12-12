using System;
using BusinessManagement.Core.Interfaces;

namespace BusinessManagement.Api
{
  public class OfficeSettings : IApplicationSettings
  {
    public int ClinicId { get { return 1; } }
    public DateTime TestDate { get { return new DateTime(2030, 9, 23); } }
  }
}
