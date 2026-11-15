using RccgHopeHouse.Core.AI;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Application.Features.AiPrompts.Dtos;

/// <summary>One prompt in the list on the AI Prompts page.</summary>
public record AiPromptSummaryDto(
    string Key,
    string Name,
    string Description,
    bool IsCustomised,
    int? CurrentVersion,
    DateTime? UpdatedAt,
    string? UpdatedBy);

/// <summary>A saved version of a prompt (history list).</summary>
public record AiPromptVersionDto(
    Guid Id,
    int Version,
    string Instructions,
    string? ChangeNote,
    string? ChangedBy,
    bool IsActive,
    DateTime CreatedAt);

/// <summary>Everything the prompt editor needs.</summary>
public record AiPromptDetailDto(
    string Key,
    string Name,
    string Description,
    string InputDescription,
    IReadOnlyList<string> RequiredTerms,
    string Instructions,
    string DefaultInstructions,
    bool IsCustomised,
    int? CurrentVersion,
    IReadOnlyList<string> MissingTerms,
    IReadOnlyList<AiPromptVersionDto> History);

/// <summary>A rewritten prompt from the AI assistant (not saved yet).</summary>
public record AiPromptSuggestionDto(
    string SuggestedInstructions,
    string Summary,
    IReadOnlyList<string> MissingTerms);

public static class AiPromptMappings
{
    public static AiPromptVersionDto ToDto(this AiPromptVersion v) => new(
        v.Id,
        v.Version,
        v.Instructions,
        v.ChangeNote,
        v.ChangedBy,
        v.IsActive,
        v.CreatedAt);

    /// <summary>
    /// True when the active version differs from the built-in default
    /// (a reset saves the default as a version, which is not "customised").
    /// </summary>
    public static bool IsCustomised(AiPromptDefinition definition, AiPromptVersion? active) =>
        active is not null &&
        !string.Equals(
            Normalize(active.Instructions),
            Normalize(definition.DefaultInstructions),
            StringComparison.Ordinal);

    private static string Normalize(string text) =>
        text.Replace("\r\n", "\n").Trim();
}
