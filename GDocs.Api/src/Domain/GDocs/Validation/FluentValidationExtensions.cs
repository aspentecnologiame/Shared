using FluentValidation.Results;
using ICE.GDocs.Domain.GDocs.Validation;
using System;
using System.Collections.Generic;

namespace ICE.GDocs.Domain.Validation
{
    public static class FluentValidationExtensions
    {
        public static ValidationException ToValidationException(this ValidationFailure validationFailure)
            => new ValidationException(validationFailure.ErrorCode, validationFailure.ErrorMessage);

        public static ExceptionReadOnlyCollection ToValidationExceptionCollection(this IEnumerable<ValidationFailure> validationFailures)
            => new ExceptionReadOnlyCollection(validationFailures.ConvertAll(ToValidationException));
    }
}
