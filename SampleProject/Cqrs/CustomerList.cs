using System.Collections.Generic;
using System;
using DB = MyProject.Models;

namespace MyProject.Cqrs
{
    public class CustomerList
    {
        public List<Customer> Customers { get; set; } = new List<Customer>();
        
        public CustomerList(List<DB.Customer> dbObjList)
        {
            foreach (var obj in dbObjList)
            {
                Customers.Add(new Customer(obj));
            }
        }
    }
}
