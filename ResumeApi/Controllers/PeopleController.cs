
using DAL;
using DAL.Models;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResumeApi.Helpers;
using ResumeApi.Helpers.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ResumeApi.ViewModels;
using AutoMapper;

namespace ResumeApi.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class PeopleController : ControllerBase
    {
        private readonly IMapper _mapper;
        private AppDbContext _dbContext { get; set; }
        public PeopleController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }



        [HttpGet("getAllPeople")]
        [Authorize(Policy = "get_list_of_persons")]
        public IList<Person> GetPeople()
        {
            var listOfPersons = _dbContext.Persons.ToList();
            return listOfPersons;
        }


        [HttpPost("registerPerson")]
        [Authorize(Policy = "create_person")]
        public Person CreatePerson(PersonPostViewModel person)
        {
            var mappPersons = _mapper.Map<Person>(person);
            var p = _dbContext.Persons.Add(mappPersons);
            _dbContext.SaveChanges();
            return p.Entity;
        }


        [HttpPut("updatePerson/{id}")]
        [Authorize(Policy = "update_person")]
        //public async Task<IActionResult> ChangePerson(int id, Person person)
        //{
        //    var PersonDb = _dbContext.Persons.Where(p => p.Id == id).FirstOrDefault<Person>();

        //    if (PersonDb == null)
        //    {
        //        return BadRequest();
        //    }

        //    PersonDb.Adress = person.Adress;
        //    PersonDb.City = person.City;
        //    PersonDb.Country = person.Country;
        //    PersonDb.DateOfBirth = person.DateOfBirth;
        //    PersonDb.Name = person.Name;
        //    PersonDb.Surname = person.Surname;
        //    PersonDb.Nid = person.Nid;
        //    PersonDb.PlaceOfBirth = person.PlaceOfBirth;

        //    _dbContext.Update(PersonDb);

        //    _dbContext.SaveChanges();
        //    return Ok("Person modified successfully!");
        //}
        public async Task<IActionResult> ChangePerson(int id, PersonPutViewModel person)
        {
            var PersonDb = _dbContext.Persons.Where(p => p.Id == id).FirstOrDefault();

            if (PersonDb == null)
            {
                return BadRequest();
            }
            _mapper.Map(person, PersonDb);
            _dbContext.Update(PersonDb);
            

            _dbContext.SaveChanges();
            return Ok("Person modified successfully!");
        }


        [HttpDelete("deletePerson/{id}")]
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


        [HttpGet("getAllSkills")]
        [Authorize(Policies.ViewAllSkillsPolicy)]
        public IList<Skill> GetSkills()
        {
            var listOfSkills = _dbContext.Skills.ToList();
            return listOfSkills;
        }


        [HttpPost("registerSkill")]
        [Authorize(Policies.ManageAllSkillsPolicy)]
        public Skill CreateSkill(Skill skill)
        {
            _dbContext.Skills.Add(skill);
            _dbContext.SaveChanges();
            return skill;
        }


        [HttpPut("updateSkill/{id}")]
        [Authorize(Policies.ManageAllSkillsPolicy)]
        public async Task<IActionResult> ChangeSkill(int id, Skill skill)
        {
            var SkillDb = _dbContext.Skills.Where(p => p.Id == id).FirstOrDefault<Skill>();

            if (SkillDb == null)
            {
                return BadRequest();
            }

            SkillDb.Value = skill.Value;
            SkillDb.PersonId = skill.PersonId;
            SkillDb.IsSoftSkill = skill.IsSoftSkill;
            SkillDb.Level = skill.Level;

            _dbContext.Update(SkillDb);

            // _dbcontext.Entry(person).State = EntityState.Modified;

            _dbContext.SaveChanges();
            return Ok("Skill modified successfully!");
        }


        [HttpDelete("deleteSkill/{id}")]
        [Authorize(Policies.ManageAllSkillsPolicy)]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            try
            {
                var skill = await _dbContext.Skills.FindAsync(id);
                if (skill == null)
                {
                    return NotFound();
                }

                _dbContext.Skills.Remove(skill);
                await _dbContext.SaveChangesAsync();

                return Ok("Skill successfully deleted!");

            }
            catch (Exception ex)
            {
                throw;
            }

        }










        [HttpGet("getAllWorkingExperiences")]
        [Authorize(Policies.ViewAllWorkingExperiencesPolicy)]
        public IList<WorkingExperience> GetWorkingExperiences()
        {
            var listOfWorkingExperiences = _dbContext.WorkingExperiences.ToList();
            return listOfWorkingExperiences;
        }


        [HttpPost("registerWorkingExperience")]
        [Authorize(Policies.ManageAllWorkingExperiencesPolicy)]
        public WorkingExperience CreateWorkingExperience(WorkingExperience workingExperience)
        {
            _dbContext.WorkingExperiences.Add(workingExperience);
            _dbContext.SaveChanges();
            return workingExperience;
        }


        [HttpPut("updateWorkingExperience/{id}")]
        [Authorize(Policies.ManageAllWorkingExperiencesPolicy)]
        public async Task<IActionResult> ChangeWorkingExperience(int id, WorkingExperience workingExperience)
        {
            var WorkingExperienceDb = _dbContext.WorkingExperiences.Where(p => p.Id == id).FirstOrDefault<WorkingExperience>();

            if (WorkingExperienceDb == null)
            {
                return BadRequest();
            }

            WorkingExperienceDb.ShortDescription = workingExperience.ShortDescription;
            WorkingExperienceDb.OrganisationName = workingExperience.OrganisationName;
            WorkingExperienceDb.OrganisationAdress = workingExperience.OrganisationAdress;
            WorkingExperienceDb.StandartDate = workingExperience.StandartDate;
            WorkingExperienceDb.EndDate = workingExperience.EndDate;
            WorkingExperienceDb.PersonId = workingExperience.PersonId;

            _dbContext.Update(WorkingExperienceDb);

            // _dbcontext.Entry(person).State = EntityState.Modified;

            _dbContext.SaveChanges();
            return Ok("Skill modified successfully!");
        }


        [HttpDelete("deleteWorkingExperience/{id}")]
        [Authorize(Policies.ManageAllWorkingExperiencesPolicy)]
        public async Task<IActionResult> DeleteWorkingExperience(int id)
        {
            try
            {
                var workingExperience = await _dbContext.WorkingExperiences.FindAsync(id);
                if (workingExperience == null)
                {
                    return NotFound();
                }

                _dbContext.WorkingExperiences.Remove(workingExperience);
                await _dbContext.SaveChangesAsync();

                return Ok("Skill successfully deleted!");

            }
            catch (Exception ex)
            {
                throw;
            }

        }






        [HttpGet("getAllForeignLanguages")]
        //[Authorize(Policy = "get_list_of_persons")]
        public IList<ForeignLanguage> GetForeignLanguages()
        {
            var listOfForeignLanguages = _dbContext.ForeignLanguages.ToList();
            return listOfForeignLanguages;
        }


        [HttpPost("registerForeignLanguages")]
        //[Authorize(Policy = "create_person")]
        public ForeignLanguage CreateForeignLanguage(ForeignLanguagePostViewModel foreignLanguage)
        {
            var mapForeignLanguages = _mapper.Map<ForeignLanguage>(foreignLanguage);
            var a = _dbContext.ForeignLanguages.Add(mapForeignLanguages);
            _dbContext.SaveChanges();
            return a.Entity;
        }


        [HttpPut("updateForeignLanguage/{id}")]
        //[Authorize(Policy = "update_person")]
        public async Task<IActionResult> ChangeForeignLanguage(int id, ForeignLanguagePutViewModel foreignLanguage)
        {
            var ForeignLanguageDb = _dbContext.ForeignLanguages.Where(p => p.Id == id).FirstOrDefault();

            if (ForeignLanguageDb == null)
            {
                return BadRequest();
            }
            _mapper.Map(foreignLanguage, ForeignLanguageDb);
            _dbContext.Update(ForeignLanguageDb);
            //ForeignLanguageDb.Language = foreignLanguage.Language;
            //ForeignLanguageDb.Level = foreignLanguage.Level;
            //ForeignLanguageDb.PersonId = foreignLanguage.PersonId;

            //var mapForeignLanguages = _mapper.Map<ForeignLanguage>(foreignLanguage);
            


            //_dbContext.Update(mapForeignLanguages);

            

            _dbContext.SaveChanges();
            return Ok("ForeignLanguage modified successfully!");
        }


        [HttpDelete("deleteForeignLanguage/{id}")]
        //[Authorize(Policy = "delete_person")]
        public async Task<IActionResult> DeleteForeignLanguage(int id)
        {
            try
            {
                var foreignLanguage = await _dbContext.ForeignLanguages.FindAsync(id);
                if (foreignLanguage == null)
                {
                    return NotFound();
                }

                _dbContext.ForeignLanguages.Remove(foreignLanguage);
                await _dbContext.SaveChangesAsync();

                return Ok("ForeignLanguage successfully deleted!");

            }
            catch (Exception ex)
            {
                throw;
            }

        }










    }
}
