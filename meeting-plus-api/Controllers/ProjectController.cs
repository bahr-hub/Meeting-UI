using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using Model.Models;
using Services;
using Shared;

namespace WebApp.Controllers
{
    [ApiController]
    public class ProjectController : BaseController
    {
        IProjectService _ProjectService;
        public ProjectController(IProjectService ProjectService, IHttpContextAccessor httpContextAccessor)
        {
            _ProjectService = ProjectService;
        }


        [HttpGet()]
        public Result Get(int ID)
        {
            return _ProjectService.Get(ID);
        }


        [HttpPost]
        public Result GetAll([FromBody]DataSource dataSource)
        {
            return _ProjectService.GetAll(dataSource);
        }

        [HttpPost]
        public Result Create([FromBody]ProjectDto Project)
        {
            return _ProjectService.Create(Project);
        }

        [HttpPut]
        public Result Update(ProjectDto Project)
        {
            return _ProjectService.Update(Project);
        }


        [HttpPost]
        public Result ChangeProjectStatus(ProjectChangeStatus Project)
        {
            return _ProjectService.ChangeProjectStatus(Project.ID, Project.IsActive);
        }

        [HttpDelete]
        public Result DeleteProject(int id)
        {
            return _ProjectService.Delete(id);
        }

    }
}