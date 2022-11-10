using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using Model.Models;
using Services;
using Shared;

namespace WebApp.Controllers
{
    [ApiController]
    public class VacationController : BaseController
    {
        IVacationService _VacationService;
        public VacationController(IVacationService VacationService, IHttpContextAccessor httpContextAccessor)
        {
            _VacationService = VacationService;
        }


        [HttpGet()]
        public Result Get(Guid ID)
        {
            return _VacationService.Get(ID);
        }


        [HttpGet]
        public Result GetAll([FromQuery]DataSource dataSource)
        {
            return _VacationService.GetAll(dataSource);
        }

        [HttpPost]
        public Result Create([FromBody]VacationDto Vacation)
        {
            return _VacationService.Create(Vacation);
        }

        [HttpPut]
        public Result Update(VacationDto Vacation)
        {
            return _VacationService.Update(Vacation);
        }

        [HttpDelete]
        public Result DeleteVacation(Guid id)
        {
            return _VacationService.Delete(id);
        }

    }
}