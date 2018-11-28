using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DB = MyProject.Models;

namespace MyProject.Controllers
{
    [Route("api/MyProject/Company/")]
    public class CompanyController : Controller
    {
        DB.EmailContext _context;
        
        public CompanyController(DB.EmailContext context)
        {
            _context = context;
        }
        
        [HttpGet("GetCompanyList")]
        [Produces("application/json", Type = typeof(List<Cqrs.CompanyList>))]
        public async Task<IActionResult> GetCompanyList()
        {
            var list = await _context.Companies.ToListAsync();
            return Ok(list);
        }
        
        [HttpGet("GetCompany")]
        [Produces("application/json", Type = typeof(Cqrs.Company))]
        public async Task<IActionResult> GetCompany(int id)
        {
            var entity = await _context.Companies
            .Where(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null)
            {
                return NotFound(id);
            }
            return Ok(new Cqrs.Company(entity));
        }
        
        [HttpPost("CreateCompany")]
        public async Task<IActionResult> CreateCompany([FromBody]Cqrs.CreateCompanyCommand command)
        {
            var entity = new DB.Company();
            
            entity.Name = command.Name;
            entity.Address1 = command.Address1;
            entity.Address2 = command.Address2;
            entity.City = command.City;
            entity.State = command.State;
            entity.Zip = command.Zip;
            entity.Country = command.Country;
            entity.CreatedUtc = command.CreatedUtc;
            entity.UpdatedUtc = command.UpdatedUtc;
            
            _context.Companies.Add(entity);
            await _context.SaveChangesAsync();
            
            return Ok();
        }
        
        [HttpPut("UpdateCompany")]
        public async Task<IActionResult> UpdateCompany([FromBody]Cqrs.UpdateCompanyCommand command)
        {
            var entity = await _context.Companies.Where(x => x.Id == command.Id).FirstOrDefaultAsync();
            if (entity == null)
            {
                return NotFound(command.Id);
            }
            
            entity.Name = command.Name;
            entity.Address1 = command.Address1;
            entity.Address2 = command.Address2;
            entity.City = command.City;
            entity.State = command.State;
            entity.Zip = command.Zip;
            entity.Country = command.Country;
            entity.CreatedUtc = command.CreatedUtc;
            entity.UpdatedUtc = command.UpdatedUtc;
            
            await _context.SaveChangesAsync();
            
            return Ok();
        }
        
    }
}
