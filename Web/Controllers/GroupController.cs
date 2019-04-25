using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

        public GroupController(
            IGroupRepository groupRepository,
            UserManager<ApplicationUser> userManager,
            IAuthenticationManager authenticationManager,
            ISchoolRepository schoolRepository,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _groupManager = new GroupManager(configuration, groupRepository);
            _groupRepository = groupRepository;
            _authenticationManager = authenticationManager;
            _schoolRepository = schoolRepository;
        }


        /**
        * Returns group status/info to android (current assignment, number of assignments done by group,
        * if group already has an assignment and the coordinates of the previous exhibitor if existing).
        * Gets groupId via token -> groupId
        */
        [HttpGet("[action]")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GroupInfo()
        {
            var groupSidClaim = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.GroupSid);

            if (groupSidClaim == null)
            {
                return NotFound("groupId not found in token.");
            }

            var groupId = groupSidClaim.Value;

            var group = await _groupRepository.GetById(Convert.ToInt32(groupId));
            if (group == null)
            {
                return NotFound(new {Message = "Group not found or groupId not in token."});
            }

            var groupInfo = _groupManager.GetGroupInfo(group, await _userManager.FindByNameAsync("admin"));
            return groupInfo;
        }

        /**
         * return group with id equal to parameter groupId.
         */
        [HttpGet("[action]/{groupId}")]
        [Authorize]
        public async Task<IActionResult> Group(int groupId)
        {
            return Ok(await _groupRepository.GetById(groupId));
        }

        /**
         * return group with id equal to parameter groupId.
         */
        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> Group()
        {
            var applicationUserIdClaim = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid);

            if (applicationUserIdClaim == null)
            {
                return NotFound("ApplicationUserId not found in token.");
            }

            var applicationUserId = applicationUserIdClaim.Value;

            var groupAppUser = await _authenticationManager.GetAppUserWithGroupsIncludedViaId(applicationUserId);

            if (groupAppUser.School.Groups == null)
            {
                return NotFound();
            }

            var group = groupAppUser.School.Groups.SingleOrDefault(g => g.ApplicationUserId == groupAppUser.Id);
            if (group == null)
            {
                return NotFound();
            }

            return Ok(group);
        }

        /**
         * Checks if the groupName already exists.
         */
        [HttpGet("[action]/{groupName}")]
        [Authorize]
        public async Task<IActionResult> CheckGroupName(string groupName)
        {
            groupName = groupName.ToLower();

            var applicationUserIdClaim = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid);

            if (applicationUserIdClaim == null)
            {
                return NotFound("ApplicationUserId not found in token.");
            }

            var applicationUserId = applicationUserIdClaim.Value;

            var schoolAppUser = await _authenticationManager.GetAppUserWithGroupsIncludedViaId(applicationUserId);


            if (schoolAppUser == null)
            {
                return NotFound("School in token not found.");
            }

            if (schoolAppUser.School == null)
            {
                return NotFound("ApplicationUser of School has no School.");
            }

            var foundGroup = "alreadyexists";

            if (schoolAppUser.School.Groups.FindIndex(g => g.Name.ToLower().Equals(groupName)) < 0)
            {
                foundGroup = "ok";
            }

            return Ok(new {GroupName = foundGroup});
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> Groups()
        {
            return Ok(await _groupRepository.GetAllLight());
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> GroupsOfSchool()
        {
            var applicationUserIdClaim = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid);

            if (applicationUserIdClaim == null)
            {
                return NotFound("ApplicationUserId not found in token.");
            }

            var applicationUserId = applicationUserIdClaim.Value;

            var schoolAppUser = await _authenticationManager.GetAppUserWithGroupsIncludedViaId(applicationUserId);
            if (schoolAppUser == null)
            {
                return NotFound("ApplicationUser with id from token could not be found.");
            }

            if (schoolAppUser.School == null)
            {
                return NotFound("ApplicationUser of School has no School.");
            }

            var school = schoolAppUser.School;

            return Ok(school.Groups);
        }


        /**
         * Returns all groups of school with schoolId equal to parameter schoolId.
         */
        [HttpGet("[action]/{schoolId}")]
        [Authorize]
        public async Task<IActionResult> Groups(int schoolId)
        {
            var school = await _schoolRepository.GetByIdLight(schoolId);
            if (school != null)
            {
                return Ok(school.Groups);
            }

            return NotFound();
        }

        /**
         * Creates a Group and returns a JwtToken. Used in android.
         */
        [HttpPost("[action]")]
        [Authorize] //todo only role school
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateGroup([FromBody] GroupDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var applicationUserIdClaim = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid);

            if (applicationUserIdClaim == null)
            {
                return NotFound("ApplicationUserId not found in token.");
            }

            var applicationUserId = applicationUserIdClaim.Value;

            var schoolAppUser = await _authenticationManager.GetAppUserWithGroupsIncludedViaId(applicationUserId);
            if (schoolAppUser == null)
            {
                return NotFound("ApplicationUser with id from token could not be found.");
            }

            if (schoolAppUser.School == null)
            {
                return NotFound("ApplicationUser of School has no School.");
            }

            var school = schoolAppUser.School;

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
            var claims = await _authenticationManager.CreateClaims(groupApplicationUser, group.Id.ToString());
            var token = _authenticationManager.GetToken(claims);

            return Ok(
                new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
        }

        /**
         * Creates a Group and returns the created group, used in web.
         */
        [HttpPost("[action]/{schoolId}")]
        [Authorize] //todo check if Teacher role
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

        [HttpPut("[action]/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateGroup([FromRoute] int id, [FromBody] GroupUpdateDTO model)
        {
            model.Name = model.Name.ToLower();

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
        [Authorize]
        //todo authorize teacher or admin
        public async Task<IActionResult> AddMember(int groupId, string memberName)
        {
            var group = await _groupRepository.AddMember(groupId, memberName);
            await _groupRepository.SaveChanges();
            return Ok(group);
        }

        [HttpGet("[Action]/{groupId}/{memberName}")]
        [Authorize]
        //todo authorize teacher or admin
        public async Task<IActionResult> RemoveMember(int groupId, string memberName)
        {
            var group = await _groupRepository.RemoveMember(groupId, memberName);
            if (group == null)
            {
                return StatusCode(500, "Group not found or group members length < 2.");
            }

            await _groupRepository.SaveChanges();

            return Ok(group);
        }

        [HttpDelete("DeleteGroup/{groupId}")]
        [Authorize]
        //todo authorize teacher or admin
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