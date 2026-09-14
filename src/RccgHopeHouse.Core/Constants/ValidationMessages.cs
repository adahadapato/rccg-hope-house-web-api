namespace RccgHopeHouse.Core.Constants;

/// <summary>
/// Centralized validation error messages for consistency across the app.
/// Use in FluentValidation rules or domain exceptions.
/// </summary>
public static class ValidationMessages
{
    public const string Required = "{PropertyName} is required.";
    public const string MaxLength = "{PropertyName} cannot exceed {MaxLength} characters.";
    public const string InvalidEmail = "Invalid email address format.";
    public const string InvalidUrl = "Invalid URL format.";
    public const string InvalidYouTubeUrl = "Please enter a valid YouTube URL (youtube.com or youtu.be).";
    public const string FileTooLarge = "File size cannot exceed {MaxSize}MB.";
    public const string InvalidImageFormat = "Only JPEG, PNG, and WebP images are allowed.";
}