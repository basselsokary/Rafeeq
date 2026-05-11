using System.Diagnostics;
using Application.Services;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Application.Behaviours;

internal static class LoggingDecorator
{
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        ILogger<CommandHandler<TCommand, TResponse>> logger)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            return await ExecuteWithLoggingAsync(
                "command",
                typeof(TCommand).Name,
                logger,
                () => innerHandler.HandleAsync(command, cancellationToken));
        }
    }

    internal sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler,
        ILogger<CommandBaseHandler<TCommand>> logger)
        : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            return await ExecuteWithLoggingAsync(
                "command",
                typeof(TCommand).Name,
                logger,
                () => innerHandler.HandleAsync(command, cancellationToken));
        }
    }

    internal sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler,
        ILogger<QueryHandler<TQuery, TResponse>> logger)
        : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> HandleAsync(TQuery query, CancellationToken cancellationToken)
        {
            return await ExecuteWithLoggingAsync(
                "query",
                typeof(TQuery).Name,
                logger,
                () => innerHandler.HandleAsync(query, cancellationToken));
        }
    }

    private static async Task<Result> ExecuteWithLoggingAsync(
        string operationType,
        string operationName,
        ILogger logger,
        Func<Task<Result>> operation)
    {
        var stopwatch = Stopwatch.StartNew();
        LogStart(logger, operationType, operationName);

        Result result = await operation();

        stopwatch.Stop();
        LogCompletion(logger, operationType, operationName, stopwatch.ElapsedMilliseconds, result);

        return result;
    }

    private static async Task<Result<TResponse>> ExecuteWithLoggingAsync<TResponse>(
        string operationType,
        string operationName,
        ILogger logger,
        Func<Task<Result<TResponse>>> operation)
    {
        var stopwatch = Stopwatch.StartNew();
        LogStart(logger, operationType, operationName);

        Result<TResponse> result = await operation();

        stopwatch.Stop();
        LogCompletion(logger, operationType, operationName, stopwatch.ElapsedMilliseconds, result);

        return result;
    }

    private static void LogStart(ILogger logger, string operationType, string operationName)
    {
        logger.LogInformation("Processing {OperationType} {OperationName}", operationType, operationName);
    }

    private static void LogCompletion(
        ILogger logger,
        string operationType,
        string operationName,
        long elapsedMilliseconds,
        Result result)
    {
        if (result.Succeeded)
        {
            logger.LogInformation(
                "Completed {OperationType} {OperationName} in {ElapsedMs}ms",
                operationType,
                operationName,
                elapsedMilliseconds);

            return;
        }

        using (LogContext.PushProperty("Error", result.Error, true))
        {
            logger.LogError(
                "Completed {OperationType} {OperationName} with error in {ElapsedMs}ms",
                operationType,
                operationName,
                elapsedMilliseconds);
        }
    }
}
