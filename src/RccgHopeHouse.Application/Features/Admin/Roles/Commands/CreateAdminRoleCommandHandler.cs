using MediatR;
using RccgHopeHouse.Core.Interfaces.Admin;

namespace RccgHopeHouse.Application.Features.Admin.Roles.Commands;

/// <summary>
/// Handles <see cref="CreateAdminRoleCommand"/>.
/// </summary>
public sealed class CreateAdminRoleCommandHandler
    : IRequestHandler<CreateAdminRoleCommand, string>
{
    private readonly IAdminRepository _adminRepository;

    public CreateAdminRoleCommandHandler(
        IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task<string> Handle(
        CreateAdminRoleCommand request,
        CancellationToken cancellationToken)
    {
        var roleName = request.Name.Trim();

        var existingRole =
            await _adminRepository.GetRoleByNameAsync(
                roleName,
                cancellationToken);

        if (existingRole is not null)
        {
            throw new InvalidOperationException(
                $"A role named '{roleName}' already exists.");
        }

        return await _adminRepository.CreateRoleAsync(
            roleName,
            request.Description,
            cancellationToken);
    }
}