using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace TalosCore
{
    public class EfClassInfoList
    {
        public string DbNamespace { get; set; }
        public string DbContextName { get; set; }
        public List<EfClass> EfClasses { get; set; } = new List<EfClass>();
    }

    public class EFParser
    {
        public static string GetProjectName(string startDirectory)
        {
            string programFileName = "Program.cs";
            string fullPathToFile = Path.Combine(startDirectory, programFileName);
            if (File.Exists(fullPathToFile))
            {
                var source = File.ReadAllText(fullPathToFile);
                var match = Regex.Match(source, @"namespace (\w+)[\W*]");
                if (match.Length > 0)
                {
                    return match.Groups[1].ToString();
                }
            }
            return null;
        }

        public static EfClassInfoList GetEfClassInfoList(string startDirectory)
        {
            var efClassInfoList = new EfClassInfoList();
            string dbContextFilePath = GetDbContextFilePath(startDirectory);
            if (dbContextFilePath == null)
            {
                throw new Exception($"No DbContext file found");
            }

            efClassInfoList.DbContextName = GetDbContextName(dbContextFilePath);
            if (efClassInfoList.DbContextName == null)
            {
                throw new Exception($"No DbContextName found");
            }

            efClassInfoList.DbNamespace = GetDbNamespace(dbContextFilePath);
            if (efClassInfoList.DbNamespace == null)
            {
                throw new Exception($"No DbNamespace found");
            }

            efClassInfoList.EfClasses = GetEfClasses(dbContextFilePath);
            if (efClassInfoList.EfClasses == null || efClassInfoList.EfClasses.Count == 0)
            {
                throw new Exception($"No EfClasses found");
            }

            return efClassInfoList;
        }

        public static string GetProjectPath(string startDirectory)
        {
            var dirs = Directory.EnumerateDirectories(startDirectory);
            foreach (var dir in dirs)
            {
                if (Directory.GetFiles(dir, "*.csproj").Length > 0)
                {
                    return dir;
                }

                var path = GetProjectPath(dir);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    return path;
                }
            }
            return null;
        }

        // TODO: Handle context file not found
        // TODO: Handle entity class not found
        // TODO: Improve regular expressions
        private static string GetDbContextName(string filename)
        {
            foreach (string line in File.ReadLines(filename))
            {
                var classMatch = Regex.Match(line, @"public class (.+) : DbContext");
                if (classMatch.Length > 0)
                {
                    return classMatch.Groups[1].ToString();
                }
            }

            return null;
        }

        private static string GetDbNamespace(string filename)
        {
            foreach (string line in File.ReadLines(filename))
            {
                var classMatch = Regex.Match(line, @"namespace (.+)");
                if (classMatch.Length > 0)
                {
                    return classMatch.Groups[1].ToString();
                }
            }

            return null;
        }

        public static string GetDbContextFilePath(string startDirectory)
        {
            var dirs = Directory.EnumerateDirectories(startDirectory);
            foreach (var dir in dirs)
            {
                foreach (var file in Directory.GetFiles(dir, "*.cs"))
                {
                    var source = File.ReadAllText(file);
                    var match = Regex.Match(source, @"public.+class.*\:.*DbContext");
                    if (match.Length > 0)
                    {
                        return file;
                    }
                }
                var path = GetDbContextFilePath(dir);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    return path;
                }
            }
            return null;
        }

        public static List<EfClass> GetEfClasses(string dbContextFile)
        {
            var classes = new List<EfClass>();
            foreach (string line in File.ReadLines(dbContextFile))
            {
                var classMatch = Regex.Match(line, @"public DbSet<(\w+)>\W+(\w+)");
                if (classMatch.Groups.Count == 3)
                {
                    string path = Path.GetDirectoryName(dbContextFile);
                    string className = classMatch.Groups[1].ToString();
                    var classInfo = GetEfClassInfo(className, path);
                    classInfo.CollectionName = classMatch.Groups[2].ToString();
                    classes.Add(classInfo);
                    //Console.WriteLine($"      EF Class: {classMatch.Groups[1]}");
                }
            }
            return classes;
        }

        public static EfClass GetEfClassInfo(string className, string dir)
        {
            var efClass = new EfClass();

            // Search files for this class name
            foreach (var file in Directory.GetFiles(dir, "*.cs"))
            {
                bool classFound = false;
                bool isDbGenerated = false;
                foreach (var line in File.ReadLines(file))
                {
                    var classMatch = Regex.Match(line, @"public\W*class\W*(\w*).*");
                    if (!classFound && classMatch.Groups.Count == 2)
                    {
                        if (classMatch.Groups[1].ToString() != className)
                        {
                            break;
                        }
                        //Console.WriteLine($"    Found EF class {className} in file {file}");
                        efClass.Name = className;
                        classFound = true;
                        continue;
                    }

                    var match = Regex.Match(line, "DatabaseGenerated");
                    if (match.Success)
                    {
                        isDbGenerated = true;
                        continue;
                    }

                    match = Regex.Match(line, @"public\W+(\w+\?*)\W+(\w+)\W+{\W*get;\W*set;\W*}");
                    if (match.Groups.Count == 3)
                    {
                        string type = match.Groups[1].ToString();
                        string name = match.Groups[2].ToString();
                        efClass.Properties.Add(new EfClassProperty()
                        {
                            Name = name,
                            Type = type,
                            IsEntity = IsEntity(type),
                            IsDbGenerated = isDbGenerated
                        });
                        isDbGenerated = false;
                    }
                }
                if (classFound)
                {
                    break;
                }
            }

            return efClass;
        }

        private static bool IsEntity(string type)
        {
            switch (type)
            {
                case "bool":
                case "bool?":
                case "int":
                case "int?":
                case "string":
                case "DateTime":
                case "DateTime?":
                case "double":
                case "double?":
                    return false;
                default:
                    return true;
            }
        }
    }
}