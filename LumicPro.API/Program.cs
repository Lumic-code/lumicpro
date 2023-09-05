using LumicPro.Core.Entities;
using LumicPro.Core.Repository;
using LumicPro.Infrastructure.Context;
using LumicPro.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using NLog;
using NLog.Web;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using LumicPro.Infrastructure.Services;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();


try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Fast authentication scheme",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
           new OpenApiSecurityScheme
           {
               Reference = new OpenApiReference
               {
                   Type = ReferenceType.SecurityScheme,
                   Id = "Bearer"
               }

           },new List<string>()
        }

    });

    }
    );
    builder.Services.AddDbContext<LumicProContext>(options =>
                      options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddAutoMapper(typeof(Program));

    builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            options.SignIn.RequireConfirmedEmail = true)
           .AddEntityFrameworkStores<LumicProContext>()
           .AddDefaultTokenProviders();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt:Key").Value);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateAudience = false,
            ValidateIssuer = false
        };
    });
    builder.Services.AddScoped<IUploadService, UploadService>();

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("RegularRole", policy => policy.RequireRole("Regular"));
        options.AddPolicy("AdminOrSuperAdmin", policy => policy.RequireAssertion(context =>
                              context.User.IsInRole("Admin") || context.User.IsInRole("SuperAdmin")));
        options.AddPolicy("AdminAndSuperAdmin", policy => policy.RequireAssertion(context =>
                          context.User.IsInRole("Admin") && context.User.IsInRole("SuperAdmin")));
    });



    //pipeline is below **********************************************************************
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseExceptionHandler(builder =>
        {
            builder.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var error = context.Features.Get<IExceptionHandlerPathFeature>();

                logger.Error($"Error path: {error.Path}, Error thrown: {error.Error.Message}, " +
                    $"Inner Message: {error.Error.InnerException}");
            });
        });
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception exception)
{
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}

