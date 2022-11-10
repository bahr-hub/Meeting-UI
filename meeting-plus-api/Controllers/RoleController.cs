using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using Model.Models;
using Services;
using Shared;

namespace WebApp.Controllers
{
    [ApiController]
    public class RoleController : BaseController
    {
        IRoleService _RoleService;
        public RoleController(IRoleService RoleService, IHttpContextAccessor httpContextAccessor)
        {
            _RoleService = RoleService;
        }


        [HttpGet()]
        public Result Get(int ID)
        {
            return _RoleService.Get(ID);
        }


        [HttpPost]
        public Result GetAll([FromBody]DataSource dataSource)
        {
            return _RoleService.GetAll(dataSource);
        }
        [HttpPost]
        public Result GetModules([FromBody]DataSource dataSource)
        {
            return _RoleService.GetModules(dataSource);
        }
        [HttpPost]
        public Result GetPrivileges([FromBody]DataSource dataSource)
        {
            return _RoleService.GetPrivileges(dataSource);
        }

        [HttpPost]
        public Result Create([FromBody]RoleDto Role)
        {
            return _RoleService.Create(Role);
        }

        [HttpPut]
        public Result Update(RoleDto Role)
        {
            return _RoleService.Update(Role);
        }


        [HttpPost]
        public Result ChangeRoleStatus(RoleChangeStatus Role)
        {
            return _RoleService.ChangeRoleStatus(Role.ID, Role.IsActive);
        }
        [HttpDelete]
        public Result DeleteRole(int id)
        {
            return _RoleService.Delete(id);
        }

        [HttpGet]
        public Result GetAllLite()
        {
            return _RoleService.GetAllLite();
        }

    }
}