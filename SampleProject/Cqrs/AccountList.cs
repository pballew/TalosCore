using System.Collections.Generic;
using System;
using DB = MyProject.Models;

namespace MyProject.Cqrs
{
    public class AccountList
    {
        public List<Account> Accounts { get; set; } = new List<Account>();
        
        public AccountList(List<DB.Account> dbObjList)
        {
            foreach (var obj in dbObjList)
            {
                Accounts.Add(new Account(obj));
            }
        }
    }
}
