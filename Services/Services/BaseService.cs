using AutoMapper;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Utils;
using Model.DTO;
using Model.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TimeZoneConverter;
using static Shared.SystemEnums;

namespace Services
{
    public interface IBaseService
    {

    }
    public class BaseService : IBaseService
    {
        public readonly BaseDataBaseContext _Context;
        public IHttpContextAccessor _ContextAccessor;
        public SystemConfigurationDto systemConfiguration;
        public readonly IMapper _mapper;
        public User CurrentUser;
        public string currentTimeZone;
        public readonly IHostingEnvironment _env;
        public readonly AppSettings _appSettings;
        public readonly string _currentBaseUrl;




        public Dictionary<int?, List<string>> ActionsPrivilegesMapping = new Dictionary<int?, List<string>>()
        {
            {
                (int)Privileges.view,
                new List<string>()
                {
                    "GetAll",
                    "GetCurrentMeetings",
                    "GetPreviousMeetings",
                    "GetUpcomingMeetings",
                    "GetInterval",
                    "Get"
                }
            },
            {
                (int)Privileges.create,
                new List<string>()
                {
                    "Create",
                    "GetParticipants"
                }
            },
            {
                (int)Privileges.edit,
                new List<string>()
                {
                    "Update"
                }
            },
            {
                (int)Privileges.details,
                new List<string>()
                {
                    "Get"
                }
            },
            {
                (int)Privileges.delete,
                new List<string>()
                {
                    "DeleteVacation",
                    "DeletePhoto",
                    "DeleteTag",
                    "DeleteProject",
                    "DeleteMeetingTopic",
                    "DeleteMeetingTask",
                    "DeleteMeeting",
                    "DeleteLocation",
                    "DeleteUser",
                    "DeleteRole"
                }
            },
            {
                (int)Privileges.delay,
                new List<string>()
                {

                }
            },
            {
                (int)Privileges.Activiation,
                new List<string>()
                {
                    "ChangeUserStatus"
                }
            }
        };


        public BaseService(IOptions<AppSettings> appSettings, IMapper mapper, BaseDataBaseContext context, IHttpContextAccessor contextAccessor, IHostingEnvironment env)
        {
            this._env = env;
            this._appSettings = appSettings.Value;
            this._mapper = mapper;
            this._Context = context;
            this._ContextAccessor = contextAccessor;
            this._Context.contextAccessor = this._ContextAccessor;
            this._currentBaseUrl = CurrentBaseUrl();

            var ID = (this._ContextAccessor.HttpContext == null) ?
                "" : this._ContextAccessor.HttpContext.User.Identity?.Name;
            CurrentUser = string.IsNullOrEmpty(ID) ? null : _Context.User.Include(x => x.FkUserConfiguration).FirstOrDefault(x => x.Id == (Guid.Parse(ID)));

            systemConfiguration = _Context.SystemConfiguration
                                                .Select(x => new SystemConfigurationDto
                                                {
                                                    Id = x.Id,
                                                    AuthenticationMode = x.AuthenticationMode,
                                                    IntegrationWithGoogleCalendar = x.IntegrationWithGoogleCalendar,
                                                    MaxMeetingTime = x.MaxMeetingTime,
                                                    EndOfWorkDays = x.EndOfWorkDays,
                                                    StartOfWorkDays = x.StartOfWorkDays,
                                                    TimeZone = x.TimeZone
                                                })
                                                .FirstOrDefault();

            if (CurrentUser != null)
            {
                if (CurrentUser.FkUserConfiguration != null)
                {
                    if (CurrentUser.FkUserConfiguration.TimeZone != null)
                    {
                        currentTimeZone = CurrentUser.FkUserConfiguration.TimeZone;
                    }
                }
                else if (systemConfiguration != null)
                {
                    if (systemConfiguration.TimeZone != null)
                    {
                        currentTimeZone = systemConfiguration.TimeZone;
                    }
                }
            }


            if (currentTimeZone == null)
            {
                currentTimeZone = _appSettings.TimeZone;
            }
        }


        public IHttpContextAccessor GetContextAccessor()
        {
            return this._ContextAccessor;
        }

        internal Result IsDuplicatedUser(Guid ID, string UserName, string Email, string Mobile)
        {
            if (_Context.User.Any(x => x.Name.ToLower().Equals(UserName.ToLower()) && x.IsDeleted != true && x.Id != ID))
            {
                return new Error(ErrorMsg.duplicatedName.ToDescriptionString());
            }
            else if (_Context.User.Any(x => x.Email.ToLower().Equals(Email.ToLower()) && x.IsDeleted != true && x.Id != ID))
            {
                return new Error(ErrorMsg.duplicatedEmail.ToDescriptionString());
            }
            else if (_Context.User.Any(x => x.Mobile.ToLower().Equals(Mobile.ToLower()) && x.IsDeleted != true && x.Id != ID))
            {
                return new Error(ErrorMsg.duplicatedMobile.ToDescriptionString());
            }
            return null;
        }


        internal string GetPassword(string Password)
        {
            return Shared.Security.TripleDES.Encrypt(Password, true);
        }

        public DateTime ToTimeZone(DateTime value)
        {
            //if (currentTimeZone != null)
            //{
            //    TimeZoneInfo timeZoneInfo =
            //  TimeZoneInfo.FindSystemTimeZoneById(currentTimeZone);
            //    return value == null ? value
            //   : TimeZoneInfo.ConvertTimeFromUtc(value, timeZoneInfo);
            //}
            //else
            //    return value;
            return (value == null) ? value
            : TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(value, DateTimeKind.Utc), TZConvert.GetTimeZoneInfo(currentTimeZone.Split(",")[0]));

            //TimeZoneInfo curTimeZone = TimeZoneInfo.Local;
            //var yourTime = TimeZoneInfo.ConvertTime(value, TimeZoneInfo.Local, TimeZoneInfo.FindSystemTimeZoneById(curTimeZone.Id));
            //return yourTime;
        }

        public DateTime ToTimeZone(DateTime value, string TimeZone)
        {

            return (value == null) ? value
            : TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(value, DateTimeKind.Utc), TZConvert.GetTimeZoneInfo(TimeZone));
        }


        public Result SendEmail(Mail mail, string TemplatePath, Dictionary<string, string> Params)
        {
            MimeMessage message = new MimeMessage();

            MailboxAddress from = new MailboxAddress(mail.From.Name, mail.From.Email);
            message.From.Add(from);
            foreach (var item in mail.To)
            {
                MailboxAddress to = new MailboxAddress(item.Name, item.Email);
                message.To.Add(to);
            }

            message.Subject = mail.Subject;

            BodyBuilder bodyBuilder = new BodyBuilder();

            if (mail.Attachments != null)
            {
                foreach (var item in mail.Attachments)
                {
                    try
                    {
                        //var builder = new BodyBuilder();
                        var path = string.Concat(_env.WebRootPath, "/", item.Contains("wwwroot") ? item.Substring(8) : item);
                        var itemFile = bodyBuilder.LinkedResources.Add(path);


                        itemFile.ContentId = MimeUtils.GenerateMessageId();
                        if (itemFile.ContentType.MediaType == "image")
                        {
                            var ParamsItem = Params.Where(x => x.Value.Equals(item)).FirstOrDefault();
                            if (ParamsItem.Key != null) Params[ParamsItem.Key] = itemFile.ContentId;


                        }
                    }
                    catch(Exception ex)
                    {
                        //Attachment is not a valid attachment
                    }
                    //bodyBuilder.Attachments.Add(itemFile);
                }
            }
            bodyBuilder.HtmlBody = this.PopulateBody(string.Concat(this._env.WebRootPath, "/", TemplatePath), Params);
            bodyBuilder.TextBody = mail.Text;


            message.Body = bodyBuilder.ToMessageBody();

            SmtpClient client = new SmtpClient();
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.Connect(_appSettings.EmailSettings.SMTPAddress, _appSettings.EmailSettings.SMTPPort, SecureSocketOptions.None);
            client.Authenticate(_appSettings.EmailSettings.Email, _appSettings.EmailSettings.Password);
            //   client.EnableSsl = true;
            client.Send(message);
            client.Disconnect(true);
            client.Dispose();
            return new Success("Email Sent.");
        }

        public string PopulateBody(string EmailTemplate, Dictionary<string, string> Params)
        {
            string body = (Params == null || !string.IsNullOrEmpty(EmailTemplate)) ? string.Empty : Params["body"];
            if (string.IsNullOrEmpty(body))
            {
                using (StreamReader reader = new StreamReader(EmailTemplate))
                {
                    body = reader.ReadToEnd();
                }
                if (Params != null && Params.Count > 0)
                {
                    foreach (var param in Params)
                    {
                        body = body.Replace("{" + param.Key + "}", param.Value);
                    }
                }
            }
            return body;
        }

        private string CurrentBaseUrl()
        {
            if (_env.IsDevelopment())
            {
                return _appSettings.DevelopmentURL;
            }
            return _appSettings.ProductionURL;
        }


    }
}
