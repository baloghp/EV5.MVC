using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV5RN
{
    public class EV5RNResult
    {
        public CommandLineOptions Params { get; set; }

        public Run Run { get; set; }
    }

    public class Run
    {
        public List<FileTypeInfo> ContentFileTypes { get; set; }
        public List<FileTypeInfo> AssetFileTypes { get; set; }
        public List<FileInfo> ContentFiles { get; set; }
        public List<FileInfo> AssetFiles { get; set; }
    }

    public class FileInfo
    {
        public string Filename { get; set; }
        public string Path { get; set; }
        public string RelativePath { get; set; }

        public string SearchPath { get; set; }

        public string ManifestPath { get; set; }

        public List<Change> Changes { get; set; }
    }

    public class Change
    {
        public int Line { get; set; }

        public FileInfo ResourceFound { get; set; }
        public string Original { get; set; }
        public string Search { get; set; }
        public string Replace { get; set; }
        public string Changed { get; set; }

    }

    public class FileTypeInfo
    {
        public string Type { get; set; }

        public int Count { get; set; }
    }
}
