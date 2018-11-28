using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DB = MyProject.Models;

namespace MyProject.Controllers
{
    [Route("api/MyProject/AccountManager/")]
    public class AccountManagerController : Controller
    {
        DB.EmailContext _context;
        
        public AccountManagerController(DB.EmailContext context)
        {
            _context = context;
        }
        
        [HttpGet("GetAccountManagerList")]
        [Produces("application/json", Type = typeof(List<Cqrs.AccountManagerList>))]
        public async Task<IActionResult> GetAccountManagerList()
        {
            var list = await _context.AccountManagers.ToListAsync();
            return Ok(list);
        }
        
        [HttpGet("GetAccountManager")]
        [Produces("application/json", Type = typeof(Cqrs.AccountManager))]
        public async Task<IActionResult> GetAccountManager(int id)
        {
            var entity = await _context.AccountManagers
            .Where(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null)
            {
                return NotFound(id);
            }
            return Ok(new Cqrs.AccountManager(entity));
        }
        
        [HttpPost("CreateAccountManager")]
        public async Task<IActionResult> CreateAccountManager([FromBody]Cqrs.CreateAccountManagerCommand command)
        {
            var entity = new DB.AccountManager();
            
            entity.FirstName = command.FirstName;
            entity.LastName = command.LastName;
            entity.CreatedUtc = command.CreatedUtc;
            entity.UpdatedUtc = command.UpdatedUtc;
            entity.DeactivatedUtc = command.DeactivatedUtc;
            
            _context.AccountManagers.Add(entity);
            await _context.SaveChangesAsync();
            
            return Ok();
        }
        
        [HttpPut("UpdateAccountManager")]
        public async Task<IActionResult> UpdateAccountManager([FromBody]Cqrs.UpdateAccountManagerCommand command)
        {
            var entity = await _context.AccountManagers.Where(x => x.Id == command.Id).FirstOrDefaultAsync();
            if (entity == null)
            {
                return NotFound(command.Id);
            }
            
            entity.FirstName = command.FirstName;
            entity.LastName = command.LastName;
            entity.CreatedUtc = command.CreatedUtc;
            entity.UpdatedUtc = command.UpdatedUtc;
            entity.DeactivatedUtc = command.DeactivatedUtc;
            
            await _context.SaveChangesAsync();
            
            return Ok();
        }
        
    }
}
