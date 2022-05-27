
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
using Microsoft.EntityFrameworkCore;

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


        //GetPersonById
        [HttpGet("getPerson/{id}")]
        [Authorize(Policy = "get_list_of_persons")]
        public async Task<ActionResult<PersonGetViewModel>> GetPerson(int id)
        {
            try
            {
                var result =  _dbContext.Persons.Where(i=>i.Id==id).Include(i => i.Skills).Include(i => i.ForeignLanguages).Include(i => i.WorkingExperiences).FirstOrDefault();

                if (result == null) return NotFound();

                var mappPeople = _mapper.Map<PersonGetViewModel>(result);

                return Ok(mappPeople);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }


        //GetAllPeople
        [HttpGet("getAllPeople")]
        [Authorize(Policy = "get_list_of_persons")]
        public IList<PersonGetViewModel> GetPeople()
        {
            var listOfPersons = _dbContext.Persons.Include(i=>i.Skills).Include(i=>i.ForeignLanguages).Include(i => i.WorkingExperiences).ToList();
            var mappPeople = _mapper.Map<IList<PersonGetViewModel>>(listOfPersons);
            return mappPeople;
        }


        //RegisterPerson
        [HttpPost("registerPerson")]
        [Authorize(Policy = "create_person")]
        public Person CreatePerson(PersonPostViewModel person)
        {
            var mappPersons = _mapper.Map<Person>(person);
            var p = _dbContext.Persons.Add(mappPersons);
            _dbContext.SaveChanges();
            return p.Entity;
        }


        //UpdatePersonById
        [HttpPut("updatePerson/{id}")]
        [Authorize(Policy = "update_person")]
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


        //DeletePersonById
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


        //GetSkillByPersonId
        [HttpGet("getSkillByPersonId/{personId}")]
        [Authorize(Policies.ViewAllSkillsPolicy)]
        public async Task<IActionResult> GetSkills(int personId)
        {
            try
            {
                var result = _dbContext.Skills.Where(i => i.PersonId == personId).ToList();

                if (result == null) return NotFound();

                var mappSkill = _mapper.Map<IList<SkillEditViewModel>>(result);

                return Ok(mappSkill);
                //return result;
            }
            catch (Exception)
            {
                return BadRequest();
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
        public Skill CreateSkill(SkillPostViewModel skill)
        {
            var mapSkills = _mapper.Map<Skill>(skill);
            var s = _dbContext.Skills.Add(mapSkills);

            _dbContext.SaveChanges();
            return s.Entity;
        }
        
        
        [HttpPut("updateSkill/{id}")]
        [Authorize(Policies.ManageAllSkillsPolicy)]
        public async Task<IActionResult> ChangeSkill(int id, SkillPutViewModel skill)
        {
            var SkillDb = _dbContext.Skills.Where(p => p.Id == id).FirstOrDefault();

            if (SkillDb == null)
            {
                return BadRequest();
            }

            _mapper.Map(skill, SkillDb);
            _dbContext.Update(SkillDb);

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


        //GetForeignLanguageByPersonId
        [HttpGet("getForeignLanguageByPersonId/{personId}")]
        [Authorize(Policies.ViewAllForeignLanguagesPolicy)]
        public async Task<IActionResult> GetForeignLanguage(int personId)
        {
            try
            {
                var result = _dbContext.ForeignLanguages.Where(i => i.PersonId == personId).ToList();

                if (result == null) return NotFound();

                var mappForeignLanguage = _mapper.Map<IList<ForeignLanguageEditViewModel>>(result);

                return Ok(mappForeignLanguage);
                
            }
            catch (Exception)
            {
                return BadRequest();
            }

        }


        //GetAllForeignLanguages
        [HttpGet("getAllForeignLanguages")]
        [Authorize(Policies.ViewAllForeignLanguagesPolicy)]
        public IList<ForeignLanguage> GetForeignLanguages()
        {
            var listOfForeignLanguages = _dbContext.ForeignLanguages.ToList();
            return listOfForeignLanguages;
        }


        //RegisterForeignLanguage
        [HttpPost("registerForeignLanguages")]
        [Authorize(Policies.ManageAllForeignLanguagesPolicy)]
        public ForeignLanguage CreateForeignLanguage(ForeignLanguagePostViewModel foreignLanguage)
        {
            var mapForeignLanguages = _mapper.Map<ForeignLanguage>(foreignLanguage);
            var a = _dbContext.ForeignLanguages.Add(mapForeignLanguages);
            _dbContext.SaveChanges();
            return a.Entity;
        }


        //UpdateForeignLanguageById
        [HttpPut("updateForeignLanguage/{id}")]
        [Authorize(Policies.ManageAllForeignLanguagesPolicy)]
        public async Task<IActionResult> ChangeForeignLanguage(int id, ForeignLanguagePutViewModel foreignLanguage)
        {
            var ForeignLanguageDb = _dbContext.ForeignLanguages.Where(p => p.Id == id).FirstOrDefault();

            if (ForeignLanguageDb == null)
            {
                return BadRequest();
            }
            _mapper.Map(foreignLanguage, ForeignLanguageDb);
            _dbContext.Update(ForeignLanguageDb);

            _dbContext.SaveChanges();
            return Ok("ForeignLanguage modified successfully!");
        }


        //DeleteForeignLanguageById
        [HttpDelete("deleteForeignLanguage/{id}")]
        [Authorize(Policies.ManageAllForeignLanguagesPolicy)]
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
