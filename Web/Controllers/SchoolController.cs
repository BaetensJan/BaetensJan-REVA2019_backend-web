using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        public SchoolController(ISchoolRepository schoolRepository)
        {
            _schoolRepository = schoolRepository;
        }

        [HttpGet("{schoolId}")]
        [Authorize]
        public async Task<IActionResult> School(int schoolId)
        {
            //var school = _schoolRepository.GetById(schoolId); //Todo: fix error: Process is Terminated due to StackOverFlowException 
            var school = await _schoolRepository.GetByIdLight(schoolId);
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
            var test = await _schoolRepository.GetByName(schoolName);
            var exists = test != null;
            if (exists)
            {
                Ok(new {schoolName = "alreadyexists"});
            }

            return Ok(new {schoolName = "ok"});
        }
    }
}