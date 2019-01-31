using System.Collections.Immutable;

namespace Arbor.FileServer.ViewModels
{
    public class FilesViewModel
    {
        public ImmutableArray<FileGroup> Files { get; }

        public FilesViewModel(ImmutableArray<FileGroup> files)
        {
            Files = files;
        }
    }
}