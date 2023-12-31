
using DishesAPI.EndPointFilters;
using DishesAPI.EndpointHandlers;

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
            dishesEndpoints.MapPost("", DishesHandler.AddDishAsync)
                .AddEndpointFilter(new ValidateAnnotationsFilter())
                .WithName("AddDish");

            // Delete a Dish
            dishesWithGuidEndpoints.MapDelete("", DishesHandler.DeleteDishAsync)
                .AddEndpointFilter(new DishLockedFilter(new Guid("fd630a57-2352-4731-b25c-db9cc7601b16")))
                .AddEndpointFilter(new DishLockedFilter(new Guid("b512d7cf-b331-4b54-8dae-d1228d128e8d")))
                .AddEndpointFilter<LogResponseNotFoundFilter>()
                .WithName("DeleteDish");

            // Update a Dish
            dishesWithGuidEndpoints.MapPut("", DishesHandler.UpdateDishAsync)
                .AddEndpointFilter(new DishLockedFilter(new Guid("fd630a57-2352-4731-b25c-db9cc7601b16")))
                .AddEndpointFilter(new DishLockedFilter(new Guid("b512d7cf-b331-4b54-8dae-d1228d128e8d")))
                .AddEndpointFilter(new ValidateAnnotationsFilter())
                .WithName("UpdateDish")
                .WithDescription("Update a Dish record.");
        }

        public static void RegisterIngredientsEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            // URL map grouping
            var ingredientsWithGuidEndpoints = endpointRouteBuilder.MapGroup("/dishes/{dishId:guid}/ingredients");

            // Get Ingredients for a Dish Id
            ingredientsWithGuidEndpoints.MapGet("", IngredientsHandlers.GetIngredientsAsync);

            // Post ingredient
            ingredientsWithGuidEndpoints.MapPost("", () => { throw new NotImplementedException(); });


        }
    }
}