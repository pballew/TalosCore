using System.Collections.Generic;
using System;
using DB = MyProject.Models;

namespace MyProject.Cqrs
{
    public class AccountManagerList
    {
        public List<AccountManager> AccountManagers { get; set; } = new List<AccountManager>();
        
        public AccountManagerList(List<DB.AccountManager> dbObjList)
        {
            foreach (var obj in dbObjList)
            {
                AccountManagers.Add(new AccountManager(obj));
            }
        }
    }
}
