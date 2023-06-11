using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Topic.API.Infrastructure;

namespace Topic.API.Controllers
{
    [ApiController]
    public class PicController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public PicController(IWebHostEnvironment env, TopicContext topicContext)
        {
            _env = env;
        }

        [HttpGet]
        [Route("api/v1/category/items/pic/{categoryFileName}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        // GET: /<controller>/
        public async Task<ActionResult> GetImageAsync(string categoryFileName)
        {
            if (string.IsNullOrEmpty(categoryFileName))
            {
                return BadRequest();
            }

            var path = Path.Combine(_env.WebRootPath, categoryFileName);

            string imageFileExtension = Path.GetExtension(categoryFileName);
            string mimetype = GetImageMimeTypeFromImageFileExtension(imageFileExtension);

            var buffer = await System.IO.File.ReadAllBytesAsync(path);

            return File(buffer, mimetype);
        }

        private string GetImageMimeTypeFromImageFileExtension(string extension)
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