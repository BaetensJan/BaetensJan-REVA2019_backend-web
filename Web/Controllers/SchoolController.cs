using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Web.Controllers
{
    /**
    * Controller that handles all API calls concerning the school.
    */
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ISchoolRepository _schoolRepository;

        public SchoolController(UserManager<ApplicationUser> userManager,
            IConfiguration configuration, ISchoolRepository schoolRepository)
        {
            _userManager = userManager;
            _configuration = configuration;
            _schoolRepository = schoolRepository;
        }

        [HttpGet("{schoolId}")]
        public IActionResult School(int schoolId)
        {
            //var school = _schoolRepository.GetById(schoolId); //Todo: fix error: Process is Terminated due to StackOverFlowException 
            var school = _schoolRepository.GetByIdLight(schoolId);
            return Ok(school);
        }

        [HttpGet("[action]/{schoolName}")]
        public async Task<IActionResult> CheckSchoolName(string schoolName)
        {
            var test = await _schoolRepository.GetByName(schoolName);
            var exists = test != null;
            return exists ? Ok(new {schoolName = "alreadyexists"}) : Ok(new {schoolName = "ok"});
        }
    }
}