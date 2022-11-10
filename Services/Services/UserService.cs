using AutoMapper;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Model.DTO;
using Model.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using TimeZoneConverter;
using static Shared.SystemEnums;

namespace Services
{
    public interface IUserService : IBaseService
    {
        Result Get(Guid ID);
        Result GetAll(DataSource dataSource);
        Result Create(UserDto User);
        Result Update(UserDto User);
        Result Register(UserDto User);
        Result ChangeUserStatus(Guid ID, bool Status);
        Result ChangeUserPassword(Guid ID, string Password);
        User GetCurrentUser(Guid ID);
        Result GetUserProfile(Guid ID);
        Result UpdatePhoto(Guid userId, string Photo);
        Result Delete(Guid ID);
        Result ToggleNotificationState();
        UserCredential Integrate();

    }
    public class UserService : BaseService, IUserService
    {
        IGoogleCalender _googleCalender;
        public UserService(IGoogleCalender googleCalender, IOptions<AppSettings> AppSettings, IMapper Mapper, BaseDataBaseContext Context, IHttpContextAccessor contextAccessor, IHostingEnvironment env)
            : base(AppSettings, Mapper, Context, contextAccessor, env)
        {
            _googleCalender = googleCalender;
        }
        public UserCredential Integrate()
        {
            return _googleCalender.Integrate2();
        }
        public Result Get(Guid ID)
        {
            var user = _Context.User
                .Where(x => x.Id == ID && x.IsDeleted == false)
                .Select(x => new UserDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email,
                    Password = x.Password,
                    Mobile = x.Mobile,
                    IsActive = x.IsActive,
                    IsAdmin = x.IsAdmin,
                    IsSuperAdmin = x.IsSuperAdmin,
                    ImageUrl = x.ImageUrl,
                    FkUserProfileId = x.FkUserProfileId,
                    LocationID =x.LocationID,
                    FkUserProfile = _mapper.Map<UserProfileDto>(x.FkUserProfile),
                    UserRole = _mapper.Map<ICollection<UserRoleDto>>(x.UserRole),
                    FkUserConfiguration = _mapper.Map<UserConfigurationDto>(x.FkUserConfiguration)
                })
                .Include(x => x.FkUserProfile)
                .Include(x => x.UserRole)
                .Include(x => x.FkUserConfiguration)
                .FirstOrDefault();
            if (user != null)
                return new Success(user);
            else return new Error(ErrorMsg.NotFound.ToDescriptionString());
        }
        public Result GetAll(DataSource dataSource)
        {
            var users = _Context.User
                .Where(x => x.IsDeleted == false)
                .Include(x => x.FkUserProfile)
                .Select(x => new UserDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email,
                    Mobile = x.Mobile,
                    LocationID=x.LocationID,
                    FkUserProfileId = x.FkUserProfileId,
                    FkUserProfile = _mapper.Map<UserProfileDto>(x.FkUserProfile),
                    UserRole = _mapper.Map<ICollection<UserRoleDto>>(x.UserRole),
                    FkUserConfiguration = _mapper.Map<UserConfigurationDto>(x.FkUserConfiguration),
                    IsActive = x.IsActive
                }).AsQueryable();
            if (users != null)
                return new Success(dataSource.ToResult(users));
            else
                return new Error(ErrorMsg.ServerError.ToDescriptionString());
        }
        public Result Create(UserDto User)
        {
            var result = IsDuplicatedUser(User.Id, User.Name, User.Email, User.Mobile);
            if (result == null)
            {
                if (User.ImageUrl == null && User.ImageBase64 != null)
                {
                    User.ImageUrl = User.Name + "_" + new Random().Next();
                }

                if (User.Password == "")
                    return new Error(ErrorMsg.PasswordIsRequired.ToDescriptionString());
                User.Password = GetPassword(User.Password);
                if (User.FkUserConfiguration != null && User.FkUserConfiguration.IntegrationWithGoogleCalendar == null)
                    User.FkUserConfiguration.IntegrationWithGoogleCalendar =
                        _Context.SystemConfiguration.FirstOrDefault().IntegrationWithGoogleCalendar;
                var user = _mapper.Map<User>(User);
                if (user.FkUserProfile != null)
                    _Context.Entry(user.FkUserProfile).State = EntityState.Added;
                if (user.FkUserConfiguration != null)
                    _Context.Entry(user.FkUserConfiguration).State = EntityState.Added;
                user.IsActive = true;
                string _currentTimeZone;
                if (user.FkUserProfile.CountryCode.ToLower() == "egy") _currentTimeZone = TimeZoneInfo.GetSystemTimeZones().Where(u => u.DisplayName.Contains(user.FkUserConfiguration.TimeZone) && u.Id.ToLower().Contains(user.FkUserProfile.CountryCode.ToLower())).FirstOrDefault().Id + "," + user.FkUserConfiguration.TimeZone;
                else _currentTimeZone = TimeZoneInfo.GetSystemTimeZones().Where(u => u.DisplayName.Contains(user.FkUserConfiguration.TimeZone)).FirstOrDefault().Id + "," + user.FkUserConfiguration.TimeZone;
                user.FkUserConfiguration.TimeZone = _currentTimeZone;

                _Context.User.Add(user);
                _Context.SaveChanges();
                SaveImage(User.ImageUrl, User.ImageBase64);
                // User.Password = null;
                ElmahCore.ElmahExtensions.RiseError(_ContextAccessor.HttpContext, new Exception("moooaoad"));
                if ((bool)user.FkUserConfiguration.IntegrationWithGoogleCalendar)
                {
                    ElmahCore.ElmahExtensions.RiseError(_ContextAccessor.HttpContext, new Exception("moooaoad2"));
                    _googleCalender.Integrate(user.Id);
                }
                User.Id = user.Id;
                User.FkUserProfileId = user.FkUserProfile.Id;

                return new Success(User);
            }
            else
                return result;
        }
        public Result Update(UserDto User)
        {
            var result = IsDuplicatedUser(User.Id, User.Name, User.Email, User.Mobile);
            if (result == null)
            {
                if (User.ImageUrl == null && User.ImageBase64 != null)
                {
                    User.ImageUrl = User.Name + "_" + new Random().Next();
                }

                if (User.Password == "")
                    return new Error(ErrorMsg.PasswordIsRequired.ToDescriptionString());
                var user = _Context.User
                                .Include(x => x.UserRole)
                                .Where(x => x.Id == User.Id)
                                .FirstOrDefault();

                // delete old roles if it changed
                if (User.UserRole?.Count > 0)
                {
                    _Context.RemoveRange(user.UserRole);
                }

                if (User.Password == null)
                    User.Password = user.Password;
                else
                    User.Password = GetPassword(User.Password);

                _mapper.Map<UserDto, User>(User, user);

                if (User.FkUserProfile != null)
                {
                    _Context.Entry(user.FkUserProfile).State = EntityState.Modified;
                }
                if (User.FkUserConfiguration != null)
                {
                    if (user.FkUserConfiguration.Id == default(Guid))
                        _Context.Entry(user.FkUserConfiguration).State = EntityState.Added;
                    else
                    {
                        string _currentTimeZone;
                        if (user.FkUserProfile.CountryCode.ToLower() == "egy") _currentTimeZone = TimeZoneInfo.GetSystemTimeZones().Where(u => u.DisplayName.Contains(user.FkUserConfiguration.TimeZone) && u.Id.ToLower().Contains(user.FkUserProfile.CountryCode.ToLower())).FirstOrDefault().Id + "," + user.FkUserConfiguration.TimeZone;
                        else _currentTimeZone = TimeZoneInfo.GetSystemTimeZones().Where(u => u.DisplayName.Contains(user.FkUserConfiguration.TimeZone)).FirstOrDefault().Id + "," + user.FkUserConfiguration.TimeZone;
                        user.FkUserConfiguration.TimeZone = _currentTimeZone;

                        //string _currentTimeZone = TimeZoneInfo.GetSystemTimeZones().Where(u => u.DisplayName.Contains(user.FkUserConfiguration.TimeZone)).FirstOrDefault().Id + "," + user.FkUserConfiguration.TimeZone;
                        //user.FkUserConfiguration.TimeZone = _currentTimeZone;

                        _Context.Entry(user.FkUserConfiguration).State = EntityState.Modified;
                    }

                }
                //if (user.UserRole?.Count > 0)
                //    _Context.Entry(user.UserRole).State = EntityState.Added;

                _Context.Entry(user).State = EntityState.Modified;
                _Context.SaveChanges();
                SaveImage(User.ImageUrl, User.ImageBase64);
                if (user.FkUserConfiguration.IntegrationWithGoogleCalendar != null && (bool)user.FkUserConfiguration.IntegrationWithGoogleCalendar)
                {
                    _googleCalender.Integrate(user.Id);
                }

                // User.Password = null;
                return new Success(User);
            }
            else
                return result;
        }
        public Result Register(UserDto User)
        {
            var result = IsDuplicatedUser(User.Id, User.Name, User.Email, User.Mobile);

            if (result == null)
            {
                User.Password = GetPassword(User.Password);
                if (User.FkUserConfiguration != null && User.FkUserConfiguration.IntegrationWithGoogleCalendar == null)
                    User.FkUserConfiguration.IntegrationWithGoogleCalendar =
                        _Context.SystemConfiguration.FirstOrDefault().IntegrationWithGoogleCalendar;
                User.IsActive = true;
                var user = _mapper.Map<User>(User);
                if (user.FkUserProfile != null)
                    _Context.Entry(user.FkUserProfile).State = EntityState.Added;
                if (user.FkUserConfiguration != null)
                    _Context.Entry(user.FkUserConfiguration).State = EntityState.Added;

                _Context.User.Add(user);
                _Context.SaveChanges();
                User.Password = null;
                User.Id = user.Id;
                User.FkUserProfileId = user.FkUserProfile.Id;
                return new Success(User);
            }
            else
                return result;
        }
        public Result ChangeUserStatus(Guid ID, bool Status)
        {
            var user = _Context.User
                .Where(x => x.Id == ID)
                .FirstOrDefault();
            if (user == null)
                return new Error(ErrorMsg.NotFound.ToDescriptionString());
            user.IsActive = Status;
            _Context.SaveChanges();
            user.Password = null;
            var UserDto = _mapper.Map<UserDto>(user);
            return new Success(UserDto);

        }
        public Result ChangeUserPassword(Guid ID, string Password)
        {
            var user = _Context.User
                .Where(x => x.Id == ID)
                .FirstOrDefault();
            if (user == null)
                return new Error(ErrorMsg.NotFound.ToDescriptionString());
            user.Password = GetPassword(Password);
            _Context.SaveChanges();
            user.Password = null;
            var UserDto = _mapper.Map<UserDto>(user);
            return new Success(UserDto);
        }
        public User GetCurrentUser(Guid ID)
        {
            return GetUser(ID);
        }
        public Result GetUserProfile(Guid ID)
        {
            var user = _Context.User
               .Where(x => x.Id == ID && x.IsDeleted == false)
               .Include(x => x.FkUserProfile)
               .Select(x => new UserProfileDto
               {
                   Id = x.Id,
                   //Email = x.Email,
                   //Mobile = x.Mobile,
                   //Name =x.Name,
                   FirstName = x.FkUserProfile.FirstName,
                   Gender = x.FkUserProfile.Gender,
                   Address = x.FkUserProfile.Address,
                   Photo = x.FkUserProfile.Photo,
                   CountryCode = x.FkUserProfile.CountryCode
                   //UserType =x.FkUserType.Code
               })
               .FirstOrDefault();
            if (user != null)
                return new Success(user);
            else return new Error(ErrorMsg.NotFound.ToDescriptionString());
        }
        public Result Delete(Guid ID)
        {
            var User = _Context.User
               .Where(x => x.Id == ID)
               .FirstOrDefault();
            var userMeeting = _Context.MeetingParticipant.Where(x => x.ParticipantId == ID).ToList();
            if (userMeeting == null || userMeeting.Count == 0)
            {
                User.IsDeleted = true;
                _Context.SaveChanges();

                var UserProfile = _Context.UserProfile
                  .Where(x => x.Id == User.FkUserProfileId)
                  .FirstOrDefault();

                UserProfile.IsDeleted = true;
                _Context.SaveChanges();
                User = new User();
                return new Success(User);
            }
            else
            {
                return new Error(SystemEnums.ErrorMsg.CannotDeleteUser.ToDescriptionString().ToString());
            }



        }
        public Result UpdatePhoto(Guid userId, string Photo)
        {
            User user = GetUser(userId);
            user.FkUserProfile.Photo = Photo;
            _Context.SaveChanges();
            return new Success(user.FkUserProfile.Photo);
        }

        public Result RefreshUsers()
        {
            if (systemConfiguration.AuthenticationMode == (int)SystemEnums.AuthenticateMode.ActiveDirctory)
            {
                var users = _Context.User;
                foreach (var user in users)
                {
                    UpdateUserAD(user.Id);
                }
                return new Success("done");
            }
            else
                return new Error(SystemEnums.ErrorMsg.ServerError.ToDescriptionString());
        }

        public Result ToggleNotificationState()
        {
            var currentUserConfig = _Context.UserConfiguration.Where(x => x.Id == CurrentUser.FkUserConfigurationId).SingleOrDefault();
            if (currentUserConfig == null)
            {
                return new Error(SystemEnums.ErrorMsg.NotFound.ToDescriptionString());
            }
            else
            {
                currentUserConfig.NotificationMuted = !currentUserConfig.NotificationMuted;
                _Context.SaveChanges();
                return new Success("done");
            }
        }

        #region helpers

        private User GetUser(Guid ID)
        {
            return _Context.User
                .Where(x => x.Id == ID && x.IsDeleted == false)
                .Include(x => x.FkUserProfile)
                .Include(x => x.UserRole)
                .Include(x => x.FkUserConfiguration)
                .FirstOrDefault();
        }
        private User UpdateUserAD(Guid ID)
        {
            using (var context = new PrincipalContext(ContextType.Domain, _appSettings.LDAP_DOMAIN))
            {
                using (var foundUser = UserPrincipal.FindByIdentity(context, IdentityType.Guid, ID.ToString()))
                {
                    if (foundUser != null)
                    {
                        var userDto = new UserDto
                        {
                            Id = ID,
                            Name = foundUser.SamAccountName,
                            Email = foundUser.EmailAddress,
                            Mobile = foundUser.VoiceTelephoneNumber,
                            FkUserProfile = new UserProfileDto
                            {
                                FirstName = foundUser.Name,
                                LastName = foundUser.MiddleName
                            }
                        };
                        var User = _Context.User.FirstOrDefault(x => x.Id == foundUser.Guid);
                        if (User != null)
                        {
                            _mapper.Map<UserDto, User>(userDto, User);
                        }

                        _Context.SaveChanges();
                        return User;
                    }
                    else
                    {
                        var User = _Context.User.FirstOrDefault(x => x.Id == foundUser.Guid);
                        if (User != null)
                        {
                            User.IsDeleted = true;
                        }

                        _Context.SaveChanges();
                        return User;
                    }
                }
            }
        }

        private void SaveImage(string imageUrl, string imageBase64)
        {
            if (imageBase64 != null)
            {
                //string webRootPath = _env.WebRootPath;//getting with wwwroot
                string contentRootPath = _env.ContentRootPath;
                string localPath = contentRootPath + "\\Images\\Users\\" + imageUrl + ".png";
                File.WriteAllBytes(localPath, Convert.FromBase64String(imageBase64));
            }
        }

        #endregion
    }
}
