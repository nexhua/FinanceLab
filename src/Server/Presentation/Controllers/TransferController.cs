﻿using FinanceLab.Server.Domain.Models.Commands;
using FinanceLab.Server.Domain.Models.Queries;
using FinanceLab.Shared.Application.Constants;
using FinanceLab.Shared.Domain.Models.Inputs;
using FinanceLab.Shared.Domain.Models.Outputs;
using Microsoft.AspNetCore.Mvc;

namespace FinanceLab.Server.Presentation.Controllers;

public sealed class TransferController : BaseController
{
    [HttpGet(ApiRouteConstants.GetTransferList)]
    public async Task<ActionResult<TransferListOutput>> GetListAsync([FromQuery] TransferListInput input)
    {
        var userName = EnsureAuthorizationForUserName(input.UserName);
        var request = new GetTransferListQuery(input.UserName, input.Filter, input.Page, input.Sort);
        var transferList = await Mediator.Send(request);

        return Ok(transferList);
    }

    [HttpGet(ApiRouteConstants.PostTransfer)]
    public async Task<IActionResult> PostAsync([FromQuery] TransferInput input)
    {
        var request = new TransferCommand(input.CoinCode, input.Amount);
        await Mediator.Send(request);

        return Ok();
    }
}