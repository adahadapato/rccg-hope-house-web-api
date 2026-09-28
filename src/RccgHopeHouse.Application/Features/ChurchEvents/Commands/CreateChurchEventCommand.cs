using MediatR;
using RccgHopeHouse.Application.Features.ChurchEvents.Dtos;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.ChurchEvents.Commands;

public record CreateChurchEventCommand(
    string Title,
    ServiceCategory Category,
    DateTime StartDateTime,
    DateTime? EndDateTime,
    string? Description = null,
    string? Location = null,
    string? Icon = null,
    string? Color = null,
    string? RegistrationUrl = null,
    string? RegistrationButtonText = null,
    string? ImageUrl = null,
    int DisplayOrder = 0,
    bool IsActive = true
) : IRequest<ChurchEventDto>;