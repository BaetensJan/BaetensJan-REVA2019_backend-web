using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface IAuthenticationManager
    {
        ApplicationUser CreateApplicationUserObject(string email, string username, string password);
        ApplicationUser UpdateApplicationUserObject(string email, string username, string password);
        IEnumerable<Claim> AddClaim(ICollection<Claim> claims, string claimName, string value);
        Task<Claim[]> CreateClaims(ApplicationUser user, string groupId = null);
        JwtSecurityToken GetToken(IEnumerable<Claim> claim);
        Task<ApplicationUser> GetAppUserWithGroupsIncludedViaId(string applicationUserId);
        Task<ApplicationUser> GetAppUserWithGroupsIncludedViaUserName(string userName);
        Task<ApplicationUser> GetAppUserWithSchoolIncludedViaUserName(string userName);
        Task<ApplicationUser> GetAppUserWithSchoolIncludedViaId(string applicationUserId);
    }
}