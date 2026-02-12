using Microsoft.AspNetCore.Identity;

namespace AllThruit3.Data.Entities;

public class ApplicationUser : IdentityUser
{
    public string? ExtraField1 { get; set; }
    public string? ExtraField2 { get; set; }
    public string? ExtraField3 { get; set; }
}