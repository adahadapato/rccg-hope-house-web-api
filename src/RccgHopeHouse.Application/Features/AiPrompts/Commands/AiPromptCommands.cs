using MediatR;
using RccgHopeHouse.Application.Features.AiPrompts.Dtos;
using RccgHopeHouse.Application.Features.AiPrompts.Queries;
using RccgHopeHouse.Core.AI;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Interfaces.AI;

namespace RccgHopeHouse.Application.Features.AiPrompts.Commands;

/// <summary>
/// Shared steps for saving a new active version of a prompt.
/// </summary>
public sealed class AiPromptVersionWriter
{
    private readonly IAiPromptRepository _repository;
    private readonly IAiPromptProvider _provider;
    private readonly ICurrentUserService _currentUser;

    public AiPromptVersionWriter(
        IAiPromptRepository repository,
        IAiPromptProvider provider,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _provider = provider;
        _currentUser = currentUser;
    }

    public async Task SaveAsync(
        AiPromptDefinition definition,
        string instructions,
        string? changeNote,
        CancellationToken ct)
    {
        var nextVersion =
            await _repository.GetLatestVersionNumberAsync(definition.Key, ct) + 1;

        var version = AiPromptVersion.Create(
            definition.Key,
            nextVersion,
            instructions,
            changeNote,
            _currentUser.UserName);

        await _repository.DeactivateAllAsync(definition.Key, ct);
        await _repository.AddAsync(version, ct);
        await _repository.SaveChangesAsync(ct);

        // The AI uses the new version straight away.
        _provider.Invalidate(definition.Key);
    }
}

/// <summary>Saves the admin's edited prompt as the new active version.</summary>
public record SaveAiPromptCommand(
    string Key,
    string Instructions,
    string? ChangeNote)
    : IRequest<AiPromptDetailDto>;

public class SaveAiPromptCommandHandler
    : IRequestHandler<SaveAiPromptCommand, AiPromptDetailDto>
{
    private readonly AiPromptVersionWriter _writer;
    private readonly IMediator _mediator;

    public SaveAiPromptCommandHandler(
        IAiPromptRepository repository,
        IAiPromptProvider provider,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _writer = new AiPromptVersionWriter(repository, provider, currentUser);
        _mediator = mediator;
    }

    public async Task<AiPromptDetailDto> Handle(
        SaveAiPromptCommand request,
        CancellationToken ct)
    {
        var definition =
            AiPromptCatalog.Find(request.Key)
            ?? throw new NotFoundException("AI prompt", request.Key);

        await _writer.SaveAsync(definition, request.Instructions, request.ChangeNote, ct);

        return await _mediator.Send(new GetAiPromptQuery(definition.Key), ct);
    }
}

/// <summary>Makes an earlier version active again (saved as a new version).</summary>
public record RestoreAiPromptVersionCommand(
    string Key,
    Guid VersionId)
    : IRequest<AiPromptDetailDto>;

public class RestoreAiPromptVersionCommandHandler
    : IRequestHandler<RestoreAiPromptVersionCommand, AiPromptDetailDto>
{
    private readonly IAiPromptRepository _repository;
    private readonly AiPromptVersionWriter _writer;
    private readonly IMediator _mediator;

    public RestoreAiPromptVersionCommandHandler(
        IAiPromptRepository repository,
        IAiPromptProvider provider,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _repository = repository;
        _writer = new AiPromptVersionWriter(repository, provider, currentUser);
        _mediator = mediator;
    }

    public async Task<AiPromptDetailDto> Handle(
        RestoreAiPromptVersionCommand request,
        CancellationToken ct)
    {
        var definition =
            AiPromptCatalog.Find(request.Key)
            ?? throw new NotFoundException("AI prompt", request.Key);

        var source = await _repository.GetByIdAsync(request.VersionId, ct);

        if (source is null ||
            !string.Equals(source.PromptKey, definition.Key, StringComparison.OrdinalIgnoreCase))
        {
            throw new NotFoundException("AI prompt version", request.VersionId);
        }

        await _writer.SaveAsync(
            definition,
            source.Instructions,
            $"Restored version {source.Version}.",
            ct);

        return await _mediator.Send(new GetAiPromptQuery(definition.Key), ct);
    }
}

/// <summary>Goes back to the built-in default (kept in history).</summary>
public record ResetAiPromptCommand(string Key) : IRequest<AiPromptDetailDto>;

public class ResetAiPromptCommandHandler
    : IRequestHandler<ResetAiPromptCommand, AiPromptDetailDto>
{
    private readonly AiPromptVersionWriter _writer;
    private readonly IMediator _mediator;

    public ResetAiPromptCommandHandler(
        IAiPromptRepository repository,
        IAiPromptProvider provider,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _writer = new AiPromptVersionWriter(repository, provider, currentUser);
        _mediator = mediator;
    }

    public async Task<AiPromptDetailDto> Handle(
        ResetAiPromptCommand request,
        CancellationToken ct)
    {
        var definition =
            AiPromptCatalog.Find(request.Key)
            ?? throw new NotFoundException("AI prompt", request.Key);

        await _writer.SaveAsync(
            definition,
            definition.DefaultInstructions,
            "Reset to the built-in default.",
            ct);

        return await _mediator.Send(new GetAiPromptQuery(definition.Key), ct);
    }
}

/// <summary>
/// Asks the AI assistant to rewrite a prompt. Nothing is saved;
/// the admin reviews the suggestion and decides.
/// </summary>
public record SuggestAiPromptCommand(
    string Key,
    string CurrentInstructions,
    string RequestedChange)
    : IRequest<AiPromptSuggestionDto>;

public class SuggestAiPromptCommandHandler
    : IRequestHandler<SuggestAiPromptCommand, AiPromptSuggestionDto>
{
    private readonly IAiPromptAssistant _assistant;

    public SuggestAiPromptCommandHandler(IAiPromptAssistant assistant)
    {
        _assistant = assistant;
    }

    public async Task<AiPromptSuggestionDto> Handle(
        SuggestAiPromptCommand request,
        CancellationToken ct)
    {
        var definition =
            AiPromptCatalog.Find(request.Key)
            ?? throw new NotFoundException("AI prompt", request.Key);

        var current = string.IsNullOrWhiteSpace(request.CurrentInstructions)
            ? definition.DefaultInstructions
            : request.CurrentInstructions;

        var result = await _assistant.SuggestAsync(
            definition,
            current,
            request.RequestedChange,
            ct);

        return new AiPromptSuggestionDto(
            result.SuggestedInstructions,
            result.Summary,
            definition.FindMissingTerms(result.SuggestedInstructions));
    }
}
