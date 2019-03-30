using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;


        public TeacherRequestController(
            ITeacherRequestRepository teacherRequestRepository,
            IEmailSender emailSender,
            IConfiguration configuration
        )
        {
            _teacherRequestRepository = teacherRequestRepository;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        /**
        * Gets all the pending requests.
        */
        [HttpGet("[Action]")]
        [Authorize]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Requests()
        {
            var requests = await _teacherRequestRepository.All();

            return Ok(requests);
        }

        /**
        * A user sends a Teacher-registration request.
        */
        [HttpPost("[Action]")]
        public async Task<IActionResult> SendRequest([FromBody] CreateTeacherDTO model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please fill in all required fields.");
                return BadRequest(ModelState);
            }

            var request = new TeacherRequest(model.Name, model.Surname, model.Email, model.SchoolName, model.Note);
            await _teacherRequestRepository.Add(request);
            await _teacherRequestRepository.SaveChanges();

            await _emailSender.SendMailAsync(_configuration["Email:Smtp:From"],
                $"Nieuwe aanvraag voor Reva App",
                $@"
                            <p>Beste,<p>
                            <p>Er werd zojuist een aanvraag ingediend voor een nieuwe gebruiker!</p>
                            <br/>
                            <br/>
                            <p>School: {model.SchoolName}</p>
                            <p>Voornaam: {model.Name}</p>
                            <p>Naam: {model.Surname}</p>
                            <p>Note: {model.Note}</p>
                            <br/>
                            <br/>
                            <p>Met vriendelijke groet,</p>
                            <p>Reva app team</p>
                    ", new[] {"freddy@reva.be"});

            return Ok(new
            {
                Message = "Teacher request successfully added."
            });
        }

        /**
        * Admin updates a Teacher-registration request.
        */
        [HttpPut("[Action]/{requestId}")]
        [Authorize]
        public async Task<IActionResult> UpdateRequest([FromBody] CreateTeacherDTO model, int requestId)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please fill in all required fields.");
                return BadRequest(ModelState);
            }

            var request = await _teacherRequestRepository.GetById(requestId);
            request.Name = model.Name;
            request.Surname = model.Surname;
            request.Note = model.Note;
            request.Email = model.Email;
            request.SchoolName = model.SchoolName;
            await _teacherRequestRepository.SaveChanges();

            return Ok(new
            {
                Message = "Teacher request successfully updated."
            });
        }

        /**
        * return Request with id equal to parameter requestId.
        */
        [HttpGet("[Action]/{requestId}")]
        [Authorize]
        public async Task<IActionResult> TeacherRequest(int requestId)
        {
            var request = await _teacherRequestRepository.GetById(requestId);
            return Json(request);
        }

        /**
        * return true if Request exists with id equal to parameter requestId.
        */
        [HttpGet("[Action]/{requestId}")]
        [Authorize]
        public async Task<IActionResult> TeacherRequestExists(int requestId)
        {
            var request = await _teacherRequestRepository.GetById(requestId);

            return Ok(request != null);
        }

        /**
        * Admin declines Teacher Request.
        *
        **/
        [HttpGet("[Action]/{teacherRequestId}")]
        [Authorize]
        public async Task<ActionResult> DeclineRequest(int teacherRequestId)
        {
            var request = await _teacherRequestRepository.GetById(teacherRequestId);
            request.Accepted = false;
            await _teacherRequestRepository.SaveChanges();

            return Ok(new
            {
                Message = "Request removed."
            });
        }
    }
}