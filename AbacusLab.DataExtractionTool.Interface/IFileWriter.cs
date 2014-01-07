using System.Threading.Tasks;

namespace AbacusLab.DataExtractionTool.Interface
{
    public interface IFileWriter
    {
        Task<bool> SaveInFile<T>(T data, string filePath);
    }
}