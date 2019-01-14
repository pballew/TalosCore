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
        public string ProjectDirectory { get; }

        private int OnExecute()
        {
            // TODO: get projname from project
            Console.WriteLine("Searching for EF project name");
            string projName = EFParser.GetProjectName(ProjectDirectory);

            Console.WriteLine("Searching for EF class source files");
            EfClassInfoList classInfoList = EFParser.GetEfClassInfoList(ProjectDirectory);

            Console.WriteLine("Creating Cqrs classes");
            var cqrsGen = new CqrsGenerator(new FileWriter(), new NameGenerator());
            cqrsGen.GenerateClasses(classInfoList, ProjectDirectory, projName);

            Console.WriteLine("Creating controller classes");
            var cg = new ControllerGenerator(classInfoList, new FileWriter(), new NameGenerator());
            cg.GenerateControllers(ProjectDirectory, projName);

            return 0;
        }
    }
}