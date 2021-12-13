using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EventBookAPI.Data;
using EventBookAPI.Domain;
using EventBookAPI.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EventBookAPI.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly DataContext _dataContext;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly UserManager<IdentityUser> _userManager;

        public IdentityService(UserManager<IdentityUser> userManager, JwtSettings jwtSettings,
            TokenValidationParameters tokenValidationParameters, DataContext dataContext)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _dataContext = dataContext;
        }

        public async Task<AuthenticationResult> RegisterAsync(string email, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser is not null)
                return new()
                {
                    Errors = new[] {"User with this email address already exists"}
                };

            var newUser = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                UserName = email
            };
            
            var createdUser = await _userManager.CreateAsync(newUser, password);

            if (!createdUser.Succeeded)
                return new()
                {
                    Errors = createdUser.Errors.Select(x => { return x.Description; })
                };
            
            var result = await _userManager.AddClaimAsync(newUser, new Claim("delete.enabled", "true"));

            return await GenerateAuthenticationResultAsync(newUser);
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return new()
                {
                    Errors = new[] {"User does not exist"}
                };

            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);

            if (!userHasValidPassword)
                return new()
                {
                    Errors = new[] {"User/password combination is not correct"}
                };

            return await GenerateAuthenticationResultAsync(user);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, Guid refreshToken)
        {
            var claimsPrincipal = getPrincipalFromToken(token);

            if (claimsPrincipal is null)
                return new() {Errors = new[] {"Invalid Token"}};

            var expirationDateUnix =
                long.Parse(claimsPrincipal.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expirationDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expirationDateUnix);

            if (expirationDateTimeUtc > DateTime.UtcNow)
                return new() {Errors = new[] {"This token hasn't expired yet"}};

            var jti = claimsPrincipal.FindFirst(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

            var storedRefreshToken =
                await _dataContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);

            if (storedRefreshToken is null)
                return new() {Errors = new[] {"This refresh token does not exist"}};

            if (DateTime.UtcNow > storedRefreshToken.ExpirationDate)
                return new() {Errors = new[] {"This refresh token has expired"}};

            if (storedRefreshToken.Invalidated)
                return new() {Errors = new[] {"This refresh token has been invalidated"}};

            if (storedRefreshToken.Used)
                return new() {Errors = new[] {"This refresh token has been used"}};

            if (storedRefreshToken.JwtId != jti)
                return new() {Errors = new[] {"This refresh token does not match this JWT"}};

            storedRefreshToken.Used = true;
            _dataContext.RefreshTokens.Update(storedRefreshToken);
            await _dataContext.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(claimsPrincipal.FindFirst("id")?.Value);
            return await GenerateAuthenticationResultAsync(user);
        }

        private ClaimsPrincipal getPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var tokenValidationParameters = _tokenValidationParameters.Clone();
                tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                return IsJwtWithValidSecurityAlgorithm(validatedToken) ? principal : null;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return validatedToken is JwtSecurityToken jwtSecurityToken &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultAsync(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("id", user.Id),
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            
            claims.AddRange(userClaims);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                SigningCredentials = new(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.Add(_jwtSettings.RefreshTokenLifetime)
            };

            await RemoveOutdatedTokensAsync();
            await _dataContext.RefreshTokens.AddAsync(refreshToken);
            await _dataContext.SaveChangesAsync();

            return new()
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            };
        }

        private async Task RemoveOutdatedTokensAsync()
        {
            var oldRefreshTokensToRemove = _dataContext.RefreshTokens
                .Where(t => t.ExpirationDate < DateTime.UtcNow);
            _dataContext.RefreshTokens.RemoveRange(oldRefreshTokensToRemove);

            await _dataContext.SaveChangesAsync();
        }
    }
}