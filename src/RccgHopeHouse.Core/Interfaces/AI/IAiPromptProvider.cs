namespace RccgHopeHouse.Core.Interfaces.AI;

/// <summary>
/// Gives AI services the prompt they should use: the admin's
/// active version when one is saved, otherwise the built-in default.
/// </summary>
public interface IAiPromptProvider
{
    /// <summary>
    /// Returns the instructions for the prompt. Never throws for
    /// database problems; falls back to the built-in default.
    /// </summary>
    Task<string> GetInstructionsAsync(
        string promptKey,
        CancellationToken cancellationToken = default);

    /// <summary>Forgets the cached prompt so the next call reads the database.</summary>
    void Invalidate(string promptKey);
}
