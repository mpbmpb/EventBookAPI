using System.Threading.Tasks;
using EventBookAPI.Domain;

namespace EventBookAPI.Services
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> RegisterAsync(string email, string password); 
    }
}