
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using RestaurantTableBookingApp.Data;
using RestaurantTableBookingApp.Service;
using Serilog;
using System.Net;

namespace RestaurantTableBookingApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Debug()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .CreateBootstrapLogger();
            try
            {
                var builder = WebApplication.CreateBuilder(args);
                var configuration = builder.Configuration;


                builder.Services.AddApplicationInsightsTelemetry();

                builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
                .WriteTo.ApplicationInsights(
                  services.GetRequiredService<TelemetryConfiguration>(),
                  TelemetryConverter.Events));

                Log.Information("Starting the application...");
                // Add services to the container.

                builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();
                builder.Services.AddScoped<IRestaurantService, RestaurantService>();

                builder.Services.AddDbContext<RestaurantTableBookingDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DbContext") ?? "")
                .EnableSensitiveDataLogging()
                );

                builder.Services.AddControllers();
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                var app = builder.Build();

                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                        var exception = exceptionHandlerPathFeature?.Error;

                        Log.Error(exception, "Unhandled exception occurred. {ExceptionDetails}", exception?.ToString());
                        Console.WriteLine(exception?.ToString());
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        await context.Response.WriteAsync("An unexpected error occurred. Please try again later.");
                    });
                });

                app.UseMiddleware<RequestResponseLoggingMiddleware>();


                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                    c.RoutePrefix = "swagger"; 
                });

                // Configure the HTTP request pipeline.
                //if (app.Environment.IsDevelopment())
                //{
                //    app.UseSwagger();
                //    app.UseSwaggerUI();
                //}

                app.UseHttpsRedirection();

                app.UseAuthorization();


                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
