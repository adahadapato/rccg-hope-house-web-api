// Core/Constants/NotificationSettings.cs
namespace RccgHopeHouse.Core.Constants;

/// <summary>
/// Strongly-typed configuration for notification settings.
/// Used to inject sender details into email services without framework coupling.
/// </summary>
public record NotificationSettings(
    string AdminContactEmail,
    string AdminContactName = "RCCG Hope House Team",
    string AdminDashboardUrl = "https://admin.rccghopehouse.org.uk"); 
