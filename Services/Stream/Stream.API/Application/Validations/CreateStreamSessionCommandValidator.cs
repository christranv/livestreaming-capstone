using FluentValidation;
using Microsoft.Extensions.Logging;
using Stream.API.Application.Commands;

namespace Stream.API.Application.Validations
{
    public class CreateStreamSessionCommandValidator : AbstractValidator<CreateStreamSessionCommand>
    {
        public CreateStreamSessionCommandValidator(ILogger<CreateStreamSessionCommandValidator> logger)
        {
            RuleFor(command => command.SrsCallbackCallbackModel).NotEmpty();

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}