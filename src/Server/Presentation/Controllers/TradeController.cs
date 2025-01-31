﻿using System.Security.Claims;
using FinanceLab.Server.Application.Abstractions;
using FinanceLab.Server.Domain.Models.Commands;
using FinanceLab.Server.Domain.Models.Queries;
using FinanceLab.Shared.Application.Constants;
using FinanceLab.Shared.Domain.Models.Inputs;
using FinanceLab.Shared.Domain.Models.Outputs;
using Microsoft.AspNetCore.Mvc;

namespace FinanceLab.Server.Presentation.Controllers;

public sealed class TradeController : BaseController
{
    private readonly IBinanceService _binanceService;
    private readonly HttpContext _httpContext;

    public TradeController(IHttpContextAccessor httpContextAccessor, IBinanceService binanceService)
    {
        _httpContext = httpContextAccessor.HttpContext!;
        _binanceService = binanceService;
    }

    [HttpGet(ApiRouteConstants.GetTradeList)]
    public async Task<ActionResult<TradeListOutput>> GetListAsync([FromQuery] TradeListInput input)
    {
        var userName = EnsureAuthorizationForUserName(input.UserName);
        var request = new GetTradeListQuery(userName, input.Filter, input.Page, input.Sort);
        var tradeList = await Mediator.Send(request);

        return Ok(tradeList);
    }

    [HttpPost(ApiRouteConstants.PostTrade)]
    public async Task<ActionResult<TradeOutput>> PostAsync([FromBody] TradeInput input)
    {
        var userName = _httpContext.User.FindFirstValue(ClaimTypes.Name);
        var price = await _binanceService.GetTickerPriceAsync(input.BaseCoinCode + input.QuoteCoinCode);
        var tradeRequest = new TradeCommand(
            userName, input.Side, input.BaseCoinCode, input.QuoteCoinCode, input.Quantity, price);

        await Mediator.Send(tradeRequest);

        var baseWalletQuery = new GetWalletQuery(userName, input.BaseCoinCode);
        var quoteWalletQuery = new GetWalletQuery(userName, input.QuoteCoinCode);
        var baseWallet = await Mediator.Send(baseWalletQuery);
        var quoteWallet = await Mediator.Send(quoteWalletQuery);
        var tradeOutput = new TradeOutput
        {
            BaseWallet = baseWallet,
            QuoteWallet = quoteWallet,
            Price = price
        };

        return Ok(tradeOutput);
    }
}