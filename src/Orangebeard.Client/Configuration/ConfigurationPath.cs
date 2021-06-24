namespace Orangebeard.Shared.Configuration
{
    /// <summary>
    /// Stores well known configuration property names.
    /// </summary>
    public static class ConfigurationPath
    {
        public static readonly string KeyDelimeter = ":";
        public static readonly string AppenderPrefix = "++";

        public static readonly string ServerUrl = $"Server{KeyDelimeter}Url";
        public static readonly string ServerProject = $"Server{KeyDelimeter}Project";
        public static readonly string ServerAuthenticationUuid = $"Server{KeyDelimeter}Authentication{KeyDelimeter}AccessToken";

        public static readonly string TestSetName = $"TestSet{KeyDelimeter}Name";
        public static readonly string TestSetDescription = $"TestSet{KeyDelimeter}Description";
        public static readonly string Attributes = $"TestSet{KeyDelimeter}Attributes";
    }
}
