﻿using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using FinanceLab.Client.Application.Abstractions;
using FinanceLab.Client.Infrastructure.Converters;
using FinanceLab.Client.Infrastructure.Providers;
using FinanceLab.Client.Infrastructure.Services;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FinanceLab.Client.Infrastructure.Extensions;

[PublicAPI]
public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationCore();
        services.AddSingleton<AuthenticationStateProvider, HostAuthenticationStateProvider>();

        services.AddSingleton<IHostAuthenticationStateProvider>(serviceProvider =>
            (HostAuthenticationStateProvider)serviceProvider.GetRequiredService<AuthenticationStateProvider>());

        return services;
    }

    public static IServiceCollection AddHttpClientService(this IServiceCollection services, string baseAddress)
    {
        services.AddHttpClient(Options.DefaultName, client =>
        {
            var language = new StringWithQualityHeaderValue(CultureInfo.CurrentUICulture.Name);

            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.AcceptLanguage.Clear();
            client.DefaultRequestHeaders.AcceptLanguage.Add(language);
        });

        services.AddSingleton<IHttpClientService, HttpClientService>();

        return services;
    }

    public static IServiceCollection AddJsonSerializerOptions(this IServiceCollection services)
        => services.AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            Converters =
            {
                new DateTimeOffsetConverter(),
                new HttpValidationProblemDetailsJsonConverter(),
                new ProblemDetailsJsonConverter()
            }
        });
}