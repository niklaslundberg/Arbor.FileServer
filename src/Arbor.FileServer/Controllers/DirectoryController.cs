using System.Collections.Immutable;
using System.IO;
using Arbor.FileServer.Hashing;
using Arbor.FileServer.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Arbor.FileServer.Controllers
{
    public class DirectoryController : Controller
    {
        [HttpGet]
        [Route("/clean")]
        public object Clean([FromServices] FileServerSettings fileServerSettings)
        {
            var directoryInfo = new DirectoryInfo(fileServerSettings.BasePath);

            ImmutableArray<string> removedFiles = HashCreator.RemoveAllHashFiles(directoryInfo);
            ImmutableArray<string> createdHashFiles = HashCreator.HashFiles(directoryInfo, SupportedHashAlgorithm.All);

            return new
            {
                Removed = removedFiles,
                Created = createdHashFiles
            };
        }

        [HttpGet]
        [Route("/files")]
        public object Files([FromServices] FileServerSettings fileServerSettings)
        {
            FilesViewModel filesViewModel = fileServerSettings.CreateViewModel();
            return filesViewModel;
        }

        [HttpGet]
        [Route("/")]
        public IActionResult Index([FromServices] FileServerSettings fileServerSettings)
        {
            FilesViewModel filesViewModel = fileServerSettings.CreateViewModel();
            return View(filesViewModel);
        }
    }
}
