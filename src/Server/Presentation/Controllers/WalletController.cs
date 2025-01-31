﻿using FinanceLab.Server.Domain.Models.Queries;
using FinanceLab.Shared.Application.Constants;
using FinanceLab.Shared.Domain.Models.Dtos;
using FinanceLab.Shared.Domain.Models.Inputs;
using FinanceLab.Shared.Domain.Models.Outputs;
using Microsoft.AspNetCore.Mvc;

namespace FinanceLab.Server.Presentation.Controllers;

public sealed class WalletController : BaseController
{
    [HttpGet(ApiRouteConstants.GetWallet)]
    public async Task<ActionResult<WalletDto>> GetAsync([FromQuery] WalletInput input)
    {
        var userName = EnsureAuthorizationForUserName(input.UserName);
        var request = new GetWalletQuery(userName, input.CoinCode);
        var wallet = await Mediator.Send(request);

        return Ok(wallet);
    }

    [HttpGet(ApiRouteConstants.GetWalletList)]
    public async Task<ActionResult<WalletListOutput>> GetListAsync([FromQuery] WalletListInput input)
    {
        var userName = EnsureAuthorizationForUserName(input.UserName);
        var request = new GetWalletListQuery(userName, input.Filter, input.Page, input.Sort);
        var walletList = await Mediator.Send(request);

        return Ok(walletList);
    }
}