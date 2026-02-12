namespace AllThruit3.Web.Components.Account;

/// <summary>
/// Model for capturing a passkey credential JSON and an optional error message from the client.
/// </summary>
public class PasskeyInputModel
{
    /// <summary>
    /// The serialized credential JSON received from the client.
    /// </summary>
    public string? CredentialJson { get; set; }

    /// <summary>
    /// Error message produced during passkey processing, if any.
    /// </summary>
    public string? Error { get; set; }
}
