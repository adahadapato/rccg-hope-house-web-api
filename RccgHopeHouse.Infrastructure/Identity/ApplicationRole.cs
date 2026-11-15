using Microsoft.AspNetCore.Identity;

namespace RccgHopeHouse.Infrastructure.Identity;

public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; }
}