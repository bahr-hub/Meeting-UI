using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Model.DTO;
using Model.Models;
using Shared;
using System;
using System.Linq;
using static Shared.SystemEnums;

namespace Services
{
    public interface IProjectService : IBaseService
    {
        Result Get(int ID);
        Result GetAll(DataSource dataSource);
        Result Create(ProjectDto Project);
        Result Update(ProjectDto Project);
        Result ChangeProjectStatus(int ID, bool Status);
        Result Delete(int ID);
    }
    public class ProjectService : BaseService, IProjectService
    {
        public ProjectService(IOptions<AppSettings> AppSettings, IMapper Mapper, BaseDataBaseContext Context, IHttpContextAccessor contextAccessor, IHostingEnvironment env)
            : base(AppSettings, Mapper, Context, contextAccessor, env) { }
        public Result Get(int ID)
        {
            var project= _Context.Project
                .Where(x => x.Id == ID &&  x.IsDeleted == false)
                .Select(x => new ProjectDto
                {
                    Id = x.Id,
                    Name=x.Name,
                    //NameAr=x.NameAr,
                    Description=x.Description,
                    IsActive=x.IsActive
                })
                .FirstOrDefault();
            if (project != null)
                return new Success(project);
            else return new Error(ErrorMsg.NotFound.ToDescriptionString());
        }
        public Result GetAll(DataSource dataSource)
        {
            var projects= _Context.Project
                .Where(x=> x.IsDeleted == false)
                 .Select(x => new ProjectDto
                 {
                     Id = x.Id,
                     Name = x.Name,
                     //NameAr = x.NameAr,
                     Description = x.Description,
                     IsActive = x.IsActive
                 })
                .AsQueryable();
            if (projects != null)
                return new Success(dataSource.ToResult(projects));
            else
                return new Error(ErrorMsg.ServerError.ToDescriptionString());
        }
        public Result Create(ProjectDto ProjectDto)
        {
            if (IsUniqueProject(ProjectDto.Id,ProjectDto.Name))
            {
                var Project = _mapper.Map<ProjectDto, Project>(ProjectDto);
                _Context.Project.Add(Project);
                _Context.SaveChanges();
                ProjectDto.Id = Project.Id;
                return new Success(ProjectDto);
            }

            else if(ProjectDto.Name.Length>50)
            {
                return new Error(ErrorMsg.projectNameTooLong.ToDescriptionString());
            }
            else
                return new Error(ErrorMsg.NotUnique.ToDescriptionString());
        }
        public Result Update(ProjectDto ProjectDto)
        {
            if (IsUniqueProject(ProjectDto.Id, ProjectDto.Name))
            {
                var projectInDb = _Context.Project.SingleOrDefault(c => c.Id == ProjectDto.Id);

                if (projectInDb == null)
                    return null;

                _mapper.Map(ProjectDto, projectInDb);
                _Context.Entry(projectInDb).State = EntityState.Modified;
                _Context.SaveChanges();
                return new Success(ProjectDto);
            }
            else
                return new Error(ErrorMsg.NotUnique.ToDescriptionString());
        }
        public Result ChangeProjectStatus(int ID, bool Status)
        {
            var Project = _Context.Project
                .Where(x => x.Id == ID)
                .FirstOrDefault();
            if (Project == null)
                return new Error(ErrorMsg.NotFound.ToDescriptionString());
            Project.IsActive = Status;
            _Context.SaveChanges();
            var ProjectDto = _mapper.Map<Project, ProjectDto>(Project);
            return new Success(ProjectDto);
        }
        public Result Delete(int ID)
        {
            var Project = _Context.Project
               .Where(x => x.Id == ID && x.IsDeleted !=true)
               .FirstOrDefault();
            if (Project == null)
                return new Error(ErrorMsg.NotFound.ToDescriptionString());
            Project.IsDeleted = true;
            _Context.SaveChanges();
            var ProjectDto = _mapper.Map<Project, ProjectDto>(Project);
            return new Success(ProjectDto);
        }

        #region helpers
        private bool IsUniqueProject(int ID, string Name)
        {
            return !_Context.Project.Any(x => x.Id != ID && (x.Name.ToLower().Equals(Name.ToLower())  && ( x.IsDeleted == false)));
        }
        #endregion
    }
}
