namespace FirstEncounterDDD.SharedKernel
{
  // These can be overridden with configuration values but serve as defaults otherwise
  public static class MessagingConstants
  {
    public static class Credentials
    {
      public const string DEFAULT_USERNAME = "guest";
      public const string DEFAULT_PASSWORD = "guest";
    }

    public static class Exchanges
    {
      public const string FRONTDESK_BUSINESSMANAGEMENT_EXCHANGE = "frontdesk-businessmanagement";
      public const string FRONTDESK_CUSTOMERPUBLICWEBSITE_EXCHANGE = "frontdesk-customerpublicwebsite";
    }

    public static class NetworkConfig
    {
      public const int DEFAULT_PORT = 5672;
      public const string DEFAULT_VIRTUAL_HOST = "/";
    }

    public static class Queues
    {
      public const string FDCM_BUSINESSMANAGEMENT_IN = "fdcm-businessmanagement-in";
      public const string FDCM_FRONTDESK_IN = "fdcm-frontdesk-in";

      public const string FDVCP_FRONTDESK_IN = "fdvcp-frontdesk-in";
      public const string FDVCP_CUSTOMERPUBLICWEBSITE_IN = "fdvcp-customerpublicwebsite-in";
    }
  }
}
