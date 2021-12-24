namespace EventBookAPI.Contracts.v1;

public static class ApiRoutes
{
    public const string Root = "api";
    public const string Version = "v1";
    private const string Base = Root + "/" + Version;

    public static class PageElements
    {
        public const string Create = Base + "/" + nameof(PageElements);
        public const string GetAll = Base + "/" + nameof(PageElements);
        public const string Get = Base + "/" + nameof(PageElements) + "/{pageElementId}";
        public const string Update = Base + "/" + nameof(PageElements) + "/{pageElementId}";
        public const string Delete = Base + "/" + nameof(PageElements) + "/{pageElementId}";
    }

    public static class Identity
    {
        public const string Login = Base + "/" + nameof(Identity) + "/login";
        public const string Register = Base + "/" + nameof(Identity) + "/register";
        public const string Refresh = Base + "/" + nameof(Identity) + "/refresh";
    }
}