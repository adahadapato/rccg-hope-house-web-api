using MediatR;
using RccgHopeHouse.Core.Models;

namespace RccgHopeHouse.Application.Features.Bibles.Queries;

/// <summary>
/// Query for retrieving the canonical Bible reference catalogue used
/// for scripture selection and validation.
/// </summary>
public sealed record GetBibleReferenceDataQuery
    : IRequest<IReadOnlyList<BibleBookReference>>;