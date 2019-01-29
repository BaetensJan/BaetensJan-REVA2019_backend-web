using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Web.DTOs;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ISchoolRepository _schoolRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IAuthenticationManager _authenticationManager;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration,
            ISchoolRepository schoolRepository, IGroupRepository groupRepository,
            IAuthenticationManager authenticationManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _schoolRepository = schoolRepository;
            _groupRepository = groupRepository;
            _authenticationManager = authenticationManager;
        }

        /**
         * Create an ApplicationUser.
         * Parameter = model: CreateUserViewModel
         */
        [HttpPost("[Action]")]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserDTO model)
        {
            if (ModelState.IsValid)
            {
                var user = _authenticationManager.CreateApplicationUserObject(model.Email, model.Username,
                    model.Password);
                user = _userManager.Users.Include(u => u.School).SingleOrDefault(u => u.Id == user.Id);

                var claim = await CreateClaims(user);

                return Ok(
                    new
                    {
                        Username = user.UserName,
                        Token = GetToken(claim)
                    });
            }

            return Ok(
                new
                {
                    Message = "Error please make sure your details are correct"
                });
        }

        /**
         * Used when a teacher wants to register in the web platform.
        * Create an ApplicationUser with Role Teacher.
        * Parameter = model: CreateTeacherViewModel
        */
        [HttpPost("[Action]")]
        public async Task<ActionResult> CreateTeacher([FromBody] CreateTeacherDTO model)
        {
            if (ModelState.IsValid)
            {
                // creating the school
                var school = new School(model.SchoolName, GetRandomString(6));
                await _schoolRepository.Add(school);
                await _schoolRepository.SaveChanges();

                // creating teacher consisting of his school
                var user = _authenticationManager.CreateApplicationUserObject(model.Email, model.Username,
                    model.Password);
                user.School = await _schoolRepository.GetByName(model.SchoolName);
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    user = _userManager.Users.SingleOrDefault(u => u.Id == user.Id);
                    if (user.School == null)
                        return Ok(
                            new
                            {
                                Message = "Something went wrong when creating your account."
                            });
                    await _userManager.AddToRoleAsync(user, "Teacher");


                    var claim = await CreateClaims(user);
                    //Todo kunnen van CreateClaims in List werken idpv array zodat men niet hoeft de array naar list om te zetten
                    // en in addschoolclaim hoeft men dan geen returnwaarde te geven (want het Adden vd claim is op reference van de lijst)
                    claim = _authenticationManager.AddClaim(claim.ToList(), "school", school.Id.ToString()).ToArray();

                    var token = GetToken(claim);

                    return Ok(
                        new
                        {
//                            Username = user.UserName,
//                            Token = GetToken(claim)
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        });
                }
            }

            return Ok(
                new
                {
                    Message = "Error please make sure your details are correct"
                });
        }


        /**
         * Creating the Claim Array for a specific ApplicationUser.
         * Return: Claim[]
         */
        private async Task<Claim[]> CreateClaims(ApplicationUser user)
        {
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            return new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("username", user.UserName),
                new Claim("isAdmin", isAdmin.ToString()),
            };
        }

        /**
         * Creates a public password to login into the application.
         */
        private static string GetRandomString(int lengthOfTheNewStr)
        {
            var output = string.Empty;
            while (true)
            {
                output = output + Path.GetRandomFileName().Replace(".", string.Empty);
                if (output.Length > lengthOfTheNewStr)
                {
                    output = output.Substring(0, lengthOfTheNewStr);
                    break;
                }
            }

            return output;
        }

        /**
        * A Teacher wants to log in via web.
        * Return: JWT Token.
        */
        [HttpPost("[Action]")]
        public async Task<ActionResult> LoginWebTeacher([FromBody] LoginDTO model)
        {
            var appUser = await GetApplicationUser(model.Username);
            if (appUser == null)
                return Ok(
                    new
                    {
                        Message = "User not found"
                    });
            if (!await CheckValidPassword(appUser, model.Password)) return Unauthorized();

            // check if user is a teacher or admin.
            if (!await _userManager.IsInRoleAsync(appUser, "Admin") && !await _userManager.IsInRoleAsync(appUser, "Teacher"))
                return Unauthorized();
            var claim = await CreateClaims(appUser);
            
            // if user is Teacher, add schoolId to Claims.
            if (appUser.School != null) claim = _authenticationManager.AddClaim(claim.ToList(), "school", appUser.School.Id.ToString())
                .ToArray();
            
            //THIS IS FOR FUTURE POSSABILITY OF A GROUP LOGGING IN IN WEB, remove isInRoleAsync Teacher check above
//            // check if userLogin is from a group. (groupLogin is a concat of schoolName + groupName)
//            var schoolName = appUser.School.Name;
//            if (schoolName.Length < model.Username.Length)
//            {
//                var groupName = model.Username.Substring(schoolName.Length);
//
//                var group = await _groupRepository.GetBySchoolIdAndGroupName(appUser.School.Id, groupName);
//                // Group login
//                if (group != null)
//                    claim = _authenticationManager
//                        .AddClaim(claim.ToList(), "group", group.Id.ToString()).ToArray();
//            }

            var token = GetToken(claim);
            return Ok(
                new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
        }

        /**
         * A group wants to log in via android app.
         * Return: JWT Token.
         */
        [HttpPost("[Action]")]
        public async Task<ActionResult> LoginAndroidGroup([FromBody] LoginGroupDTO model)
        {
            var username = model.SchoolName + model.GroupName; //ApplicationUser-username is a concat of both school- and groupname.
            var appUser = await GetApplicationUser(username);
            if (appUser == null)
                return Ok(
                    new
                    {
                        Message = "User not found"
                    });
            if (!await CheckValidPassword(appUser, model.Password)) return Unauthorized();


            var group = await _groupRepository.GetBySchoolIdAndGroupName(appUser.School.Id, model.GroupName);

            if (group == null) // check if group exists
            {
                return Ok(
                    new
                    {
                        Message = "Group not found"
                    });
            }

            var claim = await CreateClaims(appUser);
            claim = _authenticationManager.AddClaim(claim.ToList(), "group", group.Id.ToString()).ToArray();

            var token = GetToken(claim);
            return Ok(
                new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
        }

        private async Task<bool> CheckValidPassword(ApplicationUser applicationUser, string password)
        {
            return await _userManager.CheckPasswordAsync(applicationUser, password);
        }

        private async Task<ApplicationUser> GetApplicationUser(string username)
        {
            return _userManager.Users.Include(u => u.School).SingleOrDefault(u => u.UserName == username);
        }

        /**
         * School Login, via Android app.
         * Return: schoolId if successful login, else an Unauthorized error.
         */
        [HttpPost("[Action]")]
        public async Task<ActionResult> LoginAndroidSchool([FromBody] LoginDTO model)
        {
            var school = await _schoolRepository.GetByName(model.Username);
            if (school == null)
            {
                Response.StatusCode = 403;
                return Unauthorized();
            }

            if (school.Password.Equals(model.Password))
                return Ok(
                    new
                    {
                        SchoolId = school.Id
                    });
            return Unauthorized();
        }

        /**
         * Check if a Username already exists.
         */
        [Route("[action]/{username}")]
        [HttpGet]
        public async Task<IActionResult> CheckUsername(string username)
        {
            var exists = await _userManager.FindByNameAsync(username) != null;
            return exists ? Ok(new {Username = "alreadyexists"}) : Ok(new {Username = "ok"});
        }

        //Todo: dit moet in de AuthenticationManager (wordt ook door groupController gebruikt)
        /**
         * Create Jwt Token via a specific IEnumerable of claims.
         * Return: JwtSecurityToken
         */
        private JwtSecurityToken GetToken(IEnumerable<Claim> claim)
        {
            var signInKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["AppSettings:Secret"]));

            return new JwtSecurityToken(
                issuer: "http://xyz.com",
                audience: "http://xyz.com",
                claims: claim,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256));
        }
    }
}