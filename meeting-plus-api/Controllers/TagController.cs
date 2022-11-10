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
    public class TagController : BaseController
    {
        ITagService _TagService;
        public TagController(ITagService TagService, IHttpContextAccessor httpContextAccessor)
        {
            _TagService = TagService;
        }


        [HttpGet()]
        public Result Get(int ID)
        {
            return _TagService.Get(ID);
        }


        [HttpPost]
        public Result GetAll([FromBody]DataSource dataSource)
        {
            return _TagService.GetAll(dataSource);
        }

        [HttpPost]
        public Result Create([FromBody]TagDto Tag)
        {
            return _TagService.Create(Tag);
        }

        [HttpPut]
        public Result Update(TagDto Tag)
        {
            return _TagService.Update(Tag);
        }


        [HttpPost]
        public Result ChangeTagStatus(TagChangeStatus Tag)
        {
            return _TagService.ChangeTagStatus(Tag.ID, Tag.IsActive);
        }

        [HttpDelete]
        public Result DeleteTag(int id)
        {
            return _TagService.Delete(id);
        }

    }
}