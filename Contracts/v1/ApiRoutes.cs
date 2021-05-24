namespace EventBookAPI.Contracts.v1
{
    public static class ApiRoutes
    {
        public const string Root = "api";
        public const string Version = "v1";
        private const string Base = Root + "/" + Version;

        public static class PageElements
        {
            public const string GetAll = Base + "/" + nameof(PageElements);
            public const string Create = Base + "/" + nameof(PageElements);
            public const string Get = Base + "/" + nameof(PageElements) + "/{pageElementId}";
        }
    }
}