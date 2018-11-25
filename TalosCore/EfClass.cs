using System.Collections.Generic;

namespace TalosCore
{
    public class EfClass
    {
        public string Name { get; set; }
        public List<EfClassProperty> Properties { get; set; } = new List<EfClassProperty>();
        public string CollectionName { get; internal set; }
    }

    public class EfClassProperty
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public bool IsEntity { get; set; }
        public bool IsDbGenerated { get; set; }
    }
}