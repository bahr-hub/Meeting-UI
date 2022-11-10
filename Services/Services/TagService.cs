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
    public interface ITagService : IBaseService
    {
        Result Get(int ID);
        Result GetAll(DataSource dataSource);
        Result Create(TagDto Tag);
        Result Update(TagDto Tag);
        Result ChangeTagStatus(int ID, bool Status);
        Result Delete(int ID);
    }
    public class TagService : BaseService, ITagService
    {
        public TagService(IOptions<AppSettings> AppSettings, IMapper Mapper, BaseDataBaseContext Context, IHttpContextAccessor contextAccessor, IHostingEnvironment env)
            : base(AppSettings, Mapper, Context, contextAccessor, env) { }
        public Result Get(int ID)
        {
            var tag= _Context.Tag
                .Where(x => x.Id == ID && x.IsDeleted == false)
                 .Select(x => new TagDto
                 {
                     Id = x.Id,
                     Name = x.Name,
                     Description = x.Description,
                     IsActive = x.IsActive

                 })
                .FirstOrDefault();
            if (tag != null)
                return new Success(tag);
            else return new Error(ErrorMsg.NotFound.ToDescriptionString());
        }
        public Result GetAll(DataSource dataSource)
        {
            var tags= _Context.Tag
                .Where(x=>   x.IsDeleted == false)
                 .Select(x => new TagDto
                 {
                     Id = x.Id,
                     Name = x.Name,
                     Description = x.Description,
                     IsActive = x.IsActive

                 })
                .AsQueryable();
            if (tags != null)
                return new Success(dataSource.ToResult(tags));
            else
                return new Error(ErrorMsg.ServerError.ToDescriptionString());
        }
        public Result Create(TagDto TagDto)
        {
            if (TagDto.Name.Length > 50)
            {
                return new Error(ErrorMsg.tagNameTooLong.ToDescriptionString());
            }
            if (IsUniqueTag(TagDto.Id, TagDto.Name))
            {
                var Tag = _mapper.Map<TagDto, Tag>(TagDto);
                _Context.Tag.Add(Tag);
                _Context.SaveChanges();
                TagDto.Id = Tag.Id;
                return new Success(TagDto);
            }
            else
                return new Error(ErrorMsg.NotUnique.ToDescriptionString());
        }
        public Result Update(TagDto TagDto)
        {
            if(TagDto.Id==0)
                return new Error(ErrorMsg.NotFound.ToDescriptionString());
            if (IsUniqueTag(TagDto.Id, TagDto.Name))
            {
                var tagInDb = _Context.Tag.SingleOrDefault(c => c.Id == TagDto.Id);

                if (tagInDb == null)
                    return new Error(ErrorMsg.NotFound.ToDescriptionString());

                _mapper.Map(TagDto, tagInDb);
                _Context.Entry(tagInDb).State = EntityState.Modified;
                _Context.SaveChanges();
                return new Success(TagDto);
            }
            else
                return new Error(ErrorMsg.NotUnique.ToDescriptionString());
        }
        public Result ChangeTagStatus(int ID, bool Status)
        {
            var Tag = _Context.Tag
                .Where(x => x.Id == ID)
                .FirstOrDefault();
            if (Tag == null)
                return new Error(ErrorMsg.NotFound.ToDescriptionString());
            Tag.IsActive = Status;
            _Context.SaveChanges();
            var TagDto = _mapper.Map<Tag, TagDto>(Tag);
            return new Success(TagDto);
        }
        public Result Delete(int ID)
        {
            var Tag = _Context.Tag
               .Where(x => x.Id == ID)
               .FirstOrDefault();
            if (Tag == null)
                return new Error(ErrorMsg.NotFound.ToDescriptionString());
            Tag.IsDeleted = true;
            _Context.SaveChanges();
            var TagDto = _mapper.Map<Tag, TagDto>(Tag);
            return new Success(TagDto);
        }
        #region helpers
        private bool IsUniqueTag(int ID, string Name)
        {
            return !_Context.Tag.Any(x => x.Id != ID && x.Name.ToLower().Equals(Name.ToLower()) && ( x.IsDeleted == false));
        }
        #endregion
    }
}
