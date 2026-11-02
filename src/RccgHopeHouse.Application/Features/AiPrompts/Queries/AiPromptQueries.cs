using MediatR;
using RccgHopeHouse.Application.Features.AiPrompts.Dtos;
using RccgHopeHouse.Core.AI;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.AiPrompts.Queries;

/// <summary>All editable AI prompts.</summary>
public record GetAiPromptsQuery : IRequest<IReadOnlyList<AiPromptSummaryDto>>;

public class GetAiPromptsQueryHandler
    : IRequestHandler<GetAiPromptsQuery, IReadOnlyList<AiPromptSummaryDto>>
{
    private readonly IAiPromptRepository _repository;

    public GetAiPromptsQueryHandler(IAiPromptRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<AiPromptSummaryDto>> Handle(
        GetAiPromptsQuery request,
        CancellationToken ct)
    {
        // Newest active version per prompt (normally there is only one).
        var active = (await _repository.GetAllActiveAsync(ct))
            .GroupBy(v => v.PromptKey, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(v => v.Version).First(),
                StringComparer.OrdinalIgnoreCase);

        return AiPromptCatalog.All
            .Select(definition =>
            {
                active.TryGetValue(definition.Key, out var version);

                return new AiPromptSummaryDto(
                    definition.Key,
                    definition.Name,
                    definition.Description,
                    AiPromptMappings.IsCustomised(definition, version),
                    version?.Version,
                    version?.CreatedAt,
                    version?.ChangedBy);
            })
            .ToList();
    }
}

/// <summary>One prompt with its text, default and history.</summary>
public record GetAiPromptQuery(string Key) : IRequest<AiPromptDetailDto>;

public class GetAiPromptQueryHandler
    : IRequestHandler<GetAiPromptQuery, AiPromptDetailDto>
{
    private const int HistoryLimit = 50;

    private readonly IAiPromptRepository _repository;

    public GetAiPromptQueryHandler(IAiPromptRepository repository)
    {
        _repository = repository;
    }

    public async Task<AiPromptDetailDto> Handle(
        GetAiPromptQuery request,
        CancellationToken ct)
    {
        var definition =
            AiPromptCatalog.Find(request.Key)
            ?? throw new NotFoundException("AI prompt", request.Key);

        var active = await _repository.GetActiveAsync(definition.Key, ct);
        var history = await _repository.GetHistoryAsync(definition.Key, HistoryLimit, ct);

        var instructions = active?.Instructions ?? definition.DefaultInstructions;

        return new AiPromptDetailDto(
            definition.Key,
            definition.Name,
            definition.Description,
            definition.InputDescription,
            definition.RequiredTerms,
            instructions,
            definition.DefaultInstructions,
            AiPromptMappings.IsCustomised(definition, active),
            active?.Version,
            definition.FindMissingTerms(instructions),
            history.Select(v => v.ToDto()).ToList());
    }
}
