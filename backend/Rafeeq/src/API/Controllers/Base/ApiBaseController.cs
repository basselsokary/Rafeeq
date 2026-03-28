using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace API.Controllers.Base;

[ApiController]
public abstract class ApiBaseController : ControllerBase
{
    protected ActionResult<T> HandleResult<T>(Result<T> result)
    {
        if (result.Succeeded)
        {
            return result.Value switch
            {
                null => BadRequest(),
                _ => Ok(result.Value)
            };
        }

        return HandleErrorResult(result.Error);
    }

    protected ActionResult HandleResult(Result result)
    {
        if (result.Succeeded)
            return Ok();

        return HandleErrorResult(result.Error);
    }

    protected ActionResult<T> CreatedResult<T>(string actionName, object routeValues, T value)
    {
        return CreatedAtAction(actionName, routeValues, value);
    }
    
    private ActionResult HandleErrorResult(Error error)
    {
        int statusCode = GetStatusCode(error);

        var response = CreateProblem(error, statusCode);

        return StatusCode(statusCode, response);
    }

    private static int GetStatusCode(Error error)
    {
        return error.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Failure => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    private ProblemDetails CreateProblem(Error error, int statusCode)
    {
        if (error is ValidationError validationError)
        {
            var errors = validationError.Errors
                .GroupBy(e => e.Code)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.Message).ToArray()
                );

            return new ValidationProblemDetails(errors)
            {
                Title = error.Code,
                Detail = error.Message,
                Status = statusCode,
                Instance = HttpContext.Request.Path
            };
        }

        return new ProblemDetails
        {
            Title = error.Code,
            Detail = error.Message,
            Status = statusCode,
            Instance = HttpContext.Request.Path
        };
    }
}