namespace TalosCore.Generators
{
    public class NameGenerator
    {
        public string GetCqrsNamespace(string projName)
        {
            return $"{projName}.Cqrs";
        }

        public string GetCqrsType(string projName)
        {
            return $"{GetCqrsAlias()}.{projName}";
        }

        public string GetCqrsAlias()
        {
            return "Cqrs";
        }

        public string GetCqrsGetListResponseClass(string className)
        {
            return $"{className}List";
        }

        public string GetCqrsGetResponseClass(string className)
        {
            return className;
        }

        public string GetCqrsCreateCommandClass(string className)
        {
            return $"Create{className}Command";
        }

        public string GetCqrsUpdateCommandClass(string className)
        {
            return $"Update{className}Command";
        }

        public string GetCqrsDir(string dir)
        {
            dir = dir.TrimEnd('\\');
            return dir + @"\" + "Cqrs" + @"\";
        }
    }
}