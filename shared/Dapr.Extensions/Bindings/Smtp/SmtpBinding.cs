using FluentValidation;

namespace DaprDemo.Dapr.Extension.Bindings.Smtp;

public class SmtpBinding : BindingBase
{
	public SmtpBinding(string bindingName)
		: base(bindingName)
	{
	}

	public override string Operation => "create";

	public SmtpOptions Options { get; set; }
}

/// <summary>
/// Validator for <see cref="SmtpBinding"/>.
/// </summary>
public class SmtpBindingValidator : AbstractValidator<SmtpBinding>
{
	public SmtpBindingValidator(IValidator<SmtpOptions> optionsValidator)
	{
		

		RuleFor(p => p.Options)
			.SetValidator(optionsValidator);
	}
}