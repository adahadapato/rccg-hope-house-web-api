using MediatR;
using RccgHopeHouse.Application.Features.ContactUs.Dtos;

namespace RccgHopeHouse.Application.Features.ContactUs.Queries;

public record GetContactUsQuery(bool UnreadOnly = false, int Skip = 0, int Take = 20)
    : IRequest<IReadOnlyList<ContactUsDto>>;
