using Symitar;

namespace Visor.Lib
{
    public interface IFileSystem
    {
        void Open(FileType fileType, string fileName);
    }
}