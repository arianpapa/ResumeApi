
using DAL;
using DAL.Models;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ResumeApi.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class PeopleController : ControllerBase
    {
        private AppDbContext _dbContext { get; set; }
        public PeopleController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Authorize(Policy = "get_list_of_persons")]
        public IList<Person> GetPeople()
        {
            var listOfPersons = _dbContext.Persons.ToList();
            return listOfPersons;
        }

        [Authorize(Policy = "create_person")]
        [HttpPost]
        public Person CreatePerson(Person person)
        {
            _dbContext.Persons.Add(person);
            _dbContext.SaveChanges();
            return person;
        }

        
        [HttpPut("{id}")]
        [Authorize(Policy = "update_person")]
        public async Task<IActionResult> ChangePerson(int id, Person person)
        {
            var PersonDb = _dbContext.Persons.Where(p => p.Id == id).FirstOrDefault<Person>();

            if (PersonDb == null)
            {
                return BadRequest();
            }

            PersonDb.Adress = person.Adress;
            PersonDb.City = person.City;
            PersonDb.Country = person.Country;
            PersonDb.DateOfBirth = person.DateOfBirth;
            PersonDb.Name = person.Name;
            PersonDb.Surname = person.Surname;
            PersonDb.Nid = person.Nid;
            PersonDb.PlaceOfBirth = person.PlaceOfBirth;

            _dbContext.Update(PersonDb);


            // _dbcontext.Entry(person).State = EntityState.Modified;


            _dbContext.SaveChanges();
            return Ok("Person modified successfully!");
        }


        [HttpDelete("{id}")]
        [Authorize(Policy = "delete_person")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            try
            {
                var person = await _dbContext.Persons.FindAsync(id);
                if (person == null)
                {
                    return NotFound();
                }

                _dbContext.Persons.Remove(person);
                await _dbContext.SaveChangesAsync();

                return Ok("Person successfully deleted!");

            }
            catch (Exception ex)
            {
                throw;
            }

        }

    }
}
