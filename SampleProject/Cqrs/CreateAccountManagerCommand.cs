using System;
using DB = MyProject.Models;

namespace MyProject.Cqrs
{
    public class CreateAccountManagerCommand
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public DateTime? DeactivatedUtc { get; set; }
        
        public CreateAccountManagerCommand(DB.AccountManager entity)
        {
            FirstName = entity.FirstName;
            LastName = entity.LastName;
            CreatedUtc = entity.CreatedUtc;
            UpdatedUtc = entity.UpdatedUtc;
            DeactivatedUtc = entity.DeactivatedUtc;
        }
    }
}
