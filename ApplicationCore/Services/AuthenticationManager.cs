using System;
using System.Collections.Generic;
using System.Security.Claims;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Services
{
    public class AuthenticationManager : IAuthenticationManager
    {
//        private readonly  _exhibitorRepository;

        public AuthenticationManager( /*IExhibitorRepository exhibitorRepository*/)
        {
            //_exhibitorRepository = exhibitorRepository;
        }

        public ApplicationUser CreateApplicationUserObject(string email, string username, string password)
        {
            return new ApplicationUser
            {
                Email = email,
                UserName = username,
                SecurityStamp = Guid.NewGuid().ToString(),
                PasswordHash = password
            };
        }

//        public JwtSecurityToken GetToken(IEnumerable<Claim> claim)
//        {
//            var signInKey = new SymmetricSecurityKey(
//                Encoding.UTF8.GetBytes(_configuration["AppSettings:Secret"]));
//
//            return new JwtSecurityToken(
//                issuer: "http://xyz.com",
//                audience: "http://xyz.com",
//                claims: claim,
//                expires: DateTime.UtcNow.AddHours(1),
//                signingCredentials: new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256));
//        }

        public ICollection<Claim> AddClaim(ICollection<Claim> claims, string claimName, string value)
        {
            claims.Add(new Claim(claimName, value));
            return claims;
        }

//        private async Task<Claim[]> CreateClaims(ApplicationUser user)
//        {
//            return new[]
//            {
//                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
//                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
//                new Claim(ClaimTypes.NameIdentifier, user.Id),
//                new Claim("username", user.UserName),
//                new Claim("isAdmin", isAdmin.ToString()),
//            };
//        }
    }
}