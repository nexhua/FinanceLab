﻿using System.Net;
using FinanceLab.Server.Application.Abstractions;
using FinanceLab.Server.Domain.Models.Commands;
using FinanceLab.Server.Domain.Models.Entities;
using FinanceLab.Shared.Application.Abstractions;
using FinanceLab.Shared.Domain.Models.Enums;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace FinanceLab.Server.Application.Handlers.Commands;

[UsedImplicitly]
public sealed class SignUpCommandHandler : BaseRequestHandler<SignUpCommand>
{
    private readonly IMongoDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public SignUpCommandHandler(IMongoDbContext dbContext, IPasswordHasher<User> passwordHasher,
        ISharedResources sharedResources) : base(sharedResources)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public override async Task<Unit> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _dbContext.Users
            .Find(u => u.UserName == request.UserName)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingUser is not null)
            Throw(HttpStatusCode.Conflict, "Username already taken.");

        var user = new User
        {
            UserName = request.UserName,
            FirstName = request.FirstName,
            LastName = request.LastName,
            GameDifficulty = request.GameDifficulty
        };

        switch (request.GameDifficulty)
        {
            case GameDifficulty.Sandbox:
                break;
            case GameDifficulty.Easy:
                user.Wallets.Add(new Wallet("TRY", 10_000));
                break;
            case GameDifficulty.Normal:
                user.Wallets.Add(new Wallet("TRY", 1_000));
                break;
            case GameDifficulty.Hard:
                user.Wallets.Add(new Wallet("TRY", 100));
                break;
            default:
                Throw(HttpStatusCode.BadRequest, L["InvalidGameDifficulty"]);
                break;
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
        await _dbContext.Users.InsertOneAsync(user, null, cancellationToken);

        return Unit.Value;
    }
}