using MediatR;
using RccgHopeHouse.Application.Features.ChurchInfo.Dtos;

namespace RccgHopeHouse.Application.Features.ChurchInfo.Commands;

public record UpdateChurchAddressCommand(
    string AddressLine1,
    string City,
    string Country,
    string? AddressLine2,
    string? PostCode) : IRequest<ChurchInfoDto>;