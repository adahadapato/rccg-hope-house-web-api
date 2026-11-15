namespace RccgHopeHouse.Core.Results.AI;

/// <summary>
/// A rewritten prompt suggested by the AI prompt assistant.
/// </summary>
/// <param name="SuggestedInstructions">The full rewritten prompt.</param>
/// <param name="Summary">Plain-English summary of what changed.</param>
public sealed record AiPromptSuggestionResult(
    string SuggestedInstructions,
    string Summary);
