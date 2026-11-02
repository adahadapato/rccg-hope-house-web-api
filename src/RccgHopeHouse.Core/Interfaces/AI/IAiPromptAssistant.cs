using RccgHopeHouse.Core.AI;
using RccgHopeHouse.Core.Results.AI;

namespace RccgHopeHouse.Core.Interfaces.AI;

/// <summary>
/// Uses AI to rewrite a prompt according to an admin's request,
/// keeping the answer format the code depends on.
/// </summary>
public interface IAiPromptAssistant
{
    Task<AiPromptSuggestionResult> SuggestAsync(
        AiPromptDefinition definition,
        string currentInstructions,
        string requestedChange,
        CancellationToken cancellationToken = default);
}
