using System.Collections.Generic;
using System;
using DB = MyProject.Models;

namespace MyProject.Cqrs
{
    public class CompanyList
    {
        public List<Company> Companies { get; set; } = new List<Company>();
        
        public CompanyList(List<DB.Company> dbObjList)
        {
            foreach (var obj in dbObjList)
            {
                Companies.Add(new Company(obj));
            }
        }
    }
}
