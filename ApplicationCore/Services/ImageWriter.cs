using System;
using System.IO;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ApplicationCore.Services
{
    public class ImageWriter : IImageWriter
    {
        private readonly IHostingEnvironment _env;

        public ImageWriter(IHostingEnvironment environment)
        {
            _env = environment;
        }

        public async Task<string> UploadImage(IFormFile file, string fileName = "")
        {
            if (CheckIfImageFile(file))
            {
                return await WriteFile(file, fileName);
            }

            return "Invalid image file";
        }

        private static bool CheckIfImageFile(IFormFile file)
        {
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
            }

            return WriterHelper.GetImageFormat(fileBytes) != WriterHelper.ImageFormat.unknown;
        }

        private async Task<string> WriteFile(IFormFile file, string fileName = "")
        {
            try
            {
                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                if (fileName.Trim().Length == 0)
                    fileName = Guid.NewGuid() + extension; //Create a new Name 
                //for the file due to security reasons.
                var path = Path.Combine(_env.WebRootPath, "images", fileName);
                (new FileInfo(path)).Directory?.Create();
                using (var bits = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(bits);
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return fileName;
        }

        public string WriteBase64ToFile(string base64)
        {
            string fileName;
            try
            {
                var bytes = Convert.FromBase64String(base64); // a base64 image 

                fileName = Guid.NewGuid() + ".jpg"; //Create a new Name 

                // full path to file in current project location
                var path = Path.Combine(_env.WebRootPath, "images", fileName);
//                new FileInfo(path).Directory?.Create();
//
//                if (bytes.Length > 0)
//                {
//                    using (var stream = new FileStream(path, FileMode.Create))
//                    {
//                        stream.Write(bytes, 0, bytes.Length);
//                        stream.Flush();
//                    }
//                }

                File.WriteAllBytes(path, bytes);
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return fileName;
        }
    }
}