using System.Security.Claims;
using AutoMapper;
using DishesAPI.DbContexts;
using DishesAPI.Entities;
using DishesAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace DishesAPI.EndpointHandlers;

public static class DishesHandler
{
    public static async Task<Ok<IEnumerable<DishDto>>>
    GetDishesAsync(DishesDbContext dishDB,
        IMapper mapper,
        ILogger<string> logger,
        ClaimsPrincipal claim,
        string? name)
    {
        Console.WriteLine($"User authenticated: {claim.Identity?.IsAuthenticated} ");
        logger.LogInformation("Getting the dishes...{name}", name);

        return TypedResults.Ok(mapper.Map<IEnumerable<DishDto>>(await dishDB.Dishes
            .Where(d => name == null | (name != null && d.Name.Contains(name)))
            .ToListAsync()));
    }

    public static async Task<Results<NotFound, Ok<DishDto>>>
    GetDishByIdAsync(DishesDbContext dishDB, Guid dishId, IMapper mapper, ILogger<string> logger)
    {
        Dish? dishEntity = await dishDB.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
        logger.LogInformation("Get dish by ID...{dishId}", dishId);
        if (dishEntity == null)
            return TypedResults.NotFound();
        else
            return TypedResults.Ok(mapper.Map<DishDto>(dishEntity));
    }

    public static async Task<Results<NotFound, Ok<DishDto>>>
    GetDishByNameAsync(DishesDbContext dishDB, IMapper mapper, string dishName, ILogger<string> logger)
    {
        logger.LogInformation("Getting dish by name ...{dishName}", dishName);
        return TypedResults.Ok(mapper.Map<DishDto>(await dishDB.Dishes.FirstOrDefaultAsync(d => d.Name == dishName)));
    }

    public static async Task<CreatedAtRoute<DishDto>>

    AddDishAsync(DishesDbContext dishDB,
        IMapper mapper,
        ILogger<string> logger,
        // HttpContext http,
        // LinkGenerator linker,
        DishCreateDto dishCreateDto)
    {
        Dish? dishEntity = mapper.Map<Dish>(dishCreateDto);
        logger.LogInformation("Adding new dish: {Name}", dishCreateDto.Name);
        dishDB.Add(dishEntity);
        await dishDB.SaveChangesAsync();

        var dishReturn = mapper.Map<DishDto>(dishEntity);
        // string? returnLink = linker.GetUriByName(http, "GetDish", new { dishId = dishReturn.Id });

        return TypedResults.CreatedAtRoute(dishReturn, "GetDish", new { dishId = dishReturn.Id });
    }

    public static async Task<Results<NotFound, NoContent>>
    DeleteDishAsync(
        DishesDbContext dishesDB,
        ILogger<string> logger,
        Guid dishId)
    {
        logger.LogInformation("Deleting dish ^^^ {Id}", dishId);
        Dish? dishEntity = await dishesDB.Dishes.FirstOrDefaultAsync(o => o.Id == dishId);
        if (dishEntity == null)
        {
            return TypedResults.NotFound();
        }

        dishesDB.Dishes.Remove(dishEntity);
        await dishesDB.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    public static async Task<Results<NotFound, NoContent>>
    UpdateDishAsync(
    DishesDbContext dishDB,
    IMapper mapper,
    Guid dishId,
    DishUpdateDto dishUpdateDto)
    {

        Dish? dishEntity = await dishDB.Dishes.FirstOrDefaultAsync(a => a.Id == dishId);
        if (dishEntity == null)
        {
            return TypedResults.NotFound();
        }

        _ = mapper.Map<DishUpdateDto, Dish>(dishUpdateDto, dishEntity);
        await dishDB.SaveChangesAsync();
        return TypedResults.NoContent();
    }
}