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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
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
        private readonly ITeacherRequestRepository _teacherRequestRepository;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IEmailSender _emailSender;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration,
            ISchoolRepository schoolRepository, ITeacherRequestRepository teacherRequestRepository,
            IAuthenticationManager authenticationManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _configuration = configuration;
            _schoolRepository = schoolRepository;
            _teacherRequestRepository = teacherRequestRepository;
            _authenticationManager = authenticationManager;
            _emailSender = emailSender;
        }

        [HttpGet("[Action]")]
        public async Task<IActionResult> TestMail()
        {
            await _emailSender.SendMailAsync("baetens-jan@vivaldi.net",
                "Amazon SES test (SMTP interface accessed using C#)",
                "<h1>Amazon SES Test</h1>" +
                "<p>This email was sent through the " +
                "<a href='https://aws.amazon.com/ses'>Amazon SES</a> SMTP interface " +
                "using the .NET System.Net.Mail library.</p>", new string[] { });
            return Ok("OK");
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
        [HttpGet("[Action]/{teacherRequestId}")]
        public async Task<ActionResult> CreateTeacher(int teacherRequestId)
        {
            var model = await _teacherRequestRepository.GetById(teacherRequestId);
            // creating the school
            var school = new School(model.SchoolName, GetRandomString(6));
            await _schoolRepository.Add(school);
            await _schoolRepository.SaveChanges();

            var password = GetRandomString(6);

            // creating teacher consisting of his school
            var user = _authenticationManager.CreateApplicationUserObject(model.Email, model.Email,
                password);
            user.School = await _schoolRepository.GetByName(model.SchoolName);
            var result = await _userManager.CreateAsync(user, password);

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
                //todo change email once out of sandbox (amazon)
                await _emailSender.SendMailAsync(model.Email,
                    "Uw Account voor de REVA app is hier!",
                    $@"
                        <h1>Uw Reva app account werd aangemaakt!</h1>
                        <p>
                        Uw login gegevens:
                        </p>
                        <p>
                        Gebruikersnaam: {model.Email}
                        </p>
                        <p>
                        Wachtwoord: {password}
                        </p>
                        <br>
                        <b>
                        Verander uw wachtwoord na inloggen!
                        </b>
                        <br>
                        <br>
                        <br>
                        <footer>
                        <p>Deze email werd automatisch verzonden! Reageer niet op dit bericht.</p>
                        <p>Contacteer freddy@reva.be bij problemen.</p>
                        </footer>", new string[] { });

                _teacherRequestRepository.Remove(model);
                await _teacherRequestRepository.SaveChanges();

                return Ok(
                    new
                    {
                        Message = "Teacher successfully created."
//                            Username = user.UserName,
//                            Token = GetToken(claim)
//                            token = new JwtSecurityTokenHandler().WriteToken(token),
//                            expiration = token.ValidTo
                    });
            }

            return Ok(new
            {
                Message = "Error when creating Teacher."
            });
        }


        /**
         * Creating the Claim Array for a specific ApplicationUser.
         * Return: Claim[]
         */
        private async Task<Claim[]> CreateClaims(ApplicationUser user)
        {
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub,
                    user.Id), //Todo: In case something doesn't work anymore with User, this has been changed from user.Email
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("username", user.UserName),
                new Claim("isAdmin", isAdmin.ToString()),
            };
            claims.AddRange((await _userManager.GetRolesAsync(user)).Select(role => new Claim(ClaimTypes.Role, role)));

            return claims.ToArray();
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
            if (!await _userManager.IsInRoleAsync(appUser, "Admin") &&
                !await _userManager.IsInRoleAsync(appUser, "Teacher"))
                return Unauthorized();
            var claim = await CreateClaims(appUser);

            // if user is Teacher, add schoolId to Claims.
            if (appUser.School != null)
                claim = _authenticationManager.AddClaim(claim.ToList(), "school", appUser.School.Id.ToString())
                    .ToArray();

            var token = GetToken(claim);
            return Ok(
                new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            //THIS IS FOR FUTURE POSSIBILITY OF A GROUP LOGGING IN IN WEB, remove isInRoleAsync Teacher check above
            // login could be with DNS (e.g. hogent.group1) or concat (e.g. hogentgroup1), so check if "."
//            // check if userLogin is from a group. (groupLogin is a concat of schoolName + groupName)
//            var schoolName = appUser.School.Name;
//            if (schoolName.Length < model.Username.Length)
//            {
//                var groupName = model.Username.Substring(schoolName.Length);
//
//            var group = appUser.School.Groups.SingleOrDefault(g => g.Name == groupName);
//                // Group login
//                if (group != null)
//                    claim = _authenticationManager
//                        .AddClaim(claim.ToList(), "group", group.Id.ToString()).ToArray();
//            } else{
//                // this is not a group login => schoolName characters are bigger than the login username.
// }
        }

        /**
         * A group wants to log in via android app.
         * Return: JWT Token.
         */
        [HttpPost("[Action]")]
        public async Task<ActionResult> LoginAndroidGroup([FromBody] LoginGroupDTO model)
        {
            var username =
                model.SchoolName +
                model.GroupName; //ApplicationUser-username is a concat of both school- and groupname.
            var appUser = await GetApplicationUser(username);
            if (appUser == null || !await CheckValidPassword(appUser, model.Password)) return Unauthorized();


            var group = appUser.School.Groups.SingleOrDefault(g => g.Name == model.GroupName);

            if (group == null) // check if group exists
            {
                return Unauthorized();
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
            return _userManager.Users.Include(u => u.School).ThenInclude(s => s.Groups)
                .SingleOrDefault(u => u.UserName == username);
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
                return Ok(new
                {
                    Message = "School not found"
                });
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

        [Route("[action]/{email}")]
        [HttpGet]
        public async Task<IActionResult> CheckEmail(string email)
        {
            var exists = await _userManager.FindByEmailAsync(email) != null;
            if (!exists)
                exists = await _teacherRequestRepository.GetByEmail(email) != null;
            return exists ? Ok(new {Email = "alreadyexists"}) : Ok(new {Email = "ok"});
        }

        [Route("[action]/{school}")]
        [HttpGet]
        public async Task<IActionResult> CheckSchool(string school)
        {
            var exists = await _schoolRepository.GetByName(school) != null;
            if (!exists)
                exists = await _teacherRequestRepository.GetBySchool(school) != null;
            return exists ? Ok(new {School = "alreadyexists"}) : Ok(new {School = "ok"});
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
                issuer: "http://app.reva.be",
                audience: "http://app.reva.be",
                claims: claim,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256));
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> SendQuestion([FromBody] AskQuestionDTO model)
        {
            await _emailSender.SendMailAsync(_configuration["Email:Smtp:From"],
                $"Reva App vraag: {model.Subject}",
                $@"
                        <h3>
                            Vraag van {model.Email}
                        </h3>
                        <p>
                            {model.Message}
                        </p>", new[] {model.Email});
            return Ok(true);
        }

        [HttpPost("[action]/{email}")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            return Ok(code);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> DownloadPersonalData()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            //_logger.LogInformation("User with ID '{UserId}' asked for their personal data.", _userManager.GetUserId(User));

            // Only include personal data for download
            var personalData = new Dictionary<string, string>();
            var personalDataProps = typeof(ApplicationUser).GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
            return new FileContentResult(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(personalData)),
                "text/json");
        }
    }
}