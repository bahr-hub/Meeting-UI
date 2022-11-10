using System;
using System.IO;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using Model.Models;
using Services;
using Shared;
using static Shared.SystemEnums;

namespace WebApp.Controllers
{
    [ApiController]
    public class UserController : BaseController
    {

        public UserController(IUserService UserService, IAbilityService AbilityService,
            IHttpContextAccessor httpContextAccessor, IHostingEnvironment hostingEnvironment)
            : base(UserService, AbilityService, httpContextAccessor, hostingEnvironment)
        {
            _UserService = UserService;
        }
        [HttpGet()]
        public UserCredential Integrate()
        {
            return _UserService.Integrate();
        }

        [HttpGet()]
        public Result Get(Guid ID)
        {
            return _UserService.Get(ID);
        }

        [HttpGet()]
        public Result GetByToken()
        {
            return new Success(CurrentUser);
        }

        [HttpPost]
        public Result GetAll([FromBody]DataSource dataSource)
        {
            return _UserService.GetAll(dataSource);
        }

        [HttpPost]
        public Result Create([FromBody]UserDto User)
        {
            return _UserService.Create(User);
        }

        [HttpPut]
        public Result Update([FromBody]UserDto User)
        {
            return _UserService.Update(User);
        }


        [HttpPost]
        public Result UploadPhoto(Guid userId)
        {
            var files = Request.Form.Files;

            foreach (var file in files)
            {
                return _UserService.UpdatePhoto(userId, this.UploadPhoto(CurrentUser.FkUserProfile.Photo, file));
            }
            return new Error(ErrorMsg.NotUnique.ToDescriptionString());
        }

        [AllowAnonymous]
        [HttpPost]
        public Result Register([FromBody] UserDto User)
        {
            return _UserService.Register(User);
        }

        [HttpPost]
        public Result ChangeUserPassword(UserChangePassword User)
        {
            return _UserService.ChangeUserPassword(User.ID, User.Password);
        }

        [HttpPost]
        public Result ChangeUserStatus(UserChangeStatus User)
        {
            return _UserService.ChangeUserStatus(User.ID, User.IsActive);
        }

        [HttpDelete]
        public Result DeleteUser(Guid id)
        {
            return _UserService.Delete(id);
        }

        [HttpGet()]
        public Result GetUserProfile(Guid ID)
        {
            return _UserService.GetUserProfile(ID);
        }
        [HttpGet()]
        public Result ToggleNotificationState()
        {
            return _UserService.ToggleNotificationState();
        }

        #region helpers

        private string UploadPhoto(string oldPhoto, IFormFile Photo)
        {
            if (Photo != null && Photo.Length > 0)
            {
                var folderName = "images/profiles/" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
                var fileName = Path.GetFileName(Photo.FileName);
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, folderName, fileName);
                System.IO.FileInfo file = new System.IO.FileInfo(filePath);
                file.Directory.Create();
                using (var fileSteam = new FileStream(filePath, FileMode.Create))
                {
                    Photo.CopyTo(fileSteam);
                }
                if (!string.IsNullOrEmpty(oldPhoto))
                    DeletePhoto(Path.Combine(_hostingEnvironment.WebRootPath, oldPhoto));
                return folderName + "/" + fileName;
            }
            return "";
        }
        private void DeletePhoto(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return;
                System.IO.FileInfo file = new System.IO.FileInfo(filePath);
                file.Directory.Delete(true);
            }
            catch { }
        }
        #endregion
    }
}