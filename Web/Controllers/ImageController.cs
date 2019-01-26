using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : Controller
    {
        private readonly IImageWriter _imageWriter;

        public ImageController(IImageWriter imageWriter)
        {
            _imageWriter = imageWriter;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            return Ok(await _imageWriter.UploadImage(file, ""));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateExhibitionRoutePlanImage(IFormFile file)
        {
            //Todo: check in frontend that only .jpg files are uploaded.
            return Ok(new {Name = await _imageWriter.UploadImage(file, "beursplan.jpg")});
        } 
        
//        [HttpGet("[action]")]
//        public async Task<IActionResult> GetExhibitionRoutePlanImage()
//        {
//            var file = ""; //Todo: return beursplan.jpg from /wwwroot/images
//            return Ok(new {File = file});
//        }
    }
}