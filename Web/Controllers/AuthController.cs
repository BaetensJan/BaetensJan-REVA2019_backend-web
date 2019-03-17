using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
         * This method is used to manually Create an ApplicationUser (e.g. create admin) via e.g. Postman.
         * Parameter = model: CreateUserViewModel
         */
        [HttpPost("[Action]")]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserDTO model)
        {
            if (ModelState.IsValid)
            {
                var user = _authenticationManager.CreateApplicationUserObject(model.Email, model.Username,
                    model.Password);
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    user = _userManager.Users.SingleOrDefault(u => u.Id == user.Id);

//                    TODO: uncomment line beneath if you want to create an admin.
//                    await _userManager.AddToRoleAsync(user, "Admin");

                    user = _userManager.Users.SingleOrDefault(u => u.Id == user.Id);

                    var claim = await CreateClaims(user);

                    return Ok(
                        new
                        {
                            Username = user.UserName,
                            Token = GetToken(claim)
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
        * Used when a teacher wants to register in the web platform.
        * Create an ApplicationUser with Role Teacher.
        * Parameter = model: CreateTeacherViewModel
        */
        [HttpGet("[Action]/{teacherRequestId}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateTeacher(int teacherRequestId)
        {
            var teacherRequest = await _teacherRequestRepository.GetById(teacherRequestId);

            // creating the school
            var school = new School(teacherRequest.SchoolName, GetRandomString(8));
            await _schoolRepository.Add(school);
            await _schoolRepository.SaveChanges();

            // creating teacher consisting of his school
            var password = GetRandomString(8);
            var user = _authenticationManager.CreateApplicationUserObject(teacherRequest.Email, teacherRequest.Email,
                password);
            user.School =
                await _schoolRepository.GetByName(teacherRequest
                    .SchoolName); // todo teacherRequest should have schoolId
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return StatusCode(500, Json("Teacher creation failed."));
            }

            user = await _userManager.FindByIdAsync(user.Id);

            if (user.School == null)
            {
                return StatusCode(500);
            }

            // add Teacher to Teacher Role.
            await _userManager.AddToRoleAsync(user, "Teacher");

            // send email to teacher.
            await SendEmailToTeacher(teacherRequest, password);

            _teacherRequestRepository.Remove(teacherRequest);
            await _teacherRequestRepository.SaveChanges();

            return Ok(
                new
                {
                    login = user.UserName,
                    password
                });
        }

        /**
         * Sends an email to the Teacher when the TeacherRequest has been accepted by admin on web.
         */
        private async Task SendEmailToTeacher(TeacherRequest teacherRequest, string password)
        {
            //todo change email once out of sandbox (amazon)
            await _emailSender.SendMailAsync(teacherRequest.Email,
                "Uw Account voor de REVA app is hier!",
                $@"
                        <h1>Uw Reva app account werd aangemaakt!</h1>
                        <p>
                        Uw login gegevens:
                        </p>
                        <p>
                        Gebruikersnaam: {teacherRequest.Email}
                        </p>
                        <p>
                        Wachtwoord: {password}
                        </p>
                        <br>
                        <p>U kan met deze gegevens inloggen via volgende link: <strong>http://app.reva.be/login</strong></p>
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
            if (await _userManager.IsInRoleAsync(user, "Teacher"))
                claims.Add(new Claim("schoolName", user.School.Name));
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
            model.Username = model.Username.Trim();
            model.Password = model.Password.Trim();

            var appUser = await GetApplicationUserWithIncludes(model.Username);
            if (appUser == null)
            {
                return NotFound(Json("User not found."));
            }

            if (!await CheckValidPassword(appUser, model.Password))
            {
                return Unauthorized();
            }

            // check if user is a teacher or admin.
            if (!await _userManager.IsInRoleAsync(appUser, "Admin") &&
                !await _userManager.IsInRoleAsync(appUser, "Teacher"))
            {
                return Unauthorized(Json("Not allowed."));
            }

            var claim = await CreateClaims(appUser);

            // if user is Teacher, add schoolId to Claims.
            if (appUser.School != null)
            {
                claim = _authenticationManager.AddClaim(claim.ToList(), "school", appUser.School.Id.ToString())
                    .ToArray();
            }

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
            model.GroupName = model.GroupName.Trim();
            model.SchoolName = model.SchoolName.Trim();
            model.Password = model.Password.Trim();

            //ApplicationUser.UserName consist of (DNS): schoolName.groupName.
            var username = $"{model.SchoolName}.{model.GroupName}";
            var appUser = await GetApplicationUserWithIncludes(username);

            // check if group exists (ApplicationUser exists) and has a school
            // check if password is correct.
            // check if user is a group
            if (appUser?.School == null || !await CheckValidPassword(appUser, model.Password)
                || !await _userManager.IsInRoleAsync(appUser, "Group"))
            {
                return Unauthorized();
            }

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

        private async Task<ApplicationUser> GetApplicationUserWithIncludes(string username)
        {
            return await _userManager.Users.Include(u => u.School).ThenInclude(s => s.Groups)
                .SingleOrDefaultAsync(u => u.UserName == username);
        }

        /**
         * School Login, via Android app.
         * Return: schoolId if successful login, else an Unauthorized error.
         */
        [HttpPost("[Action]")]
        public async Task<ActionResult> LoginAndroidSchool([FromBody] LoginDTO model)
        {
            model.Username = model.Username.Trim();
            model.Password = model.Password.Trim();

            var school = await _schoolRepository.GetByName(model.Username);
            if (school == null)
            {
                return Unauthorized();
            }

            if (school.Password.Equals(model.Password))
            {
                return Ok(
                    new
                    {
                        SchoolId = school.Id
                    });
            }

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
                expires: DateTime.UtcNow.AddDays(7),
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

        [HttpPost("[action]")]
        public async Task<IActionResult> ForgotPassword([FromBody] ResetPasswordDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _emailSender.SendMailAsync(_configuration["Email:Smtp:From"],
                    $"Reva App Wachtwoord Reset",
                    $@"
                            <p>Beste,<p>
                            <p>Er werd zojuist een aanvraag gedaan om je wachtwoord te veranderen.</p>
                            <p>Als je deze aanvraag gemaakt hebt, <a href='http://app.reva.be/reset-wachtwoord?code={HtmlEncoder.Default.Encode(code)}&email={model.Email}'>Klik hier om je wachtwoord te veranderen</a></p>
                            
                            <p>Als je deze aanvraag niet gemaakt hebt, kan je deze email negeren.</p>
                    ", new[] {model.Email});
                Console.Error.WriteLine(code);
            }

            return Ok();
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            model.CurrentPassword = model.CurrentPassword.Trim();
            model.NewPassword = model.NewPassword.Trim();

            var errors = new List<string>();
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await _userManager.GetUserAsync(User);
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok();
            }
//            else
//            {
//                //todo if 3 x fails => lock user.
//            }

            foreach (var error in result.Errors)
            {
                errors.Add(error.Description);
            }

            return BadRequest(errors);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            var errors = new List<string>();
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return Ok();
            }

            Console.Error.WriteLine(model.Code);

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return Ok();
            }

            foreach (var error in result.Errors)
            {
                errors.Add(error.Description);
            }

            return BadRequest(errors);
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