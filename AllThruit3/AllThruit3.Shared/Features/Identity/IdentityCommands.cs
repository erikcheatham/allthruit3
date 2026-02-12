using AllThruit3.Shared.Common;  // For ICommand, Result, Error, Unit
using AllThruit3.Shared.Common.Handlers;
using AllThruit3.Shared.Services;  // For IIdentityClient
using FluentValidation;

namespace AllThruit3.Shared.Features.Identity;

// Registration command (DTO for serialization in API calls)
public sealed class RegisterCommand : ICommand<Unit>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    // Optional: Add Username, ConfirmPassword, etc., for UI forms
}

// Validator (client/server-side validation)
public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}

// Client-side handler (proxies to Web API via IIdentityClient)
public sealed class RegisterCommandHandler : ICommandHandler<RegisterCommand, Unit>
{
    private readonly IIdentityClient _identityClient;

    public RegisterCommandHandler(IIdentityClient identityClient) => _identityClient = identityClient;

    public async Task<Result<Unit>> Handle(RegisterCommand command, CancellationToken ct = default)
    {
        var result = await _identityClient.RegisterAsync(command, ct);
        if (result.IsFailure)
        {
            return Result.Failure<Unit>(result.Error);  // Propagate error
        }
        return Result.Success(default(Unit));
    }
}

// Login command (DTO for serialization)
public sealed class LoginCommand : ICommand<string>  // Returns token or success message
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

// Validator
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

// Client-side handler (proxies to Web API via IIdentityClient)
public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, string>
{
    private readonly IIdentityClient _identityClient;

    public LoginCommandHandler(IIdentityClient identityClient) => _identityClient = identityClient;

    public async Task<Result<string>> Handle(LoginCommand command, CancellationToken ct = default)
    {
        return await _identityClient.LoginAsync(command, ct);
    }
}