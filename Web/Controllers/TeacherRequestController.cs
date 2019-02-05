using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Web.DTOs;

namespace Web.Controllers
{
/**
* Controller that handles all API calls concerning the teacher registration requests.
*/
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherRequestController : Controller
    {
        private readonly ITeacherRequestRepository _teacherRequestRepository;


        public TeacherRequestController(ITeacherRequestRepository teacherRequestRepository)
        {
            _teacherRequestRepository = teacherRequestRepository;
        }

        /**
        * Gets all the pending requests.
        */
        [HttpGet("[Action]")]
        public async Task<IActionResult> Requests()
        {
            // check if admin
            if (User.Claims.ElementAt(4).Value == "true")
            {
                var requests = await _teacherRequestRepository.All();
                return Ok(requests);
            }

            return Unauthorized();
        }

        /**
        * A user sends a Teacher-registration request.
        */
        [HttpPost("[Action]")]
        public async Task<IActionResult> SendRequest([FromBody] CreateTeacherDTO model)
        {
            if (ModelState.IsValid)
            {
                var request = new TeacherRequest(model.Name, model.Surname, model.Email, model.SchoolName, model.Note);
                await _teacherRequestRepository.Add(request);
                
                return Ok(new
                {
                    Message = "Teacher request successfully added."
                });
            }

            return Ok(new
            {
                Message = "Please fill in all required fields."
            });
        }
    }
}