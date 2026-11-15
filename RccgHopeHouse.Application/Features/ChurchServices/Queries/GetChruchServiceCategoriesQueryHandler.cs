using MediatR;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.ChurchServices.Queries;

/// <summary>
/// Handles requests for all available church service categories.
/// </summary>
/// <remarks>
/// Categories are obtained directly from the ServiceCategory enum.
/// This ensures that every defined category is available even when
/// no existing or active church service currently uses that category.
/// </remarks>
public sealed class GetChruchServiceCategoriesQueryHandler : IRequestHandler<GetChruchServiceCategoriesQuery, IReadOnlyList<string>>
{
    /// <summary>
    /// Returns all church service categories defined by the backend.
    /// </summary>
    /// <param name="request">
    /// The query requesting the available church service categories.
    /// </param>
    /// <param name="cancellationToken">
    /// Token used to observe cancellation requests.
    /// </param>
    /// <returns>
    /// A read-only list containing all ServiceCategory enum names.
    /// </returns>
    public Task<IReadOnlyList<string>> Handle(
        GetChruchServiceCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<string> categories =
            Enum.GetValues<ServiceCategory>()
                .Select(category => category.ToString())
                .ToList();

        return Task.FromResult(categories);
    }
}