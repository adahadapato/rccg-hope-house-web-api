using MediatR;
using RccgHopeHouse.Application.Features.ContactUs.Dtos;

namespace RccgHopeHouse.Application.Features.ContactUs.Queries;

/// <summary>
/// Query to retrieve a single contact request by ID for admin review.
/// </summary>
public record GetContactUsByIdQuery(Guid Id) : IRequest<ContactUsDto>;