using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Model.DTO;
using Model.Models;
using Shared;
using System.Linq;
using static Shared.SystemEnums;

namespace Services
{
    public interface ISystemConfigurationService : IBaseService
    {
        Result Get();
        Result GetAuthenticationMode();
        Result Update(SystemConfiguration systemConfiguration);
    }
    public class SystemConfigurationService : BaseService, ISystemConfigurationService
    {
        public SystemConfigurationService(IOptions<AppSettings> AppSettings, IMapper Mapper, BaseDataBaseContext Context, IHttpContextAccessor contextAccessor, IHostingEnvironment env)
            : base(AppSettings, Mapper, Context, contextAccessor, env) { }

        public Result Get()
        {
            var SystemConfiguration = _Context.SystemConfiguration
                                                .Select(x => new SystemConfigurationDto {
                                                    Id = x.Id,
                                                    AuthenticationMode = x.AuthenticationMode,
                                                    IntegrationWithGoogleCalendar = x.IntegrationWithGoogleCalendar,
                                                    MaxMeetingTime = x.MaxMeetingTime,
                                                    EndOfWorkDays = x.EndOfWorkDays,
                                                    StartOfWorkDays = x.StartOfWorkDays,
                                                    TimeZone = x.TimeZone
                                                })
                                                .FirstOrDefault();
            if (SystemConfiguration != null)
                return new Success(SystemConfiguration);
            else
                return new Error(ErrorMsg.ServerError.ToDescriptionString());
        }

        public Result GetAuthenticationMode()
        {
            var SystemConfiguration = _Context.SystemConfiguration
                                                            .Select(x => new SystemConfigurationDto
                                                            {
                                                                Id = x.Id,
                                                                AuthenticationMode = x.AuthenticationMode
                                                            })
                                                            .FirstOrDefault();
            if (SystemConfiguration != null)
                return new Success(SystemConfiguration);
            else
                return new Error(ErrorMsg.ServerError.ToDescriptionString());
        }

        public Result Update(SystemConfiguration systemConfiguration)
        {
            _Context.Entry(systemConfiguration).State = EntityState.Modified;
            _Context.SaveChanges();
            return new Success(systemConfiguration);
        }
    }
}
