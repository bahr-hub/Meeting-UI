using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model.DTO;
using Model.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using static Shared.SystemEnums;

namespace Services
{
    public interface IRoleService : IBaseService
    {
        Result Get(int ID);
        Result GetAll(DataSource dataSource);
        Result Create(RoleDto Role);
        Result Update(RoleDto Role);
        Result ChangeRoleStatus(int ID, bool Status);
        Result GetModules(DataSource dataSource);
        Result GetPrivileges(DataSource dataSource);
        Result Delete(int id);
        Result GetAllLite();
    }
    public class RoleService : BaseService, IRoleService
    {
        public RoleService(IOptions<AppSettings> AppSettings, IMapper Mapper, BaseDataBaseContext Context, IHttpContextAccessor contextAccessor, IHostingEnvironment env)
            : base(AppSettings, Mapper, Context, contextAccessor, env) { }
        public Result Get(int ID)
        {
            var role = _Context.Role
                .Where(y => y.Id == ID && y.IsDeleted == false)
                .Include(y => y.RoleModulePrivilege)
                .Select(y => new RoleDto
                {
                    Id = y.Id,
                    Name = y.Name,
                    Description = y.Description,
                    IsActive = y.IsActive,
                    RoleModulePrivilege = _mapper.Map<ICollection<RoleModulePrivilegeDto>>(y.RoleModulePrivilege)
                }).FirstOrDefault();
            if (role != null)
                return new Success(role);
            else return new Error(ErrorMsg.NotFound.ToDescriptionString());
        }
        public Result GetAll(DataSource dataSource)
        {
            var roles = _Context.Role
                .Where(x => x.IsDeleted == false)
                .Select
                (x => new RoleDto
                {

                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    RoleModulePrivilege = _mapper.Map<ICollection<RoleModulePrivilegeDto>>(x.RoleModulePrivilege)

                })
                .AsQueryable();
            if (roles != null)
                return new Success(dataSource.ToResult(roles));
            else
                return new Error(ErrorMsg.ServerError.ToDescriptionString());
        }
        public Result Create(RoleDto RoleDto)
        {
            if (IsUniqueRole(RoleDto.Id, RoleDto.Name))
            {
                var Role = _mapper.Map<Role>(RoleDto);
                _Context.Role.Add(Role);
                _Context.SaveChanges();
                RoleDto.Id = Role.Id;
                return new Success(RoleDto);
            }
            else
                return new Error(ErrorMsg.NotUnique.ToDescriptionString());
        }
        public Result Update(RoleDto RoleDto)
        {

            if (IsUniqueRole(RoleDto.Id, RoleDto.Name))
            {
                //added the include to get the role privileges because if it is not present it will
                //attempt to create the already exsisting roleprivileges 
                var roleInDb = _Context.Role.Include(s => s.RoleModulePrivilege).SingleOrDefault(c => c.Id == RoleDto.Id);

                if (roleInDb == null)
                    return new Error(ErrorMsg.NotFound.ToDescriptionString());

                //if (RoleDto.RoleModulePrivilege.Count > 0)
                //{
                //    _Context.RoleModulePrivilege.RemoveRange(_Context.RoleModulePrivilege.Where(x => x.FkRoleId == RoleDto.Id));
                //    //foreach (var rolePrivilege in RoleDto.RoleModulePrivilege)
                //    //{
                //    //    //_Context.Entry(rolePrivilege).State = EntityState.Added;
                //    //    _Context.RoleModulePrivilege.Add(_mapper.Map<RoleModulePrivilege>(rolePrivilege));
                //    //}
                //}
                _mapper.Map(RoleDto, roleInDb);
                _Context.Entry(roleInDb).State = EntityState.Modified;
                _Context.SaveChanges();
                return new Success(RoleDto);
            }
            else
                return new Error(ErrorMsg.NotUnique.ToDescriptionString());
        }
        public Result ChangeRoleStatus(int ID, bool Status)
        {
            var Role = _Context.Role
                .Where(x => x.Id == ID)
                .FirstOrDefault();
            if (Role == null)
                return new Error(ErrorMsg.NotFound.ToDescriptionString());
            Role.IsActive = Status;
            _Context.SaveChanges();
            var RoleDto = _mapper.Map<Role, RoleDto>(Role);
            return new Success(RoleDto);
        }

        public Result GetModules(DataSource dataSource)
        {
            var modules = _Context.Module
                .Include(module => module.ModulePrivilege)
                .ThenInclude(modulePrivilege => modulePrivilege.FkPrivilege)
                .Where(x => x.IsDeleted == false)
                .Select
                (x => new Module
                {

                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    ModulePrivilege = x.ModulePrivilege
                    .Select(l => new ModulePrivilege
                    {
                        FkPrivilege = l.FkPrivilege,
                        FkModuleId = l.FkModuleId,
                        FkPrivilegeId = l.FkPrivilegeId

                    }).ToList()

                })
                .AsQueryable();
            if (modules != null)
                return new Success(dataSource.ToResult(modules));
            else
                return new Error(ErrorMsg.ServerError.ToDescriptionString());
        }


        public Result GetPrivileges(DataSource dataSource)
        {
            var privileges = _Context.Privilege
                .Where(x => x.IsDeleted == false)
                .Select
                (x => new Privilege
                {

                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsActive = x.IsActive,
                })
                .AsQueryable();
            if (privileges != null)
                return new Success(dataSource.ToResult(privileges));
            else
                return new Error(ErrorMsg.ServerError.ToDescriptionString());
        }

        public Result GetAllLite()
        {
            var roles = _Context.Role
                .Where(x => x.IsDeleted == false)
                .Select
                (x => new RoleDto
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .AsQueryable();
            if (roles != null)
                return new Success(roles);
            else
                return new Error(ErrorMsg.ServerError.ToDescriptionString());
        }

        #region helpers

        private bool IsUniqueRole(int ID, string Name)
        {
            return !_Context.Role.Any(x => x.Id != ID && x.Name.ToLower().Equals(Name.ToLower()) && (x.IsDeleted == false));
        }

        public Result Delete(int id)
        {
            //get the role
            var role = _Context.Role.Where(r => r.Id == id).SingleOrDefault();
            if (role == null)
                return new Error(ErrorMsg.NotFound.ToDescriptionString());
            else
            {
                role.IsDeleted = true;
                _Context.SaveChanges();
                var RoleDto = _mapper.Map<Role, RoleDto>(role);
                return new Success(RoleDto);
            }
        }


        #endregion
    }
}
