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
    public interface ILocationService : IBaseService
    {
        Result Get(int ID);
        Result GetAll(DataSource dataSource);
        Result Create(LocationDto Location);
        Result Update(LocationDto Location);
        Result ChangeLocationStatus(int ID, bool Status);
        Result Delete(int ID);
    }
    public class LocationService : BaseService, ILocationService
    {
        public LocationService(IOptions<AppSettings> AppSettings, IMapper Mapper, BaseDataBaseContext Context, IHttpContextAccessor contextAccessor, IHostingEnvironment env)
            : base(AppSettings, Mapper, Context, contextAccessor, env) { }
        public Result Get(int ID)
        {
            var location = _Context.Location
                .Where(x => x.Id == ID && x.IsDeleted == false)
                .Select(x => new LocationDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    NameAr = x.NameAr,
                    Description = x.Description,
                    IsActive = x.IsActive

                })
                .FirstOrDefault();
            if (location != null)
                return new Success(location);
            else return new Error(ErrorMsg.NotFound.ToDescriptionString());
        }
        public Result GetAll(DataSource dataSource)
        {
            var locations= _Context.Location
                .Where(x => x.IsDeleted == false)
                 .Select(x => new LocationDto
                 {
                     Id = x.Id,
                     Name = x.Name,
                     NameAr = x.NameAr,
                     Description = x.Description,
                     IsActive = x.IsActive

                 })
                .AsQueryable();
            if (locations != null)
            {
                var res = new Success(dataSource.ToResult(locations));
                return res;
            }
            else
                return new Error(ErrorMsg.ServerError.ToDescriptionString());
        }
        public Result Create(LocationDto LocationDto)
        {
            if (IsUniqueLocation(LocationDto.Id, LocationDto.Name, LocationDto.NameAr))
            {
                var Location = _mapper.Map<Location>(LocationDto);
                _Context.Location.Add(Location);
                _Context.SaveChanges();
                LocationDto.Id = Location.Id;
                return new Success(LocationDto);
            }
            else
                return new Error(ErrorMsg.NotUnique.ToDescriptionString());
        }
        public Result Update(LocationDto LocationDto)
        {
            if (LocationDto.Id == 0)
                return new Error(ErrorMsg.NotFound.ToDescriptionString());
            if (IsUniqueLocation(LocationDto.Id, LocationDto.Name, LocationDto.NameAr))
            {
                var locationInDb = _Context.Location.SingleOrDefault(c => c.Id == LocationDto.Id);

                if (locationInDb == null)
                    return new Error(ErrorMsg.NotFound.ToDescriptionString());

                _mapper.Map(LocationDto, locationInDb);
                _Context.Entry(locationInDb).State = EntityState.Modified;
                _Context.SaveChanges();
                return new Success(LocationDto);
            }
            else
                return new Error(ErrorMsg.NotUnique.ToDescriptionString());
        }
        public Result ChangeLocationStatus(int ID, bool Status)
        {
            var Location = _Context.Location
                .Where(x => x.Id == ID)
                .FirstOrDefault();
            if(Location==null)
                return new Error(ErrorMsg.NotFound.ToDescriptionString());
            Location.IsActive = Status;
            _Context.SaveChanges();
            var LocationDto = _mapper.Map<Location, LocationDto>(Location);
            return new Success(LocationDto);
             
        }
        public Result Delete(int ID)
        {
            var Location = _Context.Location
               .Where(x => x.Id == ID && x.IsDeleted!=true)
               .FirstOrDefault();
            if (Location == null)
                return new Error(ErrorMsg.NotFound.ToDescriptionString());
            Location.IsDeleted = true;
            _Context.SaveChanges();
            var LocationDto = _mapper.Map<Location, LocationDto>(Location);
            return new Success(LocationDto);
        }

        #region helpers

        private bool IsUniqueLocation(int ID, string Name, string NameAr)
        {
            return !_Context.Location.Any(x => x.Id != ID && (x.Name.ToLower().Equals(Name.ToLower()) || x.NameAr.Equals(NameAr)) && (x.IsDeleted == false));
        }
        #endregion
    }
}
