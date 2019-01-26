using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ApplicationCore.Interfaces
{
    public interface IImageWriter
    {
        Task<string> UploadImage(IFormFile file, string fileName);
        string WriteBase64ToFile(string base64);
//        Task<string> UploadExhibitorMap(IFormFile file);
    }
}