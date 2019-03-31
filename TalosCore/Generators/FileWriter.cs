using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TalosCore
{
    public class FileWriter : IFileWriter
    {
        private Dictionary<string, FileStream> FileStreams { get; } = new Dictionary<string, FileStream>();
        private Dictionary<string, int> IndentLevels { get; } = new Dictionary<string, int>();
        private int IndentSize { get; } = 4;
        private string _indentString = "";

        public FileWriter()
        {
            for (int i = 0; i < IndentSize; i++)
            {
                _indentString += " ";
            }
        }

        public void WriteStringToFile(string data, string filename)
        {
            if (!FileStreams.ContainsKey(filename))
            {
                FileStreams[filename] = File.Create(filename);
                IndentLevels[filename] = 0;
            }
            data = data.Trim();
            if (data.Length > 0 && data[0] == '.')
            {
                data = _indentString + data;
            }

            if (!(data.Contains("{") && data.Contains("}"))) // Handle properties with parens on same line
            {
                if (data.Contains("}"))
                {
                    IndentLevels[filename]--;
                }
            }

            string indent = "";
            for (int i = 0; i < IndentLevels[filename]; i++)
            {
                indent += _indentString;
            }
            byte[] info = new UTF8Encoding(true).GetBytes(indent + data + "\n");
            FileStreams[filename].Write(info);

            if (!(data.Contains("{") && data.Contains("}")))
            {
                if (data.Contains("{"))
                {
                    IndentLevels[filename]++;
                }
            }
        }

        public void Close(string filename)
        {
            if (FileStreams.ContainsKey(filename))
            {
                FileStreams[filename].Close();
                FileStreams.Remove(filename);
            }
        }

        public bool Exists(string filename)
        {
            return File.Exists(filename);
        }

        public void CreateDirectory(string dirname)
        {
            Directory.CreateDirectory(dirname);
        }
    }
}