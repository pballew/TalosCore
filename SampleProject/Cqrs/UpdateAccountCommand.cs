using System;
using DB = MyProject.Models;

namespace MyProject.Cqrs
{
    public class UpdateAccountCommand
    {
        public int Id { get; set; }
        public bool IsElite { get; set; }
        public int AccountManagerId { get; set; }
        public DateTime? LastOrder { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public DateTime? DeactivatedUtc { get; set; }
        
        public UpdateAccountCommand(DB.Account entity)
        {
            Id = entity.Id;
            IsElite = entity.IsElite;
            AccountManagerId = entity.AccountManager.Id;
            LastOrder = entity.LastOrder;
            CreatedUtc = entity.CreatedUtc;
            UpdatedUtc = entity.UpdatedUtc;
            DeactivatedUtc = entity.DeactivatedUtc;
        }
    }
}
