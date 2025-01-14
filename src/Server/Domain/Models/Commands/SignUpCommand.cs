﻿using FinanceLab.Shared.Domain.Models.Enums;
using JetBrains.Annotations;
using MediatR;

namespace FinanceLab.Server.Domain.Models.Commands;

[PublicAPI]
public sealed record SignUpCommand(
    string UserName,
    string FirstName,
    string LastName,
    string Password,
    GameDifficulty GameDifficulty) : IRequest;