using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [ApiController]
    public class PicController : ControllerBase
    {
        [HttpGet]
        [Route("/api/v1/user/profile-image/{userFileName}")]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        // GET: /<controller>/
        public async Task<ActionResult> GetProfileImageAsync(string userFileName)
        {
            if (string.IsNullOrEmpty(userFileName))
            {
                return BadRequest();
            }

            // var path = Path.Combine(_env.WebRootPath, userFileName);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"
                , $"images/profile/{userFileName}");

            string imageFileExtension = Path.GetExtension(userFileName);
            string mimetype = GetImageMimeTypeFromImageFileExtension(imageFileExtension);

            var buffer = await System.IO.File.ReadAllBytesAsync(path);

            return File(buffer, mimetype);
        }

        [HttpGet]
        [Route("/api/v1/user/banner-image/{userFileName}")]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        // GET: /<controller>/
        public async Task<ActionResult> GetBannerImageAsync(string userFileName)
        {
            if (string.IsNullOrEmpty(userFileName))
            {
                return BadRequest();
            }

            // var path = Path.Combine(_env.WebRootPath, userFileName);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"
                , $"images/banner/{userFileName}");

            string imageFileExtension = Path.GetExtension(userFileName);
            string mimetype = GetImageMimeTypeFromImageFileExtension(imageFileExtension);

            var buffer = await System.IO.File.ReadAllBytesAsync(path);

            return File(buffer, mimetype);
        }

        private static string GetImageMimeTypeFromImageFileExtension(string extension)
        {
            string mimetype;

            switch (extension)
            {
                case ".png":
                    mimetype = "image/png";
                    break;
                case ".gif":
                    mimetype = "image/gif";
                    break;
                case ".jpg":
                case ".jpeg":
                    mimetype = "image/jpeg";
                    break;
                case ".bmp":
                    mimetype = "image/bmp";
                    break;
                case ".tiff":
                    mimetype = "image/tiff";
                    break;
                case ".wmf":
                    mimetype = "image/wmf";
                    break;
                case ".jp2":
                    mimetype = "image/jp2";
                    break;
                case ".svg":
                    mimetype = "image/svg+xml";
                    break;
                default:
                    mimetype = "application/octet-stream";
                    break;
            }

            return mimetype;
        }
    }
}