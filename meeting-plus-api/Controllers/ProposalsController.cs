using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Models;
using Services;
using Shared;
using WebApp.Controllers;

namespace MeetingPlus.API.Controllers
{
    [ApiController]

    public class ProposalsController : BaseController
    {
        IProposalService _proposalService;
        public ProposalsController(IProposalService proposalService)
        {
            _proposalService = proposalService;
        }

        [HttpPost()]
        public Result Accept(Guid Id)
        {
            return _proposalService.Accept(Id);
        }

        [HttpPost()]
        public Result Reject(Guid Id)
        {
            return _proposalService.Reject(Id);
        }

        [HttpPost()]
        public Result Add([FromBody]Proposals proposal)
        {
            return _proposalService.Add(proposal);
        }

    }
}