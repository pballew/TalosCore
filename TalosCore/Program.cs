using System;
using TalosCore.Generators;

namespace TalosCore
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Usage();
                return;
            }

            string projDir = args[0];


            // TODO: get projname from project
            Console.WriteLine("Searching for EF project name");
            string projName = EFParser.GetProjectName(projDir);

            Console.WriteLine("Searching for EF class source files");
            EfClassInfoList classInfoList = EFParser.GetEfClassInfoList(projDir);

            Console.WriteLine("Creating Cqrs classes");
            var cqrsGen = new CqrsGenerator(new FileWriter(), new NameGenerator());
            cqrsGen.GenerateClasses(classInfoList, projDir, projName);

            Console.WriteLine("Creating controller classes");
            var cg = new ControllerGenerator(classInfoList, new FileWriter(), new NameGenerator());
            cg.GenerateControllers(projDir, projName);
        }

        static void Usage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("dotnet TalosCore.dll <path-to-core-ef-project>");
        }
    }
}