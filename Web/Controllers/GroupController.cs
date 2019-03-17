using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Web.DTOs;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : Controller
    {
        private readonly IGroupRepository _groupRepository;
        private readonly ISchoolRepository _schoolRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly GroupManager _groupManager;
        private readonly IConfiguration _configuration;

        public GroupController(IGroupRepository repository, UserManager<ApplicationUser> userManager,
            IAuthenticationManager authenticationManager, ISchoolRepository schoolRepository,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _groupManager = new GroupManager(configuration);
            _groupRepository = repository;
            _authenticationManager = authenticationManager;
            _schoolRepository = schoolRepository;
            _configuration = configuration;
        }


        /**
        * Returns group status/info to android (current assignment, number of assignments done by group,
        * if group already has an assignment and the coordinates of the previous exhibitor if existing).
        * Gets groupId via token -> groupId
        */
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GroupInfo()
        {
            string claim;
            int groupId;

            try
            {
                // get groupId from jwt token.
                claim = User.Claims.ElementAt(5).Value;
            }
            catch (IndexOutOfRangeException e)
            {
                // token has less than 6 items and does not have the groupId Claim.
                return StatusCode(500, $"Token does not consist of a groupId. {e}");
            }

            try
            {
                groupId = Convert.ToInt32(claim);
            }
            catch (FormatException formatException)
            {
                // System.FormatException: Input string was not in a correct format.
                return StatusCode(500, $"Token does not consist of a groupId. {formatException}");
            }

            var group = await _groupRepository.GetById(groupId);
            if (group == null) return Ok(new {Message = "Group not found or groupId not in token."});
            var groupInfo = _groupManager.GetGroupInfo(group);
            return groupInfo;
        }

        /**
         * return group with id equal to parameter groupId.
         */
        [HttpGet("[action]/{groupId}")]
        public async Task<Group> Group(int groupId)
        {
            return await _groupRepository.GetById(groupId);
        }

        /**
         * Returns all groups of school with schoolId equal to parameter schoolId.
         */
        [HttpGet("[action]/{schoolId}")]
        public async Task<IActionResult> Groups(int schoolId)
        {
            var school = await _schoolRepository.GetById(schoolId);
            if (school != null)
            {
                return Ok(school.Groups);
            }

            return Ok(new
            {
                Message = "School not found"
            });
        }


        /**
         * Returns group with schoolId equal to parameter schoolId and groupName equal to parameter.
         * If parameter schoolId is -1, then the schoolId can be extracted out of the token via User.claims.
         */
        [HttpGet("[action]/{schoolId}/{groupName}")]
        public async Task<IActionResult> GetBySchoolIdAndGroupName(int schoolId, string groupName)
        {
            var school = await _schoolRepository.GetById(schoolId);
            if (school == null)
                return Ok(new
                {
                    Message = "School not found"
                });
            var group = school.Groups.SingleOrDefault(g => g.Name == groupName);
            return group == null
                ? Ok(new {Message = "Group not found in school with school name: " + school.Name})
                : Ok(group);
        }

        /**
         * Checks if the groupName already exists.
         */
        [HttpGet("[action]/{schoolId}/{groupName}")]
        public async Task<IActionResult> CheckGroupName(int schoolId, string groupName)
        {
            var school = await _schoolRepository.GetById(schoolId);
            if (school == null) return Ok(new {GroupName = "School not found"});
            var foundGroup = "alreadyexists";
            if (school.Groups.FindIndex(g => g.Name.ToLower().Equals(groupName.ToLower())) < 0)
                foundGroup = "ok";
            return Ok(new {GroupName = foundGroup});
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<Group>> Groups()
        {
            return await _groupRepository.GetAllLight();
        }

        /**
         * Creates a Group and returns a JwtToken.
         */
        [HttpPost("[action]/{schoolId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateGroup([FromBody] GroupDTO model, int schoolId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var school = await _schoolRepository.GetById(schoolId);
            if (school == null)
            {
                return NotFound();
            }

            // check if school already has a group with given groupName.
            var found = school.Groups.SingleOrDefault(g => g.Name.ToLower().Equals(model.Name.ToLower()));
            if (found != null)
            {
                return StatusCode(500);
            }

            // check if applicationUser for group already exists.
            var userName = ConstructApplicationUserUsername(school.Name, model.Name);
            var appUser = await _userManager.FindByNameAsync(userName);
            if (appUser != null) // there already exists an ApplicationUser with given schoolName and groupName. 
            {
                return StatusCode(500);
            }

            // create ApplicationUser for group.
            var groupApplicationUser = await CreateGroupUser(school, userName, model.Name, model.Password);
            if (groupApplicationUser == null)
            {
                return StatusCode(500);
            }

            // create group.
            var group = await CreateGroup(model, groupApplicationUser.Id);
            if (group == null)
            {
                await _userManager.DeleteAsync(groupApplicationUser);
                return StatusCode(500);
            }

            // add group to school.
            AddGroupToSchool(school, group);

            await _groupRepository.SaveChanges();

            // create token.
            var token = GetToken(groupApplicationUser, group.Id);

            return
                Ok( //Todo vervangen door return _authenticationManager.GetToken(); (duplicate code van AuthController)
                    new
                    {
//                            Username = user.UserName,
//                            Token = GetToken(user, group.Id)
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
        }

        /**
         * Creates a Group and returns the created group.
         */
        [HttpPost("[action]/{schoolId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateAndReturnGroup([FromBody] GroupDTO model, int schoolId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var school = await _schoolRepository.GetById(schoolId);
            if (school == null)
            {
                return NotFound();
            }

            // check if school already has a group with given groupName.
            var found = school.Groups.SingleOrDefault(g => g.Name.ToLower().Equals(model.Name.ToLower()));
            if (found != null)
            {
                return StatusCode(500, Json("GroupName already exists."));
            }

            // check if applicationUser for group already exists.
            var userName = ConstructApplicationUserUsername(school.Name, model.Name);
            var appUser = await _userManager.FindByNameAsync(userName);
            if (appUser != null) // there already exists an ApplicationUser with given schoolName and groupName. 
            {
                return StatusCode(500);
            }

            // create ApplicationUser for group.
            var groupApplicationUser = await CreateGroupUser(school, userName, model.Name, model.Password);
            if (groupApplicationUser == null)
            {
                return StatusCode(500);
            }

            // create group.
            var group = await CreateGroup(model, groupApplicationUser.Id);
            if (group == null)
            {
                await _userManager.DeleteAsync(groupApplicationUser);
                return StatusCode(500);
            }

            // add group to school.
            AddGroupToSchool(school, group);

            await _groupRepository.SaveChanges();

            // return group object if creation of ApplicationUser succeeded.
            return Ok(group);
        }

        /**
         * Sub method, used in Group Creation Methods (CreateGroup and CreateAndReturnGroup).
         */
        private async Task<Group> CreateGroup(GroupDTO model, string groupApplicationUserId)
        {
            // Creation of Group Entity
            var group = new Group
            {
                Name = model.Name,
                Members = model.Members,
                Assignments = new List<Assignment>(),

                // add ApplicationUserId of group to group.
                ApplicationUserId = groupApplicationUserId,
            };

            await _groupRepository.Add(group);

            return group;
        }

        private async Task<ApplicationUser> CreateGroupUser(School school, string userName, string groupName,
            string password)
        {
            // Creation of ApplicationUser
            var email = ConstructApplicationUserEmail(school.Name, groupName);
            var user = _authenticationManager.CreateApplicationUserObject(email, userName, password);

            user.School = school;

            await _userManager.CreateAsync(user, password);

            // add ApplicationUser for Group to 'Group' Role.
            await _userManager.AddToRoleAsync(user, "Group");

            return user;
        }

        private static void AddGroupToSchool(School school, Group group)
        {
            if (school.Groups == null)
            {
                school.Groups = new List<Group>();
            }

            school.Groups.Add(group);
        }

        //Todo: dit moet in de AuthenticationManager (wordt ook door AuthController gebruikt)
        private JwtSecurityToken GetToken(ApplicationUser user, int groupId)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("username", user.UserName),
                new Claim("isAdmin", false.ToString()),
                new Claim("group", groupId.ToString())
            };

            var signInKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["AppSettings:Secret"]));

            return new JwtSecurityToken(
                issuer: "http://app.reva.be",
                audience: "http://app.reva.be",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256));
        }

        [HttpPut("[action]/{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateGroup([FromRoute] int id, [FromBody] GroupUpdateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var group = await _groupRepository.GetById(id);
            if (group == null)
            {
                return NotFound();
            }

            var school = await _schoolRepository.GetById(model.SchoolId);
            if (school == null)
            {
                return NotFound();
            }

            var applicationUser = await _userManager.FindByIdAsync(group.ApplicationUserId);
            if (applicationUser == null)
            {
                return NotFound();
            }

            if (model.PasswordChanged)
            {
                applicationUser.PasswordHash =
                    _userManager.PasswordHasher.HashPassword(applicationUser, model.Password);
            }

            // update Group.
            group.Name = model.Name;
            group.Members = model.Members;

            // update ApplicationUser
            var newApplicationUserName = ConstructApplicationUserUsername(school.Name, group.Name);
            applicationUser.UserName = newApplicationUserName;
            applicationUser.NormalizedUserName = newApplicationUserName.Normalize();

            var email = ConstructApplicationUserEmail(school.Name, group.Name);
            applicationUser.Email = email;
            applicationUser.NormalizedEmail = email.Normalize();

            await _userManager.UpdateAsync(applicationUser);

            _groupRepository.Update(group);
            await _groupRepository.SaveChanges();

            return Ok(group);
        }

        private static string ConstructApplicationUserUsername(string schoolName, string groupName)
        {
            return $"{schoolName}.{groupName}";
        }

        private static string ConstructApplicationUserEmail(string schoolName, string groupName)
        {
            return $"{groupName}@{schoolName}.be";
        }

        [HttpGet("[Action]/{groupId}/{memberName}")]
        public async Task<Group> AddMember(int groupId, string memberName)
        {
            var group = await _groupRepository.AddMember(groupId, memberName);
            await _groupRepository.SaveChanges();
            return group;
        }

        [HttpGet("[Action]/{groupId}/{memberName}")]
        public async Task<Group> RemoveMember(int groupId, string memberName)
        {
            var group = await _groupRepository.RemoveMember(groupId, memberName);
            await _groupRepository.SaveChanges();
            return group;
        }

        [HttpDelete("DeleteGroup/{groupId}")]
        public async Task<ActionResult> DeleteGroup(int groupId)
        {
            var group = await _groupRepository.GetById(groupId);
            if (group == null)
            {
                return NotFound();
            }

            var groupApplicationUser = await _userManager.FindByIdAsync(group.ApplicationUserId);

            if (groupApplicationUser != null)
            {
                // remove ApplicationUser of Group from Group Role.
                await _userManager.RemoveFromRoleAsync(groupApplicationUser, "Group");

                // delete ApplicationUser of Group.
                await _userManager.DeleteAsync(groupApplicationUser);
            }

            // delete Group.
            _groupRepository.Remove(group);

            await _groupRepository.SaveChanges();

            return Ok(
                new
                {
                    group.Id,
                    group.Name,
                });
        }
    }
}