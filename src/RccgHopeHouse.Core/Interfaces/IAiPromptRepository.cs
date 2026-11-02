using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Persistence for saved AI prompt versions.
/// </summary>
public interface IAiPromptRepository
{
    /// <summary>The version the AI currently uses, or null (built-in default).</summary>
    Task<AiPromptVersion?> GetActiveAsync(
        string promptKey,
        CancellationToken ct = default);

    /// <summary>Active versions for all prompts that have one.</summary>
    Task<IReadOnlyList<AiPromptVersion>> GetAllActiveAsync(
        CancellationToken ct = default);

    /// <summary>Saved versions of one prompt, newest first.</summary>
    Task<IReadOnlyList<AiPromptVersion>> GetHistoryAsync(
        string promptKey,
        int take,
        CancellationToken ct = default);

    Task<AiPromptVersion?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default);

    /// <summary>Highest version number saved for the prompt (0 when none).</summary>
    Task<int> GetLatestVersionNumberAsync(
        string promptKey,
        CancellationToken ct = default);

    /// <summary>
    /// Marks every version of the prompt as inactive (tracked;
    /// saved by <see cref="SaveChangesAsync"/>).
    /// </summary>
    Task DeactivateAllAsync(
        string promptKey,
        CancellationToken ct = default);

    Task AddAsync(
        AiPromptVersion version,
        CancellationToken ct = default);

    Task<int> SaveChangesAsync(
        CancellationToken ct = default);
}
