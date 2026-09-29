using MediatR;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Models;

namespace RccgHopeHouse.Application.Features.Bibles.Queries;

/// <summary>
/// Handles retrieval of the canonical Bible reference catalogue.
/// </summary>
public sealed class GetBibleReferenceDataQueryHandler
    : IRequestHandler<
        GetBibleReferenceDataQuery,
        IReadOnlyList<BibleBookReference>>
{
    private readonly IBibleReferenceService _bibleReferenceService;

    public GetBibleReferenceDataQueryHandler(
        IBibleReferenceService bibleReferenceService)
    {
        _bibleReferenceService = bibleReferenceService;
    }

    /// <summary>
    /// Retrieves all supported Bible books together with their
    /// chapter and verse-count structure.
    /// </summary>
    public Task<IReadOnlyList<BibleBookReference>> Handle(
        GetBibleReferenceDataQuery request,
        CancellationToken cancellationToken)
    {
        var books = _bibleReferenceService.GetBooks();

        return Task.FromResult(books);
    }
}