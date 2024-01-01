
using DishesAPI.EndPointFilters;
using DishesAPI.EndpointHandlers;
using DishesAPI.Models;

namespace DishesAPI.Extensions
{

    public static class EndpointRouteBuilderExtensions
    {
        public static void RegisterDishesEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            // URL map grouping
            var dishesEndpoints = endpointRouteBuilder.MapGroup("/dishes")
                .WithOpenApi()
                .RequireAuthorization();
            var dishesWithGuidEndpoints = dishesEndpoints.MapGroup("/{dishId:Guid}")
                .RequireAuthorization();

            // Get dishes
            dishesEndpoints.MapGet("", DishesHandler.GetDishesAsync)
                .AllowAnonymous()
                .WithDescription("Fetch all of the Dishes in the database, optionally filtered by Dish name into JSON format")
                .WithSummary("Fetch Dishes from the database")
                .WithName("GetDishes");

            // GetDish by Id
            dishesWithGuidEndpoints.MapGet("", DishesHandler.GetDishByIdAsync)
                .WithSummary("Fetch a dish by Id")
                .WithDescription("Fetch a single Dish by its Guid Id.  Requires token access.")
                .WithName("GetDish");

            // Get Dish by Name
            dishesEndpoints.MapGet("/{dishName}", DishesHandler.GetDishByNameAsync)
                .AllowAnonymous()
                .WithDescription("Fetch a single Dish by its Name.  Requires auth token")
                .WithSummary("Fetch a single Dish by its Name, case-sensitive")
                .WithOpenApi( o => { o.Deprecated = true; return o; } )
                .WithName("GetDishByName");
    

            // Add a Dish
            dishesEndpoints.MapPost("", DishesHandler.AddDishAsync)
                .AddEndpointFilter(new ValidateAnnotationsFilter())
                .WithSummary("Add a new Dish")
                .WithDescription("Add a Dish to the collection, generating a new Id.  Requires Admin authorization")
                .ProducesValidationProblem(400)
                .Accepts<DishCreateDto>("application/json")
                .WithName("AddDish");

            // Delete a Dish
            dishesWithGuidEndpoints.MapDelete("", DishesHandler.DeleteDishAsync)
                .AddEndpointFilter(new DishLockedFilter(new Guid("fd630a57-2352-4731-b25c-db9cc7601b16")))
                .AddEndpointFilter(new DishLockedFilter(new Guid("b512d7cf-b331-4b54-8dae-d1228d128e8d")))
                .AddEndpointFilter<LogResponseNotFoundFilter>()
                .RequireAuthorization("RequireAdmin")
                .WithSummary("Delete a Dish by its Guid ID.  Requires Admin authorization")
                .WithDescription("Delete a Dish by Id")
                .WithName("DeleteDish");

            // Update a Dish
            dishesWithGuidEndpoints.MapPut("", DishesHandler.UpdateDishAsync)
                .AddEndpointFilter(new DishLockedFilter(new Guid("fd630a57-2352-4731-b25c-db9cc7601b16")))
                .AddEndpointFilter(new DishLockedFilter(new Guid("b512d7cf-b331-4b54-8dae-d1228d128e8d")))
                .AddEndpointFilter(new ValidateAnnotationsFilter())
                .WithName("UpdateDish")
                .WithSummary("Update a single Dish")
                .ProducesValidationProblem(400)
                .WithDescription("Update a Dish record.  Requires Admin token access");
        }

        public static void RegisterIngredientsEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            // URL map grouping
            var ingredientsWithGuidEndpoints = endpointRouteBuilder.MapGroup("/dishes/{dishId:guid}/ingredients")
                .RequireAuthorization();

            // Get Ingredients for a Dish Id
            ingredientsWithGuidEndpoints.MapGet("", IngredientsHandlers.GetIngredientsAsync);

            // Post ingredient
            ingredientsWithGuidEndpoints.MapPost("", () => { throw new NotImplementedException(); });


        }
    }
}