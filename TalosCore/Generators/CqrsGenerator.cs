namespace TalosCore.Generators
{
    public class CqrsGenerator
    {
        private FileWriter _fileWriter;
        private NameGenerator _nameGenerator;
        private string _fileDir;

        public CqrsGenerator(FileWriter fileWriter, NameGenerator nameGenerator)
        {
            _fileWriter = fileWriter;
            _nameGenerator = nameGenerator;
        }

        public void GenerateClasses(EfClassInfoList efClassInfoList, string dir, string projName)
        {
            _fileDir = _nameGenerator.GetCqrsDir(dir);
            _fileWriter.CreateDirectory(_fileDir);

            foreach (var efClass in efClassInfoList.EfClasses)
            {
                AddClasses(efClass, projName, efClassInfoList.DbNamespace);
            }
        }

        private void AddClasses(EfClass efClass, string projName, string dbNamespace)
        {
            AddGetClass(_nameGenerator.GetCqrsGetResponseClass(efClass.Name), efClass, projName, dbNamespace);
            AddGetListClass(_nameGenerator.GetCqrsGetListResponseClass(efClass.Name), efClass, projName, dbNamespace);
            AddCreateClass(_nameGenerator.GetCqrsCreateCommandClass(efClass.Name), efClass, projName, dbNamespace);
            AddUpdateClass(_nameGenerator.GetCqrsUpdateCommandClass(efClass.Name), efClass, projName, dbNamespace);
        }

        private void AddGetClass(string className, EfClass efClass, string projName, string dbNamespace)
        {
            string filename = $"{_fileDir}{className}.cs";

            WriteClassHeader(className, projName, dbNamespace, filename);
            WriteIdProperty(filename);
            WriteProperties(efClass, filename);
            WriteConvertConstructor(className, efClass, filename);
            WriteClassFooter(filename);
        }

        private void AddGetListClass(string className, EfClass efClass, string projName, string dbNamespace)
        {
            string filename = $"{_fileDir}{className}.cs";

            _fileWriter.WriteStringToFile("using System.Collections.Generic;", filename);
            WriteClassHeader(className, projName, dbNamespace, filename);
            WriteListProperty(efClass, filename);
            WriteListConvertConstructor(efClass, filename);
            WriteClassFooter(filename);
        }

        private void AddCreateClass(string className, EfClass efClass, string projName, string dbNamespace)
        {
            string filename = $"{_fileDir}{className}.cs";

            WriteClassHeader(className, projName, dbNamespace, filename);
            WriteProperties(efClass, filename);
            WriteCreateConvertConstructor(className, efClass, filename);
            WriteClassFooter(filename);
        }

        private void AddUpdateClass(string className, EfClass efClass, string projName, string dbNamespace)
        {
            string filename = $"{_fileDir}{className}.cs";

            WriteClassHeader(className, projName, dbNamespace, filename);
            WriteIdProperty(filename);
            WriteProperties(efClass, filename);
            WriteConvertConstructor(className, efClass, filename);
            WriteClassFooter(filename);
        }

        private void WriteConvertConstructor(string className, EfClass efClass, string filename)
        {
            _fileWriter.WriteStringToFile("", filename);
            _fileWriter.WriteStringToFile($"public {className}(DB.{efClass.Name} entity)", filename);
            _fileWriter.WriteStringToFile("{", filename);
            foreach (var prop in efClass.Properties)
            {
                if (prop.IsEntity)
                {
                    _fileWriter.WriteStringToFile($"{prop.Name}Id = entity.{prop.Name}.Id;", filename);
                }
                else
                {
                    _fileWriter.WriteStringToFile($"{prop.Name} = entity.{prop.Name};", filename);
                }
            }
            _fileWriter.WriteStringToFile("}", filename);
        }

        private void WriteCreateConvertConstructor(string className, EfClass efClass, string filename)
        {
            _fileWriter.WriteStringToFile("", filename);
            _fileWriter.WriteStringToFile($"public {className}(DB.{efClass.Name} entity)", filename);
            _fileWriter.WriteStringToFile("{", filename);
            foreach (var prop in efClass.Properties)
            {
                if (prop.Name == "Id")
                {
                    continue;
                }
                if (prop.IsEntity)
                {
                    _fileWriter.WriteStringToFile($"{prop.Name}Id = entity.{prop.Name}.Id;", filename);
                }
                else
                {
                    _fileWriter.WriteStringToFile($"{prop.Name} = entity.{prop.Name};", filename);
                }
            }
            _fileWriter.WriteStringToFile("}", filename);
        }

        private void WriteUpdateConvertConstructor(string className, EfClass efClass, string filename)
        {
            _fileWriter.WriteStringToFile("", filename);
            _fileWriter.WriteStringToFile($"public {className}(DB.{efClass.Name} entity)", filename);
            _fileWriter.WriteStringToFile("{", filename);
            foreach (var prop in efClass.Properties)
            {
                if (prop.IsEntity)
                {
                    _fileWriter.WriteStringToFile($"{prop.Name}Id = entity.{prop.Name}.Id;", filename);
                }
                else
                {
                    _fileWriter.WriteStringToFile($"{prop.Name} = entity.{prop.Name};", filename);
                }
            }
            _fileWriter.WriteStringToFile("}", filename);
        }

        private void WriteListConvertConstructor(EfClass efClass, string filename)
        {
            string responseListClassName = _nameGenerator.GetCqrsGetListResponseClass(efClass.Name);
            string responseClassName = _nameGenerator.GetCqrsGetResponseClass(efClass.Name);

            _fileWriter.WriteStringToFile("", filename);
            _fileWriter.WriteStringToFile($"public {responseListClassName}(List<DB.{efClass.Name}> dbObjList)", filename);
            _fileWriter.WriteStringToFile("{", filename);
            _fileWriter.WriteStringToFile("foreach (var obj in dbObjList)", filename);
            _fileWriter.WriteStringToFile("{", filename);
            _fileWriter.WriteStringToFile($"{efClass.CollectionName}.Add(new {responseClassName}(obj));", filename);
            _fileWriter.WriteStringToFile("}", filename);
            _fileWriter.WriteStringToFile("}", filename);
        }

        private void WriteListProperty(EfClass efClass, string filename)
        {
            string listResponseType = $"List<{_nameGenerator.GetCqrsGetResponseClass(efClass.Name)}>";
            string propString = $"public {listResponseType} {efClass.CollectionName}" + " { get; set; }" + $" = new {listResponseType}();";
            _fileWriter.WriteStringToFile(propString, filename);
        }

        private void WriteClassHeader(string className, string projName, string dbNamespace, string filename)
        {
            WriteUsings(filename, dbNamespace);
            WriteNamespace(filename, projName);
            WriteClassHeader(className, filename);
        }

        private void WriteNamespace(string filename, string projName)
        {
            _fileWriter.WriteStringToFile("", filename);
            _fileWriter.WriteStringToFile($"namespace {_nameGenerator.GetCqrsNamespace(projName)}", filename);
            _fileWriter.WriteStringToFile("{", filename);
        }

        private void WriteUsings(string filename, string dbNamespace)
        {
            _fileWriter.WriteStringToFile("using System;", filename);
            _fileWriter.WriteStringToFile($"using DB = {dbNamespace};", filename);
        }

        private void WriteClassHeader(string className, string filename)
        {
            _fileWriter.WriteStringToFile($"public class {className}", filename);
            _fileWriter.WriteStringToFile("{", filename);
        }

        private void WriteIdProperty(string filename)
        {
            _fileWriter.WriteStringToFile("public int Id { get; set; }", filename);
        }

        private void WriteProperties(EfClass efClass, string filename)
        {
            foreach (var prop in efClass.Properties)
            {
                if (prop.Name == "Id")
                {
                    continue;
                }
                if (prop.IsEntity)
                {
                    _fileWriter.WriteStringToFile($"public int {prop.Name}Id" + " { get; set; }", filename);
                }
                else
                {
                    _fileWriter.WriteStringToFile($"public {prop.Type} {prop.Name}" + " { get; set; }", filename);
                }
            }
        }

        private void WriteClassFooter(string filename)
        {
            _fileWriter.WriteStringToFile("}", filename);
            _fileWriter.WriteStringToFile("}", filename);
            _fileWriter.Close(filename);
        }
    }
}