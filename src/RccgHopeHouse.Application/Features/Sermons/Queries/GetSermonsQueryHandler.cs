using MediatR;
using RccgHopeHouse.Application.Features.Sermons.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Sermons.Queries;

/// <summary>
/// Handler for fetching filtered, paginated sermon records.
/// Delegates data access and SQL-level filtering to the repository for optimal performance.
/// </summary>
public class GetSermonsQueryHandler : IRequestHandler<GetSermonsQuery, IReadOnlyList<SermonDto>>
{
    private readonly ISermonRepository _repository;

    public GetSermonsQueryHandler(ISermonRepository repository) => _repository = repository;

    /// <summary>
    /// Executes the query, applies filters at the database level, and maps results to DTOs.
    /// </summary>
    public async Task<IReadOnlyList<SermonDto>> Handle(
        GetSermonsQuery request,
        CancellationToken cancellationToken)
    {
        var sermons = await _repository.GetPagedAsync(
            skip: request.Skip,
            take: request.Take,
            search: request.Search,
            fromDate: request.FromDate,
            toDate: request.ToDate,
            ct: cancellationToken);

        return sermons.Select(s => new SermonDto(
            s.Id,
            s.Title,
            s.Speaker,
            s.ServiceDate,
            s.VideoUrl,
            s.Description,
            s.IsPublished,
            s.CreatedAt)).ToList();
    }
}