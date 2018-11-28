using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DB = MyProject.Models;

namespace MyProject.Controllers
{
    [Route("api/MyProject/Customer/")]
    public class CustomerController : Controller
    {
        DB.EmailContext _context;
        
        public CustomerController(DB.EmailContext context)
        {
            _context = context;
        }
        
        [HttpGet("GetCustomerList")]
        [Produces("application/json", Type = typeof(List<Cqrs.CustomerList>))]
        public async Task<IActionResult> GetCustomerList()
        {
            var list = await _context.Customers.ToListAsync();
            return Ok(list);
        }
        
        [HttpGet("GetCustomer")]
        [Produces("application/json", Type = typeof(Cqrs.Customer))]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var entity = await _context.Customers
            .Where(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null)
            {
                return NotFound(id);
            }
            return Ok(new Cqrs.Customer(entity));
        }
        
        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> CreateCustomer([FromBody]Cqrs.CreateCustomerCommand command)
        {
            var entity = new DB.Customer();
            
            entity.Title = command.Title;
            entity.FullName = command.FullName;
            entity.FirstName = command.FirstName;
            entity.LastName = command.LastName;
            var varCompany = await _context.Companies.Where(x => x.Id == command.CompanyId).FirstOrDefaultAsync();
            if (varCompany == null)
            {
                return NotFound("CompanyId = " + command.CompanyId);
            }
            entity.Company = varCompany;
            var varAccount = await _context.Accounts.Where(x => x.Id == command.AccountId).FirstOrDefaultAsync();
            if (varAccount == null)
            {
                return NotFound("AccountId = " + command.AccountId);
            }
            entity.Account = varAccount;
            entity.CreatedUtc = command.CreatedUtc;
            entity.UpdatedUtc = command.UpdatedUtc;
            
            _context.Customers.Add(entity);
            await _context.SaveChangesAsync();
            
            return Ok();
        }
        
        [HttpPut("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer([FromBody]Cqrs.UpdateCustomerCommand command)
        {
            var entity = await _context.Customers.Where(x => x.Id == command.Id).FirstOrDefaultAsync();
            if (entity == null)
            {
                return NotFound(command.Id);
            }
            
            entity.Title = command.Title;
            entity.FullName = command.FullName;
            entity.FirstName = command.FirstName;
            entity.LastName = command.LastName;
            var varCompany = await _context.Companies.Where(x => x.Id == command.CompanyId).FirstOrDefaultAsync();
            if (varCompany == null)
            {
                return NotFound("CompanyId = " + command.CompanyId);
            }
            entity.Company = varCompany;
            var varAccount = await _context.Accounts.Where(x => x.Id == command.AccountId).FirstOrDefaultAsync();
            if (varAccount == null)
            {
                return NotFound("AccountId = " + command.AccountId);
            }
            entity.Account = varAccount;
            entity.CreatedUtc = command.CreatedUtc;
            entity.UpdatedUtc = command.UpdatedUtc;
            
            await _context.SaveChangesAsync();
            
            return Ok();
        }
        
    }
}
