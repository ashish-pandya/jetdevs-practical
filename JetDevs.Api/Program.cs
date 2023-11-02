using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using JetDevs.Api.SeedData;
using JetDevs.Common.Web.Seeding;
using Microsoft.AspNetCore.Builder;
using JetDevs.Api.Context;
using JetDevs.Api.Models.DbEntities;
using JetDevs.Common.Web.Security;
using JetDevs.Common.Web.Setup;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using JetDevs.Common.Web.Email;
using JetDevs.Common.Web.ExceptionHandling;
using JetDevs.Common.Web.Filters;
using FluentValidation.AspNetCore;
using System.Reflection;
using System;
using System.IO;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration["ConnectionString"];
var SERVICE_NAME = "JetDevs.Api";
var _swaggerFileName = Path.Combine(Path.GetDirectoryName((new Uri(Assembly.GetExecutingAssembly().CodeBase)).LocalPath),
    string.Format("{0}.xml", SERVICE_NAME));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("JetDevs.Api"));
});
builder.Services.AddIdentity<User, IdentityRole>(config =>
{
    config.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

//var builder = new BuilderOptions().GetBuilderOptions(services);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.ConfigureAuth(builder.Configuration);
builder.Services.ConfigureEmailSender(builder.Configuration);

builder.Services.AddSingleton<IJWTFactory, JWTFactory>();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;
    options.AddPolicy("Api", context => RateLimitPartition.GetFixedWindowLimiter(
         partitionKey: context.Connection.RemoteIpAddress,
        factory: partition => new FixedWindowRateLimiterOptions
        {
            AutoReplenishment = true,
            PermitLimit = 100,
            Window = TimeSpan.FromMinutes(1)
        }));
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ADMINISTRATOR_POLICY",
        authBuilder =>
        {
            authBuilder.RequireRole("Administrator");
        });
});
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ResponseExceptionFilterAttribute());
    options.Filters.Add(typeof(LoggerExceptionFilterAttribute));
    options.Filters.Add(typeof(ValidationFilter));

}).AddFluentValidation(opt =>
{
    opt.RegisterValidatorsFromAssemblyContaining(typeof(Program));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (File.Exists(_swaggerFileName))
{
    Swagger.ConfigureServices(builder.Services, _swaggerFileName);
}
var app = builder.Build();
await Seeder<SeedDatabase, Program>.Seed(app);
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    if (File.Exists(_swaggerFileName))
    {
        Swagger.Configure(app, SERVICE_NAME);
    }
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.Run();