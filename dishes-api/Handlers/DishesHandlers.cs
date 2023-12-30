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
        ClaimsPrincipal claim,
        string? name)
    {
        Console.WriteLine($"User authenticated: {claim.Identity?.IsAuthenticated} ");

        return TypedResults.Ok(mapper.Map<IEnumerable<DishDto>>(await dishDB.Dishes
            .Where(d => name == null | (name != null && d.Name.Contains(name)))
            .ToListAsync()));
    }

    public static async Task<Results<NotFound, Ok<DishDto>>>
    GetDishByIdAsync(DishesDbContext dishDB, Guid dishId, IMapper mapper)
    {
        Dish? dishEntity = await dishDB.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
        if (dishEntity == null)
            return TypedResults.NotFound();
        else
            return TypedResults.Ok(mapper.Map<DishDto>(dishEntity));
    }

    public static async Task<Results<NotFound, Ok<DishDto>>>
    GetDishByNameAsync(DishesDbContext dishDB, IMapper mapper, string dishName)
    {
        return TypedResults.Ok(mapper.Map<DishDto>(await dishDB.Dishes.FirstOrDefaultAsync(d => d.Name == dishName)));
    }

    public static async Task<CreatedAtRoute<DishDto>>

    AddDishAsync(DishesDbContext dishDB,
        IMapper mapper,
        // HttpContext http,
        // LinkGenerator linker,
        DishCreateDto dishCreateDto)
    {
        Dish? dishEntity = mapper.Map<Dish>(dishCreateDto);
        dishDB.Add(dishEntity);
        await dishDB.SaveChangesAsync();

        var dishReturn = mapper.Map<DishDto>(dishEntity);
        // string? returnLink = linker.GetUriByName(http, "GetDish", new { dishId = dishReturn.Id });

        return TypedResults.CreatedAtRoute(dishReturn, "GetDish", new { dishId = dishReturn.Id });
    }

    public static async Task<Results<NotFound, NoContent>>
    DeleteDishAsync(
        DishesDbContext dishesDB,
        Guid dishId)
    {
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