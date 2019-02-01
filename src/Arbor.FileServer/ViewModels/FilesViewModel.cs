using System.Collections.Immutable;

namespace Arbor.FileServer.ViewModels
{
    public class FilesViewModel
    {
        public FilesViewModel(ImmutableArray<FileGroup> files)
        {
            Files = files;
        }

        public ImmutableArray<FileGroup> Files { get; }
    }
}
