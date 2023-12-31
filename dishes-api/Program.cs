using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using AutoMapper;
using DishesAPI.DbContexts;
using DishesAPI.EndpointHandlers;
using DishesAPI.Entities;
using DishesAPI.Models;
using DishesAPI.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Log4Net.AspNetCore;
using SQLitePCL;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddHttpsRedirection(options =>
// {
//     options.RedirectStatusCode = (int)HttpStatusCode.TemporaryRedirect;
//     options.HttpsPort = 7140;
// });

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DishesDbContext>
(options => options.UseSqlite("name=ConnectionStrings:DishesDBConnectionString"));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddProblemDetails();

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdmin", policy =>
    {
        policy
            .RequireRole("admin")
            .RequireClaim("country", "US");

    });

#region logging
builder.Logging.AddLog4Net(log4NetConfigFile: "log4net.config");
#endregion

//builder.Services.AddCors();  // CORS Error: XMLHttpRequest. has been blocked by CORS policy: No 'Access-Control-Allow-Origin'
var app = builder.Build();

// app.UseCors(builder => builder // CORS remedy
// .AllowAnyOrigin()
// .AllowAnyMethod()
// .AllowAnyHeader()
// );

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler(configureApplicationBuilder =>
    {
        app.UseExceptionHandler();
        //    configureApplicationBuilder.Run(
        //        async context =>
        //        {
        //            context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
        //            context.Response.ContentType = "text/html";
        //            await context.Response.WriteAsync("An unexpected problem happened.");
        //        });
        
    });

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// configure API endpoint routing paths
app.RegisterDishesEndpoints();
app.RegisterIngredientsEndpoints();

// recreate & migrate the database on each run, for demo purposes
using (var serviceScope = app.Services.GetService<IServiceScopeFactory>()?.CreateScope())
{
    var context = serviceScope?.ServiceProvider.GetRequiredService<DishesDbContext>();
    context?.Database.EnsureDeleted();
    context?.Database.Migrate();
}

app.Run();
