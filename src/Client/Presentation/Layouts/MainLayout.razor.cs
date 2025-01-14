﻿using MudBlazor;

namespace FinanceLab.Client.Presentation.Layouts;

public partial class MainLayout
{
    private readonly MudTheme _muudstrapTheme = new()
    {
        
        
        
        Palette = new Palette
        {
            AppbarBackground = "#e0eef6",
            AppbarText = "#000000"
        },
        Typography = new Typography
        {
            H6 = new H6
            {
                FontFamily = new[]
                {
                    "system-ui", "-apple-system", "Segoe UI", "Roboto ", "Helvetica Neue", "Arial", "Noto Sans",
                    "Liberation Sans", "sans-serif", "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol",
                    "Noto Color Emoji"
                },
                FontSize = "1.25rem",
                FontWeight = 400,
                LineHeight = 1.7,
                LetterSpacing = "normal"
            },
            Button = new Button
            {
                
                FontFamily = new[]
                {
                    "system-ui", "-apple-system", "Segoe UI", "Roboto ", "Helvetica Neue", "Arial", "Noto Sans",
                    "Liberation Sans", "sans-serif", "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol",
                    "Noto Color Emoji"
                },
                FontSize = "1rem",
                FontWeight = 400,
                LineHeight = 1.5,
                LetterSpacing = "normal"
            }
        }
    };
}