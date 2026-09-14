using MediatR;
using RccgHopeHouse.Application.Features.ChurchServices.Dtos;

namespace RccgHopeHouse.Application.Features.ChurchServices.Queries;

/// <summary>
/// Query to retrieve a single service by ID for admin editing or detailed public view.
/// </summary>
public record GetChurchServiceByIdQuery(Guid Id) : IRequest<ChurchServiceDto>;