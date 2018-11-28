using System;
using DB = MyProject.Models;

namespace MyProject.Cqrs
{
    public class UpdateCustomerCommand
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CompanyId { get; set; }
        public int AccountId { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        
        public UpdateCustomerCommand(DB.Customer entity)
        {
            Id = entity.Id;
            Title = entity.Title;
            FullName = entity.FullName;
            FirstName = entity.FirstName;
            LastName = entity.LastName;
            CompanyId = entity.Company.Id;
            AccountId = entity.Account.Id;
            CreatedUtc = entity.CreatedUtc;
            UpdatedUtc = entity.UpdatedUtc;
        }
    }
}
