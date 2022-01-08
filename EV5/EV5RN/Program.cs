﻿
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Shared;
using System.Linq;
using CommandLine;
using System.Threading.Tasks;
using System.Text.Json;

namespace EV5RN
{
    public class CommandLineOptions
    {
        [Value(index: 0, Required = true, HelpText = "Directory within which the resource files are")]
        public string Path { get; set; }

        [Option(shortName: 'r', longName: "rootnamespace", Required = true, HelpText = "Prefix for each of the embedded resource names"
            )]
        //, Default = "")]
        public string rootnamespace { get; set; }

        [Option(shortName: 'a', longName: "assetfilter", Required = false, HelpText = "File filter for which resource files to consider"
            , Default = "*.PNG; *.html; *.jpg; *.ico; *.svg; *.css; *.js; *.woff; *.ttf")]
        //, Default = "")]
        public string assetfilter { get; set; }

        [Option(shortName: 'c', longName: "contentfilter", Required = false, HelpText = "File filter for which content files to consider"
              , Default = "*.html; *.css")]
              //, Default = "*.html")]
              //, Default = "*.css")]
        //, Default = "")]
        public string contentfilter { get; set; }
        [Option(shortName: 'e', longName: "execute", Required = false, HelpText = "Execute the file changes calculated."
            , Default = false)]
        public bool execute { get; set; }
    }

    class Program
    {
        static async Task<int> Main(string[] args)
        {
            return await Parser.Default.ParseArguments<CommandLineOptions>(args)
                .MapResult(async (CommandLineOptions opts) =>
                {
                    try
                    {
                        if (opts.execute)
                        {
                            Console.Write("Are you sure you want to execute the calculated changes(Y/N)?");
                            var a1 = Console.ReadKey();
                            if (a1.Key != ConsoleKey.Y) return -1;
                            Console.WriteLine();
                            Console.Write("Did you run and observe the changes already(Y/N)?");
                            a1 = Console.ReadKey();
                            if (a1.Key != ConsoleKey.Y) return -1;
                            Console.WriteLine();
                            Console.Write("Did you run make sure you have a backup of the files that are about to change(Y/N)?");
                            a1 = Console.ReadKey();
                            if (a1.Key != ConsoleKey.Y) return -1;
                            Console.WriteLine();
                        }

                        return Process(opts);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error!" + ex.ToString());
                        Console.ReadLine();
                        return -3; // Unhandled error
                    }
                },
                errs => Task.FromResult(-1)); // Invalid arguments
        }

        private static int Process(CommandLineOptions opts)
        {
            EV5RNResult result = new EV5RNResult() { Params = opts, Run = new Run() };
            Console.WriteLine("Evaluating Files");
            var assetfilters = opts.assetfilter.Split(";");
            Console.WriteLine("Identifying resource files: " + opts.assetfilter);
            result.Run.AssetFileTypes = new List<FileTypeInfo>();
            result.Run.AssetFiles = new List<FileInfo>();
            foreach (var filter in assetfilters)
            {
                Console.WriteLine("Enumerating: " + filter);
                var files = Directory.EnumerateFiles(opts.Path, filter.Trim(), SearchOption.AllDirectories);
                result.Run.AssetFileTypes.Add(new FileTypeInfo() { Type = filter, Count = files.Count() });

                foreach (var file in files)
                {
                    var relpath = Path.GetRelativePath(opts.Path, file);
                    result.Run.AssetFiles.Add(new FileInfo()
                    {
                        Filename = Path.GetFileName(file),
                        Path = file,
                        RelativePath = relpath,
                        SearchPath = FileUtilities.SearchFilePath(relpath),
                        ManifestPath = CreateManifestName(relpath, opts.rootnamespace)
                    });
                    //Console.WriteLine(relpath);
                }
                Console.WriteLine("Number of " + filter + " Asset files: " + files.Count());
            }
            Console.WriteLine("Number of Asset files: " + result.Run.AssetFiles.Count);



            Console.WriteLine("Identifying content files: " + opts.contentfilter);
            var contentfilters = opts.contentfilter.Split(";");
            result.Run.ContentFileTypes = new List<FileTypeInfo>();
            result.Run.ContentFiles = new List<FileInfo>();
            foreach (var contentfilter in contentfilters)
            {
                Console.WriteLine("Enumerating: " + contentfilter);
                var contentfiles = Directory.EnumerateFiles(opts.Path, contentfilter.Trim(), SearchOption.AllDirectories);
                result.Run.ContentFileTypes.Add(new FileTypeInfo() { Type = contentfilter, Count = contentfiles.Count() });

                foreach (var contentfile in contentfiles)
                {
                    var relpath = Path.GetRelativePath(opts.Path, contentfile);
                    var contentfi = new FileInfo()
                    {
                        Filename = Path.GetFileName(contentfile),
                        Path = contentfile,
                        RelativePath = relpath,
                        SearchPath = FileUtilities.SearchFilePath(relpath),
                        ManifestPath = CreateManifestName(relpath, opts.rootnamespace),
                        Changes = new List<Change>()
                    };
                    result.Run.ContentFiles.Add(contentfi);
                    Console.WriteLine(relpath);
                    Console.WriteLine("Calculating changes:");

                    var content = File.ReadAllLines(contentfile);
                    int linecounter = 1;
                    foreach (var line in content)
                    {

                        var assetfiles = result.Run.AssetFiles;
                        foreach (var assetfile in assetfiles)
                        {
                            if (line.Contains(assetfile.SearchPath))
                            {
                                var search = assetfile.SearchPath;

                                if (line.Contains('/' + assetfile.SearchPath)) search = '/' + assetfile.SearchPath;


                                contentfi.Changes.Add(new Change()
                                {
                                    Line = linecounter,
                                    Original = line,
                                    Search = search,
                                    Replace = assetfile.ManifestPath,
                                    Changed = line.Replace(search, assetfile.ManifestPath)
                                });
                                Console.Write("Line :" + linecounter + "\r");
                                continue;
                            }
                            if (line.Contains(assetfile.Filename))
                            {
                                int fnIndex = line.IndexOf(assetfile.Filename);
                                int counter = fnIndex;
                                int endMarkIndex = fnIndex + assetfile.Filename.Length;
                                if (endMarkIndex >= line.Length || line[endMarkIndex] != '"') continue;

                                char currentChar = line[counter];
                               
                                if (currentChar == '"') endMarkIndex = counter;
                                counter = fnIndex;
                                currentChar = line[counter];
                                while (counter >= 0 && currentChar != '"')
                                {
                                    counter--;
                                    if (counter >= 0) currentChar = line[counter];

                                }
                                int startMarkIndex = 0;
                                if (currentChar == '"') startMarkIndex = counter;
                                if (endMarkIndex - startMarkIndex - 1 <= 0) continue;
                                string attributePath = line.Substring(startMarkIndex + 1, endMarkIndex - startMarkIndex - 1);
                                var contentDir = Path.GetDirectoryName(contentfi.Path);
                                var calculatedAsserPath = Path.GetFullPath(Path.Combine(contentDir, FileUtilities.FixFilePath(attributePath)));
                                if (calculatedAsserPath == assetfile.Path)
                                    contentfi.Changes.Add(new Change()
                                    {
                                        Line = linecounter,
                                        Original = line,
                                        Search = attributePath,
                                        Replace = assetfile.ManifestPath,
                                        Changed = line.Replace(attributePath, assetfile.ManifestPath)
                                    });

                            }
                        }

                        linecounter++;
                    }
                    Console.WriteLine(contentfi.Changes.Count() + " Changes found.");
                }
            }
            string filename = string.Format("EV5RN-{0:yyyy-MM-dd_hh-mm-ss-tt}.json",
            DateTime.Now);
            File.WriteAllText(filename, JsonSerializer.Serialize<EV5RNResult>(result, new JsonSerializerOptions() { WriteIndented = true }));
            if (opts.execute)
            {
                Console.Write("Calculated changes will now be executed, ABORT (Y/N)?");
                var a1 = Console.ReadKey();
                if (a1.Key != ConsoleKey.N) return -1;

                foreach (var contentfile in result.Run.ContentFiles)
                {
                    if (contentfile.Changes == null || contentfile.Changes.Count() == 0) continue;
                    Console.WriteLine("Writing changes to: " + contentfile.RelativePath);
                    var lines = File.ReadAllLines(contentfile.Path);
                    foreach (var change in contentfile.Changes)
                    {
                        lines[change.Line - 1] = lines[change.Line - 1].Replace(change.Search, change.Replace);
                    }
                    File.WriteAllLines(contentfile.Path, lines);
                }

            }
            Console.ReadLine();
            return 0;
        }

        internal static string CreateManifestName
        (
            string fileName,
            string rootNamespace// May be null
        )
        {
            // Use the link file name if there is one, otherwise, fall back to file name.
            string embeddedFileName = FileUtilities.FixFilePath(fileName);



            var manifestName = string.Empty;


            // If there's no manifest name at this point, then fall back to using the
            // RootNamespace+Filename_with_slashes_converted_to_dots         
            if (manifestName.Length == 0)
            {
                // If Rootnamespace was null, then it wasn't set from the project resourceFile.
                // Empty namespaces are allowed.
                if (!string.IsNullOrEmpty(rootNamespace))
                {
                    manifestName += rootNamespace + '.';
                }

                string sourceExtension = Path.GetExtension(embeddedFileName);
                string directoryName = Path.GetDirectoryName(embeddedFileName);

                // append the directory name
                manifestName += (FileUtilities.MakeValidEverettIdentifier(directoryName));


                if (!manifestName.EndsWith('.')) manifestName += '.';


                manifestName += Path.GetFileName(embeddedFileName);

                // Replace all '\' with '.'
                manifestName.Replace(Path.DirectorySeparatorChar, '.');
                manifestName.Replace(Path.AltDirectorySeparatorChar, '.');
            }

            return manifestName;
        }


    }
}

