using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using Microsoft.AspNetCore.Http;

namespace ApplicationCore.Interfaces
{
    public interface IAuthenticationManager
    {
        ApplicationUser CreateApplicationUserObject(string email, string username, string password);
        ApplicationUser UpdateApplicationUserObject(string email, string username, string password);
        ICollection<Claim> AddClaim(ICollection<Claim> claims, string claimName, string value);
    }
}