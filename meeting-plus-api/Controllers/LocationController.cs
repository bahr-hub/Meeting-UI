using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using Model.Models;
using Services;
using Shared;
using WebApp.Filters;

namespace WebApp.Controllers
{
    [ApiController]
    public class LocationController : BaseController
    {
        ILocationService _LocationService;
        public LocationController(ILocationService LocationService, IHttpContextAccessor httpContextAccessor)
        {
            _LocationService = LocationService;
        }


        [HttpGet()]
        public Result Get(int ID)
        {
            return _LocationService.Get(ID);
        }


        [HttpPost]
        public Result GetAll([FromBody]DataSource dataSource)
        {
            return _LocationService.GetAll(dataSource);
        }

        [HttpPost]
        public Result Create([FromBody]LocationDto Location)
        {
            return _LocationService.Create(Location);
        }

        [HttpPut]
        public Result Update(LocationDto Location)
        {
            return _LocationService.Update(Location);
        }


        [HttpPost]
        public Result ChangeLocationStatus(LocationChangeStatus Location)
        {
            return _LocationService.ChangeLocationStatus(Location.ID, Location.IsActive);
        }


        [HttpDelete]
        public Result DeleteLocation(int id)
        {
            return _LocationService.Delete(id);
        }

    }
}