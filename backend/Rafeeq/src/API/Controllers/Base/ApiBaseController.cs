using API.Services.Dispatchers;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace API.Controllers.Base;

[ApiController]
public abstract class ApiBaseController(IRequestDispatcher dispatcher) : ControllerBase
{
    protected readonly IRequestDispatcher Dispatcher = dispatcher;

    protected ActionResult HandleResult<T>(Result<T> result)
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
            ErrorType.Validation => StatusCodes.Status422UnprocessableEntity,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Failure => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    private ProblemDetails CreateProblem(Error error, int statusCode)
    {
        return new ProblemDetails
        {
            Title = error.Code,
            Detail = error.Description,
            Status = statusCode,
            Instance = HttpContext.Request.Path
        };
    }
}