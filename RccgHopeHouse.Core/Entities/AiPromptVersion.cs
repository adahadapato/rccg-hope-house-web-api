using RccgHopeHouse.Core.Exceptions;

namespace RccgHopeHouse.Core.Entities;

/// <summary>
/// One saved version of an editable AI prompt.
///
/// Every save, restore or reset adds a new row, so the full history
/// is kept. Exactly one version per prompt is active; when none
/// exists, the built-in default from AiPromptCatalog is used.
/// </summary>
public class AiPromptVersion : BaseEntity
{
    public const int PromptKeyMaxLength = 100;
    public const int InstructionsMaxLength = 20000;
    public const int ChangeNoteMaxLength = 300;
    public const int ChangedByMaxLength = 150;

    /// <summary>Which prompt this is, e.g. "smart-seo".</summary>
    public string PromptKey { get; private set; } = string.Empty;

    /// <summary>1, 2, 3… per prompt.</summary>
    public int Version { get; private set; }

    /// <summary>The prompt text sent to the AI as its instructions.</summary>
    public string Instructions { get; private set; } = string.Empty;

    /// <summary>Optional note on what changed and why.</summary>
    public string? ChangeNote { get; private set; }

    /// <summary>Admin who saved this version.</summary>
    public string? ChangedBy { get; private set; }

    /// <summary>True for the version the AI currently uses.</summary>
    public bool IsActive { get; private set; }

    private AiPromptVersion()
    {
    }

    public static AiPromptVersion Create(
        string promptKey,
        int version,
        string instructions,
        string? changeNote,
        string? changedBy)
    {
        if (string.IsNullOrWhiteSpace(promptKey))
        {
            throw new DomainException("A prompt key is required.");
        }

        if (version < 1)
        {
            throw new DomainException("Prompt version must be 1 or greater.");
        }

        if (string.IsNullOrWhiteSpace(instructions))
        {
            throw new DomainException("The prompt cannot be empty.");
        }

        var text = instructions.Replace("\r\n", "\n").Trim();

        if (text.Length > InstructionsMaxLength)
        {
            throw new DomainException(
                $"The prompt cannot be longer than {InstructionsMaxLength} characters.");
        }

        return new AiPromptVersion
        {
            PromptKey = promptKey.Trim(),
            Version = version,
            Instructions = text,
            ChangeNote = Truncate(changeNote, ChangeNoteMaxLength),
            ChangedBy = Truncate(changedBy, ChangedByMaxLength),
            IsActive = true
        };
    }

    /// <summary>Stops this version being used (a newer one replaced it).</summary>
    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        MarkAsUpdated();
    }

    private static string? Truncate(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var trimmed = value.Trim();

        return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength];
    }
}
