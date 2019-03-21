using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

        public SchoolController
        (
            ISchoolRepository schoolRepository,
            ITeacherRequestRepository teacherRequestRepository
        )
        {
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

            var schools = await _schoolRepository.GetAll();
            var exists = schools.Any(s => s.LoginName.ToLower().Equals(loginName.ToLower()));
            if (exists)
            {
                return StatusCode(500, "There is already a school with that login name.");
            }

            school.LoginName = loginName;
            await _schoolRepository.SaveChanges();

            return Ok(school);
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
            var exists = await _schoolRepository.GetByName(schoolName) != null
                         || await _teacherRequestRepository.GetBySchool(schoolName) != null;
            if (exists)
            {
                return Ok(new {schoolName = "alreadyexists"});
            }

            return Ok(new {schoolName = "ok"});
        }

        [HttpGet("[action]/{loginName}")]
        [Authorize]
        public async Task<IActionResult> CheckLoginName(string loginName)
        {
            var schools = await _schoolRepository.GetAll();
            foreach (var school in schools)
            {
                if (school.LoginName.ToLower().Equals(loginName.Trim().ToLower()))
                {
                    return Ok(new {loginName = "alreadyexists"});
                }
            }

            return Ok(new {loginName = "ok"});
        }
    }
}