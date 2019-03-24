using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.DTOs;

namespace Web.Controllers
{
    /**
    * Controller that handles all API calls concerning the school.
    */
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolController : Controller
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly ITeacherRequestRepository _teacherRequestRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public SchoolController
        (
            UserManager<ApplicationUser> userManager,
            ISchoolRepository schoolRepository,
            ITeacherRequestRepository teacherRequestRepository
        )
        {
            _userManager = userManager;
            _schoolRepository = schoolRepository;
            _teacherRequestRepository = teacherRequestRepository;
        }

        [HttpGet("{schoolId}")]
        [Authorize]
        public async Task<IActionResult> School(int schoolId)
        {
            //var school = _schoolRepository.GetById(schoolId); //Todo: fix error: Process is Terminated due to StackOverFlowException 
            var school = await _schoolRepository.GetByIdLight(schoolId);
            return Ok(school);
        }

        //Todo not yet implemented in web.
        [HttpPut("[action]/{schoolId}")]
        [Authorize]
        public async Task<IActionResult> UpdateSchoolPassword([FromBody] SchoolLoginNameDto loginNameDto,
            [FromRoute] int schoolId)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(loginNameDto.SchoolLoginName))
            {
                return BadRequest();
            }

            var loginName = loginNameDto.SchoolLoginName.Trim();

            if (loginName?.Length < 1 || loginName?.Length > 16)
            {
                return BadRequest("Bad loginName. Min length 1. Max length 15.");
            }

            var school = await _schoolRepository.GetById(schoolId);
            if (school == null)
            {
                return NotFound();
            }
            
//            school.Password = loginName;
            await _schoolRepository.SaveChanges();
            
            // update password of ApplicationUser of School
            var schoolAppUser = await _userManager.FindByNameAsync(school.Name);
            schoolAppUser.PasswordHash = school.Password;
            
            return Ok(school);
        }

        [HttpPut("[action]/{schoolId}")]
        [Authorize]
        public async Task<IActionResult> UpdateSchoolLoginName([FromBody] SchoolLoginNameDto loginNameDto,
            [FromRoute] int schoolId)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(loginNameDto.SchoolLoginName))
            {
                return BadRequest();
            }

            var loginName = loginNameDto.SchoolLoginName.Trim();

            if (loginName?.Length < 1 || loginName?.Length > 16)
            {
                return BadRequest("Bad loginName. Min length 1. Max length 15.");
            }

            var school = await _schoolRepository.GetById(schoolId);
            if (school == null)
            {
                return NotFound();
            }


            var exists = await CheckLoginNameExists(loginName);
            if (exists)
            {
                return StatusCode(500, "There is already a school with that login name.");
            }
            
            school.LoginName = loginName;
            await _schoolRepository.SaveChanges();

            return Ok(school);
        }

        [HttpGet("[action]/{loginName}")]
        [Authorize]
        public async Task<IActionResult> CheckLoginName(string loginName)
        {
            var exists = await CheckLoginNameExists(loginName);
            
            if (exists)
            {
                return Ok(new {loginName = "alreadyexists"});
            }

            return Ok(new {loginName = "ok"});
        }

        private async Task<bool> CheckLoginNameExists(string schoolLoginName)
        {
            var school = await _schoolRepository.GetBySchoolLoginName(schoolLoginName.Trim().ToLower());
            return school != null;
        }

        /**
         * Gets all the schools. 
         */
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Schools()
        {
            var schools = await _schoolRepository.GetAll();
            return Ok(schools);
        }

        [HttpGet("[action]/{schoolName}")]
        public async Task<IActionResult> CheckSchoolName(string schoolName)
        {
            var exists = await _schoolRepository.GetBySchoolName(schoolName) != null
                         || await CheckIfUnacceptedSchoolRequestWithSchoolNameExists(schoolName);
            if (exists)
            {
                return Ok(new {schoolName = "alreadyexists"});
            }

            return Ok(new {schoolName = "ok"});
        }

        private async Task<bool> CheckIfUnacceptedSchoolRequestWithSchoolNameExists(string schoolName)
        {
            var teacherRequest = await _teacherRequestRepository.GetBySchool(schoolName);
            if (teacherRequest == null)
            {
                return false;
            }

            if (teacherRequest.Accepted == false) // Declined request => schoolName is still "open" or "usable".
                         {
                return false;
            }

            return true;
        }
    }
}