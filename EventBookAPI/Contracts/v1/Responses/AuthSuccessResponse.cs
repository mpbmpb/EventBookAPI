using System;

namespace EventBookAPI.Contracts.v1.Responses
{
    public class AuthSuccessResponse
    {
        public string Token { get; set; }
        public Guid RefreshToken { get; set; }
    }
}