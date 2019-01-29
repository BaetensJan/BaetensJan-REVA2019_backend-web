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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IConfiguration _configuration;

        public GroupController(IGroupRepository repository, UserManager<ApplicationUser> userManager,
            IAuthenticationManager authenticationManager, ISchoolRepository schoolRepository,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _groupRepository = repository;
            _authenticationManager = authenticationManager;
            _schoolRepository = schoolRepository;
            _configuration = configuration;
        }

        /**
         * Returns all groups of school with schoolId equal to parameter schoolId.
         * Return: Task<IEnumerable<Group>> (list of groups) 
         */
        [HttpGet("[action]/{schoolId}")]
        public async Task<IEnumerable<Group>> GroupsBasicBySchoolId(int schoolId)
        {
            //TODO get username out of token, get groups from user.
            return await _groupRepository.GetBasicsBySchoolId(schoolId);
        }

        /**
         * return group with id equal to parameter groupId.
         */
        [HttpGet("[action]")] // /{groupId}
        public async Task<Group> Group( /*int groupId*/)
        {
            // get username from jwt token.
            var groupId = User.Claims.ElementAt(5).Value;

            return await _groupRepository.GetById(Convert.ToInt32(groupId));
        }

        /**
         * Returns all groups of school with schoolId equal to parameter schoolId.
         */
        [HttpGet("[action]/{schoolId}")]
        public async Task<IEnumerable<Group>> Groups(int schoolId)
        {
//            return await _groupRepository.GetAllBySchoolId(schoolId); //Todo: deze haalt de assignments niet op
            return _groupRepository.GetAllBySchoolIdLight(schoolId);
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
         * Returns group with schoolId equal to parameter schoolId and groupName equal to parameter.
         * If parameter schoolId is -1, then the schoolId can be extracted out of the token via User.claims.
         */
        [HttpGet("[action]/{schoolId}/{groupName}")]
        public async Task<Group> GetBySchoolIdAndGroupName(int schoolId, string groupName)
        {
            // get group object via schoolId and groupName
            return await _groupRepository.GetBySchoolIdAndGroupName(schoolId, groupName);
        }

        /**
         * Creates a Group and returns a JwtToken.
         */
        [HttpPost("[action]/{schoolId}")]
        public async Task<ActionResult> CreateGroup([FromBody] GroupDTO model, int schoolId)
        {
            if (ModelState.IsValid)
            {
                var group = await CreateGroup(model);
                var school = await AddGroupToSchool(schoolId, group);

                var user = await CreateGroupUser(school, model.Name,
                    model.Password); //todo: add column group to appuser table

                var token = GetToken(user, group.Id);
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

            return Ok(
                new
                {
                    Message = "Zorg dat naam ingevuld is."
                });
        }

        /**
         * Creates a Group and returns the created group.
         */
        [HttpPost("[action]/{schoolId}")]
        public async Task<ActionResult> CreateAndReturnGroup([FromBody] GroupDTO model, int schoolId)
        {
            if (ModelState.IsValid)
            {
                var group = await CreateGroup(model);
                var school = await AddGroupToSchool(schoolId, group);

                await CreateGroupUser(school, model.Name, model.Password); //todo: add column group to appuser table

                return
                    Ok(group);
            }

            return Ok(
                new
                {
                    Message = "Zorg dat naam ingevuld is."
                });
        }

        /**
         * Sub method, used in Group Creation Methods (CreateGroup and CreateAndReturnGroup).
         */
        private async Task<Group> CreateGroup(GroupDTO model)
        {
            // Creation of Group Entity
            var group = new Group
            {
                Name = model.Name,
                Members = model.Members,
                Assignments = new List<Assignment>()
            };

            await _groupRepository.Add(group);
            await _groupRepository.SaveChanges();
            return group;
        }

        private async Task<ApplicationUser> CreateGroupUser(School school, string username, string password)
        {
            // Creation of ApplicationUser
            var userName = school.Name + username;
            var user = _authenticationManager.CreateApplicationUserObject("test@test.be",
                userName, password);
            user.School = school;
            await _userManager.CreateAsync(user, password);
            //await _userManager.AddToRoleAsync(user, "Group");//Todo error at this point.
            return user;
        }

        private async Task<School> AddGroupToSchool(int schoolId, Group group)
        {
            var school = await _schoolRepository.GetById(schoolId);
            if (school.Groups == null) school.Groups = new List<Group>();
            school.Groups.Add(group);
            return school;
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
                new Claim("school", user.School.Id.ToString()),
                new Claim("group", groupId.ToString()),
            };

            var signInKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["AppSettings:Secret"]));

            return new JwtSecurityToken(
                issuer: "http://xyz.com",
                audience: "http://xyz.com",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256));
        }

        [HttpPost("UpdateGroup")]
        public async Task<ActionResult> UpdateGroup([FromBody] GroupUpdateDTO model)
        {
            if (ModelState.IsValid)
            {
                var groupApplicationUser = _userManager.Users.SingleOrDefault(u => u.UserName == model.Name);

                //check if group exists 
                if (groupApplicationUser == null)
                    return Ok(
                        new
                        {
                            Message = "Groep niet teruggevonden."
                        });

                var group = await _groupRepository.GetById(model.GroupId);

                // name was updated.
                if (!group.Name.ToLower().Equals(model.Name.ToLower()))
                {
                    groupApplicationUser.UserName = model.Name;
                    group.Name = model.Name;
                }

                // password was updated.
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    groupApplicationUser.PasswordHash = model.Password;
                }

                if (model.Members != null && model.Members.Count > 0)
                    group.Members = model.Members;

                await _groupRepository.SaveChanges();

                return Ok(group);
            }

            return Ok(
                new
                {
                    Message = "Zorg dat naam ingevuld is."
                });
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
            if (group != null)
            {
                _groupRepository.Remove(group);
                await _groupRepository.SaveChanges();
            }

            return Ok(
                new
                {
                    Id = group.Id,
                    Name = group.Name
                });
        }
    }
}