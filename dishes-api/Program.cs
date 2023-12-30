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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DishesDbContext>
(options => options.UseSqlite("name=ConnectionStrings:DishesDBConnectionString"));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
app.UseExceptionHandler(configureApplicationBuilder =>
{
       configureApplicationBuilder.Run(
           async context =>
           {
               context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
               context.Response.ContentType = "text/html";
               await context.Response.WriteAsync("An unexpected problem happened.");
           });
    
});

app.UseHttpsRedirection();

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
