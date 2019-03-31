using System;
using System.Linq;

namespace TalosCore.Generators
{
    public class ControllerGenerator
    {
        private FileWriter _fileWriter;
        private NameGenerator _nameGenerator;
        private EfClassInfoList _efClassInfoList;

        public ControllerGenerator(EfClassInfoList efClassInfoList, FileWriter fileWriter, NameGenerator nameGenerator)
        {
            _nameGenerator = nameGenerator;
            _fileWriter = fileWriter;
            _efClassInfoList = efClassInfoList;
        }

        public void GenerateControllers(string dir, string projName)
        {
            dir = dir.TrimEnd('\\');

            // Create controller directory if needed
            string controllerDir = dir + @"\" + "Controllers" + @"\";
            _fileWriter.CreateDirectory(controllerDir);

            // Add controller classes and actions
            foreach (var efClass in _efClassInfoList.EfClasses)
            {
                AddControllerClass(efClass, projName, _efClassInfoList.DbContextName, controllerDir);
            }
        }

        private void AddControllerClass(EfClass efClass, string projName, string contextName,
            string controllerDir)
        {
            Console.WriteLine($"Creating controller class {efClass.Name}");

            string controllerClassName = efClass.Name + "Controller";

            // Create file
            string filename = $"{controllerDir}{controllerClassName}.cs";
            if (_fileWriter.Exists(filename))
            {
                Console.WriteLine($"Controller already exists...skipping: {filename}");
                return;
            }

            // Add controller header
            AddControllerHeader(efClass, projName, contextName, controllerClassName, filename);

            // Add controller actions
            AddControllerActions(efClass, filename);

            // Add controller footer
            CloseControllerFile(filename);
        }

        private void AddControllerActions(EfClass efClass, string filename)
        {
            AddGetListAction(efClass, filename);
            AddGetAction(efClass, filename);
            AddCreateAction(efClass, filename);
            AddUpdateAction(efClass, filename);
            AddDeleteAction(efClass, filename);
        }

        private void AddControllerHeader(EfClass efClass, string projName, string contextName, string controllerClassName, string filename)
        {
            AddControllerUsings(projName, filename);
            AddControllerNamespace(projName, filename);
            AddControllerClassRoute(efClass, projName, filename);
            AddControllerClassDeclarationHeader(controllerClassName, filename);
            string dbContextTypeWithAlias = AddControllerDbContextProperty(contextName, filename);
            AddControllerConstructor(controllerClassName, filename, dbContextTypeWithAlias);
        }

        private void CloseControllerFile(string filename)
        {
            _fileWriter.WriteStringToFile("}", filename);
            _fileWriter.WriteStringToFile("}", filename);
            _fileWriter.Close(filename);
        }

        private void AddControllerConstructor(string controllerClassName, string filename, string dbContextTypeWithAlias)
        {
            _fileWriter.WriteStringToFile("", filename);
            _fileWriter.WriteStringToFile($"public {controllerClassName}({dbContextTypeWithAlias} context)", filename);
            _fileWriter.WriteStringToFile("{", filename);
            _fileWriter.WriteStringToFile("_context = context;", filename);
            _fileWriter.WriteStringToFile("}", filename);
            _fileWriter.WriteStringToFile("", filename);
        }

        private string AddControllerDbContextProperty(string contextName, string filename)
        {
            string dbContextTypeWithAlias = GetDbTypeWithAlias(contextName);
            _fileWriter.WriteStringToFile($"{dbContextTypeWithAlias} _context;", filename);
            return dbContextTypeWithAlias;
        }

        private void AddControllerClassDeclarationHeader(string controllerClassName, string filename)
        {
            _fileWriter.WriteStringToFile($"public class {controllerClassName} : Controller", filename);
            _fileWriter.WriteStringToFile("{", filename);
        }

        private void AddControllerClassRoute(EfClass efClass, string projName, string filename)
        {
            _fileWriter.WriteStringToFile("[Route(\"api/" + projName + "/" + efClass.Name + "/\")]",
                filename);
        }

        private void AddControllerNamespace(string projName, string filename)
        {
            _fileWriter.WriteStringToFile($"namespace {projName}.Controllers", filename);
            _fileWriter.WriteStringToFile("{", filename);
        }

        private void AddControllerUsings(string projName, string filename)
        {
            _fileWriter.WriteStringToFile("using System;", filename);
            _fileWriter.WriteStringToFile("using System.Collections.Generic;", filename);
            _fileWriter.WriteStringToFile("using System.Linq;", filename);
            _fileWriter.WriteStringToFile("using System.Threading.Tasks;", filename);
            _fileWriter.WriteStringToFile("using Microsoft.AspNetCore.Mvc;", filename);
            _fileWriter.WriteStringToFile("using Microsoft.EntityFrameworkCore;", filename);
            _fileWriter.WriteStringToFile($"using DB = {_efClassInfoList.DbNamespace};", filename);
            _fileWriter.WriteStringToFile("", filename);
        }

        private string GetDbTypeWithAlias(string contextName)
        {
            return "DB." + contextName;
        }

        private void AddGetListAction(EfClass efClass, string controllerFile)
        {
            string getListMethod = $"Get{efClass.Name}List";
            string cqrsClassName = _nameGenerator.GetCqrsAlias() + "." + _nameGenerator.GetCqrsGetListResponseClass(efClass.Name);

            _fileWriter.WriteStringToFile("[HttpGet(\"" + getListMethod + "\")]", controllerFile);
            _fileWriter.WriteStringToFile($"[Produces(\"application/json\", Type = typeof(List<{cqrsClassName}>))]", controllerFile);
            _fileWriter.WriteStringToFile($"public async Task<IActionResult> {getListMethod}()", controllerFile);
            _fileWriter.WriteStringToFile("{", controllerFile);
            _fileWriter.WriteStringToFile($"var list = await _context.{efClass.CollectionName}.ToListAsync();", controllerFile);
            _fileWriter.WriteStringToFile("return Ok(list);", controllerFile);
            _fileWriter.WriteStringToFile("}", controllerFile);
        }

        private void AddGetAction(EfClass efClass, string filename)
        {
            string methodName = "Get" + efClass.Name;
            string cqrsClassName = _nameGenerator.GetCqrsAlias() + "." + _nameGenerator.GetCqrsGetResponseClass(efClass.Name);

            _fileWriter.WriteStringToFile("", filename);
            _fileWriter.WriteStringToFile("[HttpGet(\"" + methodName + "\")]", filename);
            _fileWriter.WriteStringToFile($"[Produces(\"application/json\", Type = typeof({cqrsClassName}))]", filename);
            _fileWriter.WriteStringToFile($"public async Task<IActionResult> {methodName}(int id)", filename);
            _fileWriter.WriteStringToFile("{", filename);
            _fileWriter.WriteStringToFile($"var entity = await _context.{efClass.CollectionName}", filename);
            _fileWriter.WriteStringToFile($".Where(x => x.Id == id).FirstOrDefaultAsync();", filename);
            _fileWriter.WriteStringToFile($"if (entity == null)", filename);
            _fileWriter.WriteStringToFile("{", filename);
            _fileWriter.WriteStringToFile("return NotFound(id);", filename);
            _fileWriter.WriteStringToFile("}", filename);
            _fileWriter.WriteStringToFile($"return Ok(new {cqrsClassName}(entity));", filename);
            _fileWriter.WriteStringToFile("}", filename);
        }

        private void AddCreateAction(EfClass efClass, string filename)
        {
            string methodName = "Create" + efClass.Name;
            var className = _nameGenerator.GetCqrsCreateCommandClass(efClass.Name);

            _fileWriter.WriteStringToFile("", filename);
            _fileWriter.WriteStringToFile("[HttpPost(\"" + methodName + "\")]", filename);
            _fileWriter.WriteStringToFile($"public async Task<IActionResult> {methodName}([FromBody]{_nameGenerator.GetCqrsAlias()}.{className} command)", filename);
            _fileWriter.WriteStringToFile("{", filename);
            _fileWriter.WriteStringToFile($"var entity = new DB.{efClass.Name}();", filename);
            _fileWriter.WriteStringToFile("", filename);

            foreach (var prop in efClass.Properties)
            {
                if (prop.IsDbGenerated)
                {
                    continue;
                }

                if (prop.IsEntity)
                {
                    var entityClass = _efClassInfoList.EfClasses.Where(e => e.Name == prop.Name).FirstOrDefault();
                    if (entityClass != null)
                    {
                        var entityVarName = "var" + entityClass.Name;

                        _fileWriter.WriteStringToFile($"var {entityVarName} = await _context.{entityClass.CollectionName}" +
                            $".Where(x => x.Id == command.{entityClass.Name}Id).FirstOrDefaultAsync();", filename);
                        _fileWriter.WriteStringToFile($"if ({entityVarName} == null)", filename);
                        _fileWriter.WriteStringToFile("{", filename);
                        _fileWriter.WriteStringToFile("return NotFound(\"" + $"{entityClass.Name}Id = \" + " + $"command.{entityClass.Name}Id);", filename);
                        _fileWriter.WriteStringToFile("}", filename);
                        _fileWriter.WriteStringToFile($"entity.{entityClass.Name} = {entityVarName};", filename);
                    }
                    continue;
                }

                _fileWriter.WriteStringToFile($"entity.{prop.Name} = command.{prop.Name};", filename);
            }
            _fileWriter.WriteStringToFile("", filename);

            _fileWriter.WriteStringToFile($"_context.{efClass.CollectionName}.Add(entity);", filename);
            _fileWriter.WriteStringToFile("await _context.SaveChangesAsync(); ", filename);
            _fileWriter.WriteStringToFile("", filename);

            _fileWriter.WriteStringToFile($"return Ok();", filename);
            _fileWriter.WriteStringToFile("}", filename);
        }

        private void AddUpdateAction(EfClass efClass, string filename)
        {
            string methodName = "Update" + efClass.Name;
            var className = _nameGenerator.GetCqrsUpdateCommandClass(efClass.Name);

            _fileWriter.WriteStringToFile("", filename);
            _fileWriter.WriteStringToFile("[HttpPut(\"" + methodName + "\")]", filename);
            _fileWriter.WriteStringToFile($"public async Task<IActionResult> {methodName}([FromBody]{_nameGenerator.GetCqrsAlias()}.{className} command)", filename);
            _fileWriter.WriteStringToFile("{", filename);
            _fileWriter.WriteStringToFile($"var entity = await _context.{efClass.CollectionName}.Where(x => x.Id == command.Id).FirstOrDefaultAsync();", filename);
            _fileWriter.WriteStringToFile($"if (entity == null)", filename);
            _fileWriter.WriteStringToFile("{", filename);
            _fileWriter.WriteStringToFile($"return NotFound(command.Id);", filename);
            _fileWriter.WriteStringToFile("}", filename);
            _fileWriter.WriteStringToFile("", filename);

            foreach (var prop in efClass.Properties)
            {
                if (prop.IsDbGenerated)
                {
                    continue;
                }

                if (prop.IsEntity)
                {
                    var entityClass = _efClassInfoList.EfClasses.Where(e => e.Name == prop.Name).FirstOrDefault();
                    if (entityClass != null)
                    {
                        var entityVarName = "var" + entityClass.Name;

                        _fileWriter.WriteStringToFile($"var {entityVarName} = await _context.{entityClass.CollectionName}" +
                            $".Where(x => x.Id == command.{entityClass.Name}Id).FirstOrDefaultAsync();", filename);
                        _fileWriter.WriteStringToFile($"if ({entityVarName} == null)", filename);
                        _fileWriter.WriteStringToFile("{", filename);
                        _fileWriter.WriteStringToFile("return NotFound(\"" + $"{entityClass.Name}Id = \" + " + $"command.{entityClass.Name}Id);", filename);
                        _fileWriter.WriteStringToFile("}", filename);
                        _fileWriter.WriteStringToFile($"entity.{entityClass.Name} = {entityVarName};", filename);
                    }
                    continue;
                }

                _fileWriter.WriteStringToFile($"entity.{prop.Name} = command.{prop.Name};", filename);
            }

            _fileWriter.WriteStringToFile("", filename);
            _fileWriter.WriteStringToFile("await _context.SaveChangesAsync(); ", filename);

            _fileWriter.WriteStringToFile("", filename);
            _fileWriter.WriteStringToFile("return Ok();", filename);
            _fileWriter.WriteStringToFile("}", filename);
        }

        private void AddDeleteAction(EfClass efClass, string filename)
        {
            _fileWriter.WriteStringToFile("", filename);
        }
    }
}