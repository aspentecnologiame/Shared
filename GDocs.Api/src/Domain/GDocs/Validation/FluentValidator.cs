using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Validation
{
    public abstract class FluentValidator<TEntity>
            : AbstractValidator<TEntity>, INullEntityBuilderOptions
            where TEntity : class
    {
        private FluentValidation.Results.ValidationFailure _nullEntityValidationFailure;

        protected FluentValidator()
        {
            ConfigureNulLEntity();
        }

        public virtual INullEntityBuilderOptions NullEntity()
        {
            var newFailure = new FluentValidation.Results.ValidationFailure(typeof(TEntity).Name, $"{typeof(TEntity).Name} não pode ser null")
            {
                Severity = Severity.Error
            };

            _nullEntityValidationFailure = newFailure;

            return this;
        }

        INullEntityBuilderOptions INullEntityBuilderOptions.OverridePropertyName(string propertyName)
        {
            var newFailure = new FluentValidation.Results.ValidationFailure(propertyName, _nullEntityValidationFailure.ErrorMessage)
            {
                Severity = _nullEntityValidationFailure.Severity
            };

            _nullEntityValidationFailure = newFailure;

            return this;
        }

        INullEntityBuilderOptions INullEntityBuilderOptions.WithMessage(string errorMessage)
        {
            var newFailure = new FluentValidation.Results.ValidationFailure(_nullEntityValidationFailure.PropertyName, errorMessage)
            {
                Severity = _nullEntityValidationFailure.Severity
            };

            _nullEntityValidationFailure = newFailure;

            return this;
        }

        INullEntityBuilderOptions INullEntityBuilderOptions.WithSeverity(Severity severity)
        {
            _nullEntityValidationFailure.Severity = severity;

            return this;
        }

        INullEntityBuilderOptions INullEntityBuilderOptions.WithErrorCode(string errorCode)
        {
            _nullEntityValidationFailure.ErrorCode = errorCode;

            return this;
        }

        private FluentValidation.Results.ValidationResult ValidateEntityIsNull(ValidationContext<TEntity> context)
        {
            return (context.InstanceToValidate == null)
                ? new FluentValidation.Results.ValidationResult(new FluentValidation.Results.ValidationFailure[] { _nullEntityValidationFailure })
                : new FluentValidation.Results.ValidationResult();
        }

        public override FluentValidation.Results.ValidationResult Validate(ValidationContext<TEntity> context)
        {
            var validationResult = ValidateEntityIsNull(context);

            if (validationResult.IsValid)
                validationResult = base.Validate(context);

            return validationResult;
        }

        public new FluentValidation.Results.ValidationResult Validate(TEntity entity)
        {
            var validationResult = base.Validate(entity);

            return new FluentValidation.Results.ValidationResult(validationResult.Errors);
        }

        public override async Task<FluentValidation.Results.ValidationResult> ValidateAsync(ValidationContext<TEntity> context, CancellationToken cancellation = default)
        {
            var validationResult = ValidateEntityIsNull(context);

            if (validationResult.IsValid)
                validationResult = await base.ValidateAsync(context, cancellation);

            return validationResult;
        }

        private void ConfigureNulLEntity()
        {
            NullEntity();
        }
    }
}
