using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DishesAPI.EndPointFilters
{
    public class LogResponseNotFoundFilter : IEndpointFilter
    {
        private readonly ILogger<LogResponseNotFoundFilter> _logger;

        public LogResponseNotFoundFilter(ILogger<LogResponseNotFoundFilter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var result = await next(context);
            var actualResult = (result is INestedHttpResult) ? ((INestedHttpResult)result)?.Result : (IResult?)result;

            if ((actualResult as IStatusCodeHttpResult)?.StatusCode == (int)System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation($"Resource {context.HttpContext.Request.Path} was not found.");
            }

            return result;
        }
    }
}