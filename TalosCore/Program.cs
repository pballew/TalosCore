using McMaster.Extensions.CommandLineUtils;
using System;
using System.ComponentModel.DataAnnotations;
using TalosCore.Generators;

namespace TalosCore
{
    class Program
    {
        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        [Argument(0, Description = "A project directory must be specified.")]
        [Required]
        public string StartDirectory { get; }

        private int OnExecute()
        {
            try
            {
                Console.WriteLine("Searching for project path");
                string projectPath = EFParser.GetProjectPath(StartDirectory);

                Console.WriteLine("Searching for EF project name");
                string projectNamespace = EFParser.GetProjectName(projectPath);

                Console.WriteLine("Searching for EF class source files");
                EfClassInfoList classInfoList = EFParser.GetEfClassInfoList(projectPath);

                Console.WriteLine("Creating Cqrs classes");
                var cqrsGen = new CqrsGenerator(new FileWriter(), new NameGenerator());
                cqrsGen.GenerateClasses(classInfoList, projectPath, projectNamespace);

                Console.WriteLine("Creating controller classes");
                var cg = new ControllerGenerator(classInfoList, new FileWriter(), new NameGenerator());
                cg.GenerateControllers(projectPath, projectNamespace);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return 0;
        }
    }
}