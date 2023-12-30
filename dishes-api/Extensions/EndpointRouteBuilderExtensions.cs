
using DishesAPI.EndpointHandlers;
using DishesAPI.Models;

namespace DishesAPI.Extensions
{

    public static class EndpointRouteBuilderExtensions
    {
        public static void RegisterDishesEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            // URL map grouping
            var dishesEndpoints = endpointRouteBuilder.MapGroup("/dishes");
            var dishesWithGuidEndpoints = dishesEndpoints.MapGroup("/{dishId:Guid}");

            // Get dishes
            dishesEndpoints.MapGet("", DishesHandler.GetDishesAsync).WithName("GetDishes");

            // GetDish by Id
            dishesWithGuidEndpoints.MapGet("", DishesHandler.GetDishByIdAsync).WithName("GetDish");

            // Get Dish by Name
            dishesEndpoints.MapGet("/{dishName}", DishesHandler.GetDishByNameAsync).WithName("GetDishByName");

            // Add a Dish
            dishesEndpoints.MapPost("", DishesHandler.AddDishAsync);

            // Delete a Dish
            dishesWithGuidEndpoints.MapDelete("", DishesHandler.DeleteDishAsync);

            // Update a Dish
            dishesWithGuidEndpoints.MapPut("", DishesHandler.UpdateDishAsync).WithName("UpdateDish").WithDescription("Update a Dish record.");
        }

        public static void RegisterIngredientsEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            // URL map grouping
            var ingredientsWithGuidEndpoints = endpointRouteBuilder.MapGroup("/dishes/{dishId:guid}/ingredients");

            // Get Ingredients for a Dish Id
            ingredientsWithGuidEndpoints.MapGet("", IngredientsHandlers.GetIngredientsAsync);

        }
    }
}