using System;
using DB = MyProject.Models;

namespace MyProject.Cqrs
{
    public class UpdateAccountManagerCommand
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public DateTime? DeactivatedUtc { get; set; }
        
        public UpdateAccountManagerCommand(DB.AccountManager entity)
        {
            Id = entity.Id;
            FirstName = entity.FirstName;
            LastName = entity.LastName;
            CreatedUtc = entity.CreatedUtc;
            UpdatedUtc = entity.UpdatedUtc;
            DeactivatedUtc = entity.DeactivatedUtc;
        }
    }
}
