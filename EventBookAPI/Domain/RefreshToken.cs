using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EventBookAPI.Domain;

public class RefreshToken
{
    [Key] public Guid Token { get; set; }

    public string JwtId { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool Used { get; set; }
    public bool Invalidated { get; set; }
    public string UserId { get; set; }

    public IdentityUser User { get; set; }
}