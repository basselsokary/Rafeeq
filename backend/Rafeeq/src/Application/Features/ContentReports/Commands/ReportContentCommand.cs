using FluentValidation;

namespace Application.Features.Sites.Commands;

public record ReportContentCommand() : ICommand;

internal class ReportContentCommandHandler : ICommandHandler<ReportContentCommand>
{
    public Task<Result> HandleAsync(ReportContentCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class ReportContentCommandValidator : AbstractValidator<ReportContentCommand>
{
    public ReportContentCommandValidator()
    {
        throw new NotImplementedException();
    }
}
