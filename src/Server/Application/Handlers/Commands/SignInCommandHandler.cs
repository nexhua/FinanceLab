﻿using System.Net;
using System.Security.Claims;
using FinanceLab.Server.Application.Abstractions;
using FinanceLab.Server.Domain.Models.Commands;
using FinanceLab.Server.Domain.Models.Entities;
using FinanceLab.Shared.Application.Abstractions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace FinanceLab.Server.Application.Handlers.Commands;

[UsedImplicitly]
public sealed class SignInCommandHandler : BaseRequestHandler<SignInCommand>
{
    private readonly IMongoDbContext _dbContext;
    private readonly HttpContext _httpContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public SignInCommandHandler(IMongoDbContext dbContext, IHttpContextAccessor httpContextAccessor,
        IPasswordHasher<User> passwordHasher, ISharedResources sharedResources) : base(sharedResources)
    {
        _dbContext = dbContext;
        _httpContext = httpContextAccessor.HttpContext!;
        _passwordHasher = passwordHasher;
    }

    public override async Task<Unit> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .Find(u => u.UserName == request.UserName)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            Throw(HttpStatusCode.NotFound, L["UserNotFound"]);

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
            Throw(HttpStatusCode.Unauthorized, L["IncorrectPassword"]);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(nameof(User.GameDifficulty), ((int)user.GameDifficulty).ToString())
        };

        if (user.Role is not null)
            claims.Add(new Claim(ClaimTypes.Role, user.Role));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var authenticationProperties = new AuthenticationProperties { IsPersistent = request.Remember };

        await _httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            authenticationProperties);

        return Unit.Value;
    }
}