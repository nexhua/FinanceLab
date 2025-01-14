﻿using JetBrains.Annotations;

namespace FinanceLab.Shared.Domain.Models.Inputs;

[PublicAPI]
public sealed class SignInInput
{
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public bool Remember { get; set; }
}