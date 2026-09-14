using MediatR;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.ContactUs.Commands;

public record SubmitContactUsCommand(
    string FirstName, string LastName, string Email, string Message,
    ContactReason Reason, string? PhoneNumber) : IRequest<Unit>;