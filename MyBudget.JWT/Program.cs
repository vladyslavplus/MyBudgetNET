using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using MyBudget.BLL;
using MyBudget.BLL.Configuration;
using MyBudget.DAL;
using MyBudget.JWT.Configuration;
using MyBudget.JWT.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var smtpSettings = configuration.GetSection("Smtp");
var smtpClient = new SmtpClient(smtpSettings["Host"])
{
    Port = int.Parse(smtpSettings["Port"] ?? string.Empty),
    Credentials = new NetworkCredential(smtpSettings["User"], smtpSettings["Pass"]),
    EnableSsl = true
};

builder.Services
    .AddFluentEmail(smtpSettings["Sender"])
    .AddSmtpSender(smtpClient);

builder.Services.AddDataAccess(configuration);
builder.Services.AddBusinessLogic();

builder.Services.AddMapsterConfiguration();

// Add services to the container.
builder.Services.AddJwtAuthentication(configuration);

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Enter your JWT Access Token",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    options.AddSecurityDefinition("Bearer", jwtSecurityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, [] }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();