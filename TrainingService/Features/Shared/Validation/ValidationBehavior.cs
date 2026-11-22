using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace TrainingService.Features.Shared.Validation;

public class ValidationBehavior<TRequest, TData> : IPipelineBehavior<TRequest, RequestResponse<TData>>
    where TRequest : IRequest<RequestResponse<TData>>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<RequestResponse<TData>> Handle(
        TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<RequestResponse<TData>> next)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var failures = new List<ValidationFailure>();
        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(context, cancellationToken);
            failures.AddRange(result.Errors);
        }

        if (failures.Count == 0)
            return await next();

        var errors = failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).Distinct().ToArray());

        // الـ Fail بتاعتك أنت، ونفس نوع رد الـ handler
        return RequestResponse<TData>.Fail("Validation failed.", 400, errors);
    }
}