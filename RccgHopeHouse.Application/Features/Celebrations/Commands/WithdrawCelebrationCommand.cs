
using MediatR;
using RccgHopeHouse.Application.Features.Celebrations.Dtos;

namespace RccgHopeHouse.Application.Features.Celebrations.Commands;

/// <summary>
/// Represents a request to withdraw publication permission for a celebration.
/// </summary>
/// <param name="CelebrationId">The unique identifier of the celebration to withdraw.</param>
/// <remarks>
/// Withdrawal is supported after member approval, administrator approval or publication.
/// A withdrawn celebration must no longer be displayed publicly.
/// </remarks>
public sealed record WithdrawCelebrationCommand(Guid CelebrationId) : IRequest<CelebrationDto>;
