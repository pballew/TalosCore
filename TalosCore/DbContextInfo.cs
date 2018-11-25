using System.Collections.Generic;

namespace TalosCore
{
    public class DbContextInfo
    {
        public string Name { get; set; }
        public List<EfClass> Classes { get; set; }
    }
}