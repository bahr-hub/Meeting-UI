using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model.DTO;
using Model.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using static Shared.SystemEnums;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Hosting;

namespace Services
{
    public interface INotificationService : IBaseService
    {
        Result Get(Guid ID);
        Result GetAll(DataSource dataSource);
        Result Create(NotificationDto Notification);
        Result Update(NotificationDto Notification);

    }
    public class NotificationService : BaseService, INotificationService
    {
        public NotificationService(IOptions<AppSettings> AppSettings, IMapper Mapper, BaseDataBaseContext Context, IHttpContextAccessor contextAccessor, IHostingEnvironment env)
            : base(AppSettings, Mapper, Context, contextAccessor, env) { }

        public Result Get(Guid ID)
        {
            var Notification = _Context.Notification
                .Where(x => x.Id == ID)
                .Include(x => x.User)
                .Select(x => new NotificationDto
                {
                    Id = x.Id,
                    DateTime = x.DateTime,
                    Message = x.Message,
                    User = new UserProfileDto
                    {
                        Id = x.User.Id,
                        //Email = x.FkUser.Email,
                        //Mobile = x.FkUser.Mobile,
                        //Name = x.FkUser.Name,
                        FirstName = x.User.FkUserProfile.FirstName,
                        Gender = x.User.FkUserProfile.Gender,
                        Address = x.User.FkUserProfile.Address,
                        Photo = x.User.FkUserProfile.Photo,
                        //UserType = x.FkUser.FkUserType.Code
                    }

                })
                .FirstOrDefault();
            if (Notification == null)
                return new Error("not_found");
            else return new Success(Notification);

        }

        public Result GetAll(DataSource dataSource)
        {
            var notifications = _Context.Notification.Where(x => x.UserId == CurrentUser.Id && x.Meeting.IsDeleted == false)
                 .Include(x => x.User)
                .Select(x => new NotificationDto
                {
                    Id = x.Id,
                    DateTime = x.DateTime,
                    Message = x.Message,
                    MessageAR = x.MessageAR,
                    MeetingID = x.MeetingID,
                    IsRead = x.IsRead,
                    User = new UserProfileDto
                    {
                        Id = x.User.Id,
                        //Email = x.FkUser.Email,
                        //Mobile = x.FkUser.Mobile,
                        //Name = x.FkUser.Name,
                        FirstName = x.User.FkUserProfile.FirstName,
                        Gender = x.User.FkUserProfile.Gender,
                        Address = x.User.FkUserProfile.Address,
                        Photo = x.User.FkUserProfile.Photo,
                        //UserType = x.FkUser.FkUserType.Code
                    }

                })
                .AsQueryable().OrderByDescending(x => x.DateTime);
            if (notifications != null)
                return new Success(dataSource.ToResult(notifications));
            else
                return new Error(ErrorMsg.ServerError.ToDescriptionString());

        }

        public Result Create(NotificationDto NotificationDto)
        {
            var Notification = _mapper.Map<NotificationDto, Notification>(NotificationDto);
            _Context.Notification.Add(Notification);
            _Context.SaveChanges();
            NotificationDto.Id = Notification.Id;
            return new Success(NotificationDto);
        }

        public Result Update(NotificationDto notificationDto)
        {

            //var notification = _mapper.Map<Notification>(new NotificationDto()
            //{
            //    Id = notificationDto.Id,
            //    IsRead = true
            //});
            Notification notification = _Context.Notification.SingleOrDefault(x => x.Id == notificationDto.Id);
            notification.IsRead = true;
            _Context.Notification.Update(notification);
            _Context.SaveChanges();
            return new Success(notification.Id);
        }

    }

}
