using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DishesAPI.Models;
using MiniValidation;

namespace DishesAPI.EndPointFilters
{
    public class ValidateAnnotationsFilter : IEndpointFilter

    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var dishCreateDto = context.GetArgument<DishCreateDto>(3);

            if (!MiniValidator.TryValidate(dishCreateDto, out var validationErrors))
            {
                return TypedResults.ValidationProblem(validationErrors);
            }

            return await next(context);
        }
    }
}
