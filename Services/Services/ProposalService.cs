using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Model.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static Shared.SystemEnums;
using Model.DTO;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public interface IProposalService : IBaseService
    {
        Result Add(Proposals Proposal);
        Result Accept(Guid Id);
        Result Reject(Guid Id);
    }
    public class ProposalService : BaseService , IProposalService
    {
        IAccountService _accountService;
        IMapper _mapper;
        IMeetingService _meetingService;
        public ProposalService(IMeetingService meetingService, IOptions<AppSettings> AppSettings, IAccountService accountService, IMapper Mapper, BaseDataBaseContext Context, IHttpContextAccessor contextAccessor, IHostingEnvironment env) : base(AppSettings, Mapper, Context, contextAccessor, env)
        {
            _accountService = accountService;
            _mapper = Mapper;
            _meetingService = meetingService;
        }
        public Result Add(Proposals Proposal)
        {
            if (Proposal.ProposedStartTime > Proposal.ProposedEndTime)
            {
                //if the proposing start Time is after the proposed end time
                return new Error("");
            }
            else if (Proposal.ProposedStartTime < DateTime.Now)
            {
                //if proposing a time in the past
                return new Error("");
            }
            else
            {
                //all ok

                _Context.Proposals.Add(Proposal);
                //send the email to the user
                _Context.SaveChanges();
                SendProposal(Proposal.MeetingId, Proposal);
                return new Success(Proposal);
            }
        }
        public Result Accept(Guid Id)
        {
            Proposals Proposal = _Context.Proposals.SingleOrDefault(x => x.Id.Equals(Id));
            Meeting meeting = _Context.Meeting.SingleOrDefault(x=>x.Id.Equals(Proposal.MeetingId));
            if (Proposal.ProposedStartTime < DateTime.Now)
            {
                //if proposing a time in the past
                return new Error("");
            }
            else if (Proposal.ProposalState != (int)ProposalState.New)
            {
                return new Error("");
            }
            else
            {
                //all is ok
                meeting.From = Proposal.ProposedStartTime;
                meeting.To = Proposal.ProposedEndTime;
                Proposal.ProposalState = (int)ProposalState.Approved;
                _Context.SaveChanges();
                _meetingService.Update(_meetingService.Get(meeting.Id).Data);
                Proposal.Meeting = null;
                return new Success(Proposal,"Proposal accepted");
            }
        }

        public Result Reject(Guid Id)
        {
            
            Proposals Proposal = _Context.Proposals.SingleOrDefault(x => x.Id.Equals(Id));
            if(Proposal.ProposalState != (int)ProposalState.New)
            {
                return new Error("");
            }
            else
            {
                Proposal.ProposalState = (int)ProposalState.Rejected;
                return new Success("");
            }

        }

        private void SendProposal(Guid MeetingId,Proposals proposal)
        {
            Meeting meeting = _Context.Meeting.Find(MeetingId);
            User meetingCreator = _Context.User.Find(meeting.CreatedBy);
            UserDto emailSender = new UserDto
            {
                Email = CurrentUser.Email,
                Name = CurrentUser.Name
            };
            List<UserDto> emailRecipients = new List<UserDto>();
            emailRecipients.Add(new UserDto { Email = meetingCreator.Email, Name = meetingCreator.Name });
            var template = _appSettings.EmailTemplates.FirstOrDefault(x => x.Name.Equals("MeetingTimeChangePurpose"));
            string Accept = QueryConstructor("Proposals", "Accept", proposal.Id, meetingCreator.Id, meetingCreator.IsAdmin,meeting.Id);
            string Reject = QueryConstructor("Proposals", "Reject", proposal.Id, meetingCreator.Id, meetingCreator.IsAdmin,meeting.Id);
            Dictionary<string, string> _params = new Dictionary<string, string>()
            {
                {"Reciver",meetingCreator.Name},
                {"PurposedBy", CurrentUser.Name },
                {"MeetingName", meeting.Name },
                {"ProposedFrom", proposal.ProposedStartTime.ToString()},
                {"ProposedTo", proposal.ProposedEndTime.ToString()},
                {"AcceptLink", Accept },
                {"RejectLink", Reject }
            };
            this.SendEmail(new Mail
            {
                From = emailSender,
                To = emailRecipients,
                Subject = "proposal of a new meeting time",
                Text = string.Format("{0} participant of meeting: {1} is proposing to change the meeting time to start from {2} and end at {3}", CurrentUser.Name, meeting.Name, proposal.ProposedStartTime,proposal.ProposedEndTime),
                Html = this.PopulateBody(string.Concat(this._env.WebRootPath, "/", template.Path), _params),
                Attachments = template.Images
            }, template.Path, _params);

        }

        private string QueryConstructor(string module, string action, Guid proposalId, Guid userId, bool isAdmin,Guid meetingId)
        {
            var _token = _accountService.GenerateToken(new UserDto { Id = userId, IsAdmin = isAdmin });
            var TokenHandler = new JwtSecurityTokenHandler();
            //token/module/action/meetingid
            return _currentBaseUrl + string.Format(@"Redirect?access_token={0}&module={1}&action={2}&Id={3}&meetingId={4}", TokenHandler.WriteToken(_token), module, action, proposalId, meetingId);

        }


    }

}
