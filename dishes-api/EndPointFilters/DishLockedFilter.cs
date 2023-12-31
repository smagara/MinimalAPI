using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DishesAPI.EndPointFilters
{
    public class DishLockedFilter : IEndpointFilter
    {
        Guid lockedDishId;

        public DishLockedFilter(Guid lockedDishId)
        {
            this.lockedDishId = lockedDishId;
        }

        public async ValueTask<object?> InvokeAsync(
            EndpointFilterInvocationContext context,
            EndpointFilterDelegate next)
        {
            var dishId = context.GetArgument<Guid>(2);
               if (dishId == this.lockedDishId)
            {
                return TypedResults.Problem(new()
                {
                    Status = 400,
                    Title = "Cannot change this one",
                    Detail = "This dish is perfect as-is"
                });
            }
            var result = await next.Invoke(context);
            return result;
        }
    }
}