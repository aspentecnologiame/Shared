namespace ICE.GDocs.Domain.Validation
{
    public interface INullEntityBuilderOptions
    {
        INullEntityBuilderOptions OverridePropertyName(string propertyName);

        INullEntityBuilderOptions WithMessage(string errorMessage);

        INullEntityBuilderOptions WithSeverity(FluentValidation.Severity severity);

        INullEntityBuilderOptions WithErrorCode(string errorCode);
    }
}
