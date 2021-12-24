using System;
using System.Collections.Generic;

namespace EventBookAPI.Domain;

public class AuthenticationResult
{
    public string Token { get; set; }
    public Guid RefreshToken { get; set; }
    public bool Success { get; set; }
    public IEnumerable<string> Errors { get; set; }
}