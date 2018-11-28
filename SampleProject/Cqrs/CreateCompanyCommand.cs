using System;
using DB = MyProject.Models;

namespace MyProject.Cqrs
{
    public class CreateCompanyCommand
    {
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime? UpdatedUtc { get; set; }
        
        public CreateCompanyCommand(DB.Company entity)
        {
            Name = entity.Name;
            Address1 = entity.Address1;
            Address2 = entity.Address2;
            City = entity.City;
            State = entity.State;
            Zip = entity.Zip;
            Country = entity.Country;
            CreatedUtc = entity.CreatedUtc;
            UpdatedUtc = entity.UpdatedUtc;
        }
    }
}
