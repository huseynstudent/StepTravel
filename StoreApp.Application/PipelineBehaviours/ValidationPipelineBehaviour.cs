using FluentValidation;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace StoreApp.Application.PipelineBehaviours;
public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior()
    {
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("az");
    }
    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> compositeValidator)
    {
        _validators = compositeValidator;
    }
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        var validationResults = _validators.Select(x => x.Validate(context)).ToList();
        var failures = validationResults.SelectMany(x => x.Errors).Where(x => x != null).ToList();
        if (failures.Any())
        {
            throw new FluentValidation.ValidationException(failures);
        }
        return next();
    }
}