namespace EventBookAPI.Contracts.v1
{
    public static class ApiRoutes
    {
        public const string Root = "api";
        public const string Version = "v1";
        private const string Base = Root + "/" + Version;

        public static class Paragraphs
        {
            public const string GetAll = Base + "/" + nameof(Paragraphs);
            public const string Create = Base + "/" + nameof(Paragraphs);
            public const string Get = Base + "/" + nameof(Paragraphs) + "/{paragraphId}";
        }
    }
}