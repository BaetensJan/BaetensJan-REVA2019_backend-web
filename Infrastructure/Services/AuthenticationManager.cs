using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthenticationManager(UserManager<ApplicationUser> userManager,
            IConfiguration configuration
        )
        {
            _userManager = userManager;
            _configuration = configuration;
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

        public ApplicationUser UpdateApplicationUserObject(string email, string username, string password)
        {
            return new ApplicationUser
            {
                Email = email,
                UserName = username,
                SecurityStamp = Guid.NewGuid().ToString(),
                PasswordHash = password
            };
        }

        public IEnumerable<Claim> AddClaim(ICollection<Claim> claims, string claimName, string value)
        {
            claims.Add(new Claim(claimName, value));
            return claims;
        }

        /**
        * Creating the Claim Array for a specific ApplicationUser.
        * Return: Claim[]
        */
        public async Task<Claim[]> CreateClaims(ApplicationUser user, string groupId = null)
        {
            // RESPECT THE ORDER OF THE CLAIMS, METHODS WILL GET INFO FROM CLAIMS VIA FIXED INDEXES
            // todo work with claimtypes, which makes the order unimportant
            // source: https://stackoverflow.com/questions/22246538/access-claim-values-in-controller-in-mvc-5
            var roles = await _userManager.GetRolesAsync(user);
            var isAdmin = roles.Contains("Admin");

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub,
                    user.Id),
                new Claim(ClaimTypes.Sid, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id), //todo I think this is added automatically (obsolete)
                new Claim("username", user.UserName),
                new Claim("isAdmin", isAdmin.ToString()),
            };

            if (groupId != null && roles.Contains("Group"))
            {
                claims.Add(new Claim(ClaimTypes.GroupSid, groupId));
            }
            else if (roles.Contains("Teacher"))
            {
                claims.Add(new Claim("schoolName", user.School.Name));
            }

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            return claims.ToArray();
        }

        /**
         * Create Jwt Token via a specific IEnumerable of claims.
         * Return: JwtSecurityToken
         */
        public JwtSecurityToken GetToken(IEnumerable<Claim> claim)
        {
            var signInKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["AppSettings:Secret"]));

            return new JwtSecurityToken(
                issuer: "http://app.reva.be",
                audience: "http://app.reva.be",
                claims: claim,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256));
        }

        public async Task<ApplicationUser> GetAppUserWithGroupsIncludedViaId(string applicationUserId)
        {
            return await _userManager.Users.Include(u => u.School).ThenInclude(s => s.Groups)
                .FirstOrDefaultAsync(u => u.Id == applicationUserId);
        }
        public async Task<ApplicationUser> GetAppUserWithGroupsIncludedViaUserName(string userName)
        {
            return await _userManager.Users.Include(u => u.School).ThenInclude(s => s.Groups)
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }
    }
}