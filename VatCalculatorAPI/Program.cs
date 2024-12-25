using System.Reflection;
using System.Text.Json;
using FluentValidation;
using FluentValidation.AspNetCore;
using VatCalculatorAPI.Behaviors;
using VatCalculatorAPI.Handlers;
using VatCalculatorAPI.Services;
using VatCalculatorAPI.Services.Interfaces;
using VatCalculatorAPI.Validators;

namespace VatCalculatorAPI;

/// <summary>
///  The main entry point for the application.
/// </summary>
public class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        // Add services
        builder.Services.AddScoped<IVatCalculatorService, VatCalculatorService>();
        
        // Add validators
        builder.Services.AddFluentValidationAutoValidation()
            .AddValidatorsFromAssemblyContaining<VatCalculatorRequestValidator>();
        
        // Add MediatR
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(RequestResponseLoggingBehavior<,>));
        });
        
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        
        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();
        app.MapControllers();
        app.UseExceptionHandler();
        
        app.Run();
    }
}