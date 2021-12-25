using System.Threading.Tasks;
using EventBookAPI.Contracts.v1.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EventBookAPI.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(kvp => kvp.Key, 
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage));

            var errorResponse = new ErrorResponse();

            foreach (var error in errors)
            {
                foreach (var subError in error.Value)
                {
                    var errorModel = new ErrorModel
                    {
                        FieldName = error.Key,
                        Message = subError
                    };

                    errorResponse.Errors.Add(errorModel);
                }
            }
            
            context.Result = new BadRequestObjectResult(errorResponse);
            return;
        }
        
        await next();
    }
}