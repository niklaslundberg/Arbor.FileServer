using System.IO;
using Arbor.FileServer.Hashing;
using Microsoft.AspNetCore.Mvc;

namespace Arbor.FileServer.Controllers
{
    public class FilesController : Controller
    {
        [Route("/file/{**filePath}")]
        [HttpGet]
        public object Index(string filePath, [FromServices] FileServerSettings settings)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return new StatusCodeResult(400);
            }

            string fullFilePath = Path.Combine(settings.BasePath, filePath);

            if (!System.IO.File.Exists(fullFilePath))
            {
                return new StatusCodeResult(404);
            }

            var fileInfo = new FileInfo(fullFilePath);

            return new
            {
                fileInfo.Name,
                fileInfo.Length,
                fileInfo.LastAccessTimeUtc,
                fileInfo.CreationTimeUtc,
                fileInfo.LastWriteTimeUtc,
                fileInfo.Extension
            };
        }
    }
}
