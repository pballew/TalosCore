using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DB = MyProject.Models;

namespace MyProject.Controllers
{
    [Route("api/MyProject/Account/")]
    public class AccountController : Controller
    {
        DB.EmailContext _context;
        
        public AccountController(DB.EmailContext context)
        {
            _context = context;
        }
        
        [HttpGet("GetAccountList")]
        [Produces("application/json", Type = typeof(List<Cqrs.AccountList>))]
        public async Task<IActionResult> GetAccountList()
        {
            var list = await _context.Accounts.ToListAsync();
            return Ok(list);
        }
        
        [HttpGet("GetAccount")]
        [Produces("application/json", Type = typeof(Cqrs.Account))]
        public async Task<IActionResult> GetAccount(int id)
        {
            var entity = await _context.Accounts
            .Where(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null)
            {
                return NotFound(id);
            }
            return Ok(new Cqrs.Account(entity));
        }
        
        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateAccount([FromBody]Cqrs.CreateAccountCommand command)
        {
            var entity = new DB.Account();
            
            entity.IsElite = command.IsElite;
            var varAccountManager = await _context.AccountManagers.Where(x => x.Id == command.AccountManagerId).FirstOrDefaultAsync();
            if (varAccountManager == null)
            {
                return NotFound("AccountManagerId = " + command.AccountManagerId);
            }
            entity.AccountManager = varAccountManager;
            entity.LastOrder = command.LastOrder;
            entity.CreatedUtc = command.CreatedUtc;
            entity.UpdatedUtc = command.UpdatedUtc;
            entity.DeactivatedUtc = command.DeactivatedUtc;
            
            _context.Accounts.Add(entity);
            await _context.SaveChangesAsync();
            
            return Ok();
        }
        
        [HttpPut("UpdateAccount")]
        public async Task<IActionResult> UpdateAccount([FromBody]Cqrs.UpdateAccountCommand command)
        {
            var entity = await _context.Accounts.Where(x => x.Id == command.Id).FirstOrDefaultAsync();
            if (entity == null)
            {
                return NotFound(command.Id);
            }
            
            entity.IsElite = command.IsElite;
            var varAccountManager = await _context.AccountManagers.Where(x => x.Id == command.AccountManagerId).FirstOrDefaultAsync();
            if (varAccountManager == null)
            {
                return NotFound("AccountManagerId = " + command.AccountManagerId);
            }
            entity.AccountManager = varAccountManager;
            entity.LastOrder = command.LastOrder;
            entity.CreatedUtc = command.CreatedUtc;
            entity.UpdatedUtc = command.UpdatedUtc;
            entity.DeactivatedUtc = command.DeactivatedUtc;
            
            await _context.SaveChangesAsync();
            
            return Ok();
        }
        
    }
}
