using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model.DTO;
using Model.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using static Shared.SystemEnums;

namespace Services
{
    public interface IAccountService : IBaseService
    {
        Result Authenticate(string UserName, string Password, DateTime userDate, bool External = false);
        SecurityToken GenerateToken(UserDto user);
    }
    public class AccountService : BaseService, IAccountService
    {
        public AccountService(IOptions<AppSettings> AppSettings, IMapper Mapper, BaseDataBaseContext Context, IHttpContextAccessor contextAccessor, IHostingEnvironment env)
            : base(AppSettings, Mapper, Context, contextAccessor, env) { }

        public Result Authenticate(string UserName, string Password, DateTime userDate, bool External = false)
        {
            if (!IsValidateUserDate(userDate))
                return new Error(ErrorMsg.InvalidDate.ToDescriptionString());

            UserDto User = null;
            if (systemConfiguration.AuthenticationMode == (int)AuthenticateMode.ActiveDirctory && !External)
            {
                User = AuthenticateAD(UserName, Password);
            }
            else if (systemConfiguration.AuthenticationMode == (int)AuthenticateMode.Database || External)
            {
                User = AuthenticateDB(UserName, Password);
            }
            //else
            //{
            //    User = AuthenticateAD(UserName, Password);
            //    if(User != null)
            //        User = AuthenticateDB(UserName, Password);
            //}

            // return null if user not found
            if (User == null)
                return new Error(ErrorMsg.UnAuthorized.ToDescriptionString());
            // authentication successful so generate jwt token
            var TokenHandler = new JwtSecurityTokenHandler();
            var Token = GenerateToken(User);
            User.Token = TokenHandler.WriteToken(Token);
            User.Ability = GetCurrentUserAbility(User.Id);

            // remove password before returning
            User.Password = null;

            return new Success(User);
        }

        private bool IsValidateUserDate(DateTime userDate)
        {
            if (ToTimeZone(userDate).Date == (DateTime.Now).Date)
            {
                return true;//He is valid user
            }
            return false;
            //return true;
        }


        #region helpers
        public List<RoleModulePrivilege> GetCurrentUserAbility(Guid CurrentUserId)
        {
            var UserRoleList = _Context.UserRole.Where(x => x.FkUserId == CurrentUserId);

            List<RoleModulePrivilege> ModulePrivilege = new List<RoleModulePrivilege>();
            if (UserRoleList.Count() != 0)
            {
                foreach (var UserRole in UserRoleList)
                {
                    ModulePrivilege.AddRange(_Context.RoleModulePrivilege
                        .Where(x => x.FkRoleId == UserRole.FkRoleId).Select(x => new RoleModulePrivilege()
                        {
                            FkModule = x.FkModule,
                            FkPrivilege = x.FkPrivilege

                        }).ToList());
                }

            }

            return ModulePrivilege;
        }

        private UserDto AuthenticateDB(string Email, string Password)
        {
            return _Context.User
                .Where(x => x.IsActive && !x.IsDeleted && x.Email.ToLower() == Email.ToLower() && x.Password == GetPassword(Password))
                .Select(x => new UserDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email,
                    Mobile = x.Mobile,
                    IsSuperAdmin = x.IsSuperAdmin,
                    IsAdmin = x.IsAdmin,
                    IsActive = x.IsActive,
                    FkUserProfile = _mapper.Map<UserProfileDto>(x.FkUserProfile),
                    UserRole = _mapper.Map<ICollection<UserRoleDto>>(x.UserRole),
                    FkUserConfiguration = _mapper.Map<UserConfigurationDto>(x.FkUserConfiguration)
                })
                .Include(x => x.FkUserProfile)
                .Include(x => x.UserRole)
                .Include(x => x.FkUserConfiguration)
                .FirstOrDefault();
        }

        private UserDto AuthenticateAD(string UserName, string Password)
        {
            using (var context = new PrincipalContext(ContextType.Domain, _appSettings.LDAP_DOMAIN, UserName, Password))
            {
                if (context.ValidateCredentials(UserName, Password))
                {

                    UserPrincipal user = new UserPrincipal(context);
                    PrincipalSearcher searcher = new PrincipalSearcher(user);
                    UserPrincipal foundUser = searcher.FindAll().Where(x => x.SamAccountName.Equals(UserName)).FirstOrDefault() as UserPrincipal;
                    if (foundUser != null)
                    {
                        var User = _Context.User.Where(x => x.IsActive && !x.IsDeleted && x.Id == foundUser.Guid)
                            .Select(x => new UserDto
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Email = x.Email,
                                Mobile = x.Mobile,
                                IsSuperAdmin = x.IsSuperAdmin,
                                IsAdmin = x.IsAdmin,
                                IsActive = x.IsActive,
                                FkUserProfile = _mapper.Map<UserProfileDto>(x.FkUserProfile),
                                UserRole = _mapper.Map<ICollection<UserRoleDto>>(x.UserRole),
                                FkUserConfiguration = _mapper.Map<UserConfigurationDto>(x.FkUserConfiguration)
                            })
                            .Include(x => x.FkUserProfile)
                            .Include(x => x.UserRole)
                            .Include(x => x.FkUserConfiguration)
                            .FirstOrDefault();
                        if (User == null)
                        {
                            return GetUserAD(foundUser, Password);
                        }
                        else
                            return User;
                    }
                }
            }

            return null;
        }
        private UserDto GetUserAD(UserPrincipal foundUser, string _password)
        {
            if (foundUser != null)
            {
                var user = new User
                {
                    Password = GetPassword(_password),
                    Id = foundUser.Guid.Value,
                    Name = foundUser.SamAccountName,
                    Email = foundUser.EmailAddress,
                    Mobile = foundUser.VoiceTelephoneNumber,
                    IsActive = true,
                    IsDeleted = false,
                    FkUserProfile = new UserProfile
                    {
                        FirstName = foundUser.Name,
                        LastName = foundUser.MiddleName
                    }
                };
                _Context.User.Add(user);
                _Context.SaveChanges();
                return _mapper.Map<UserDto>(user);
            }
            else
            {
                return null;
            }
        }

        public SecurityToken GenerateToken(UserDto User)
        {
            var TokenHandler = new JwtSecurityTokenHandler();
            var Key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var TokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    // 'user' is the model for the authenticated user
                    // also note that you can include many claims here
                    // but keep in mind that if the token causes the
                    // request headers to be too large, some servers
                    // such as IIS may reject the request.
                    new Claim(ClaimTypes.Name, User.Id.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, User.Id.ToString()),
                    new Claim("IsSuperAdmin", User.IsSuperAdmin.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Key), SecurityAlgorithms.HmacSha256Signature)
            };
            return TokenHandler.CreateToken(TokenDescriptor);
        }
        #endregion
    }
}
