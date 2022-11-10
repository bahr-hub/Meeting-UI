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
    public interface IVacationService : IBaseService
    {
        Result Get(Guid ID);
        Result GetAll(DataSource dataSource);
        Result Create(VacationDto Location);
        Result Update(VacationDto Location);
        Result Delete(Guid ID);
    }
    public class VacationService : BaseService, IVacationService
    {
        public VacationService(IOptions<AppSettings> AppSettings, IMapper Mapper, BaseDataBaseContext Context, IHttpContextAccessor contextAccessor, IHostingEnvironment env)
            : base(AppSettings, Mapper, Context, contextAccessor, env) { }
        public Result Get(Guid ID)
        {
            var vacation = _Context.Vacation
                .Where(x => x.Id == ID)
                .Select(x => new VacationDto
                {
                    Id = x.Id,
                    StartDate = ToTimeZone(x.StartDate),
                    EndDate = ToTimeZone(x.EndDate),
                    FkUserId = x.FkUserId

                })
                .FirstOrDefault();
            if (vacation != null)
                return new Success(vacation);
            else return new Error(ErrorMsg.NotFound.ToDescriptionString());
        }
        public Result GetAll(DataSource dataSource)
        {
            var vacations = _Context.Vacation
                 .Select(x => new VacationDto
                 {
                     Id = x.Id,
                     StartDate = ToTimeZone(x.StartDate),
                     EndDate = ToTimeZone(x.EndDate),
                     FkUserId = x.FkUserId

                 })
                .AsQueryable();
            if (vacations != null)
                return new Success(dataSource.ToResult(vacations));
            else
                return new Error(ErrorMsg.ServerError.ToDescriptionString());
        }
        public Result Create(VacationDto VacationDto)
        {
            if (!IsValidVacation(VacationDto.StartDate,VacationDto.EndDate))
                return new Error(ErrorMsg.VacationEndEarlierThanVacationStart.ToDescriptionString());
            if(!IsValidVacation(VacationDto.StartDate))
                return new Error(ErrorMsg.VacationStartInPast.ToDescriptionString());
            if (!IsValidVacation(VacationDto))
                return new Error(ErrorMsg.VacationIntersection.ToDescriptionString());
            var Vacation = _mapper.Map<Vacation>(VacationDto);
            _Context.Vacation.Add(Vacation);
            _Context.SaveChanges();
            VacationDto.Id = Vacation.Id;
            return new Success(VacationDto);
        }
        public Result Update(VacationDto VacationDto)
        {
            var vacationInDb = _Context.Vacation.SingleOrDefault(c => c.Id == VacationDto.Id);

            if (vacationInDb == null)
                return new Error(ErrorMsg.NotFound.ToDescriptionString());
            if (!IsValidVacation(VacationDto.StartDate, VacationDto.EndDate))
                return new Error(ErrorMsg.VacationEndEarlierThanVacationStart.ToDescriptionString());
            if (!IsValidVacation(VacationDto.StartDate))
                return new Error(ErrorMsg.VacationStartInPast.ToDescriptionString());
            if (!IsValidVacation(VacationDto))
                return new Error(ErrorMsg.VacationIntersection.ToDescriptionString());
            _mapper.Map(VacationDto, vacationInDb);
            _Context.Entry(vacationInDb).State = EntityState.Modified;
            _Context.SaveChanges();
            return new Success(VacationDto);
        }
        public Result Delete(Guid ID)
        {
            var vacation = _Context.Vacation
               .Where(x => x.Id == ID)
               .FirstOrDefault();
            if (vacation == null)
                return new Error(ErrorMsg.NotFound.ToDescriptionString());
            _Context.Vacation.Remove(vacation);
            _Context.SaveChanges();
            var VacationDto = _mapper.Map<Vacation, VacationDto>(vacation);
            return new Success(VacationDto);
        }

        #region helpers

        private bool IsValidVacation(DateTime StartDate, DateTime EndDate)
        {
            return (StartDate >= EndDate)? false:true;
        }
        private bool IsValidVacation(DateTime StartDate)
        {
            var t = ToTimeZone(StartDate);
            return (ToTimeZone(StartDate) < ToTimeZone(DateTime.Now)) ? false : true; 
        }
        private bool IsValidVacation(VacationDto VacationDto)
        {
            var UserVacations = _Context.Vacation.Where(x => x.FkUserId == VacationDto.FkUserId).ToList();
            foreach(Vacation v in UserVacations)
            {
                if (DateTimeIntervalIntersection(VacationDto.StartDate, VacationDto.EndDate,
                    v.StartDate, v.EndDate))
                    return false;
            }
            return true;
        }
        private bool DateTimeIntervalIntersection(DateTime NewVacationStart, DateTime NewVacationEnd, DateTime VacationStart, DateTime VacationEnd)
        {
            //  |--- Vacation ---|
            //     | --- NewVacation --- |


            //     | --- Vacation --- |
            //| --- NewVacation ---- |


            //| -------- Vacation -------- |
            //     | --- NewVacation --- |

            //     | --- Vacation --- |
            //| -------- NewVacation -------- |


            bool Intersect = false;
            if (NewVacationStart == VacationStart || NewVacationEnd == VacationEnd)
                return true; // If any set is the same time, then by default there must be some overlap. 

            if (VacationStart < NewVacationStart)
            {
                if (VacationEnd > NewVacationStart && VacationEnd < NewVacationEnd)
                    return true; // Condition 1

                if (VacationEnd > NewVacationEnd)
                    return true; // Condition 3
            }
            else
            {
                if (NewVacationEnd > VacationStart && NewVacationEnd < VacationEnd)
                    return true; // Condition 2

                if (NewVacationEnd > VacationEnd)
                    return true; // Condition 4
            }


            return Intersect;
        }
        #endregion
    }
}
