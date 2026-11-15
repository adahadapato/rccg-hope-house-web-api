using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RccgHopeHouse.Core.AI;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Interfaces.AI;

namespace RccgHopeHouse.Infrastructure.Services.AI;

/// <summary>
/// Supplies AI services with the admin's active prompt, or the
/// built-in default from <see cref="AiPromptCatalog"/>.
///
/// Prompts are cached for a few minutes so the database is not
/// read on every AI call; saving a prompt clears its cache entry.
/// </summary>
public sealed class AiPromptProvider : IAiPromptProvider
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    private readonly IAiPromptRepository _repository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<AiPromptProvider> _logger;

    public AiPromptProvider(
        IAiPromptRepository repository,
        IMemoryCache cache,
        ILogger<AiPromptProvider> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> GetInstructionsAsync(
        string promptKey,
        CancellationToken cancellationToken = default)
    {
        var definition =
            AiPromptCatalog.Find(promptKey)
            ?? throw new ArgumentException(
                $"Unknown AI prompt '{promptKey}'. Add it to AiPromptCatalog.",
                nameof(promptKey));

        var cacheKey = CacheKey(definition.Key);

        if (_cache.TryGetValue(cacheKey, out string? cached) &&
            !string.IsNullOrWhiteSpace(cached))
        {
            return cached;
        }

        try
        {
            var active = await _repository.GetActiveAsync(
                definition.Key,
                cancellationToken);

            var custom = active?.Instructions;

            var instructions = string.IsNullOrWhiteSpace(custom)
                ? definition.DefaultInstructions
                : custom;

            _cache.Set(cacheKey, instructions, CacheDuration);

            return instructions;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            // Never stop an AI feature because the prompt could not be read.
            _logger.LogWarning(
                ex,
                "Could not read AI prompt {PromptKey}; using the built-in default.",
                definition.Key);

            return definition.DefaultInstructions;
        }
    }

    /// <inheritdoc />
    public void Invalidate(string promptKey)
    {
        var definition = AiPromptCatalog.Find(promptKey);

        if (definition is not null)
        {
            _cache.Remove(CacheKey(definition.Key));
        }
    }

    private static string CacheKey(string promptKey) => $"ai-prompt:{promptKey}";
}
