namespace TalosCore
{
    public interface IFileWriter
    {
        void WriteStringToFile(string data, string filename);
        void Close(string filename);
        bool Exists(string filename);
        void CreateDirectory(string dirname);
    }
}