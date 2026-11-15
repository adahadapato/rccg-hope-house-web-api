using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.Account.Commands;
using RccgHopeHouse.Application.Features.Account.Queries;
using RccgHopeHouse.Core.Results;
using System.Security.Claims;

namespace RccgHopeHouse.Api.Endpoints.Accounts;

public static class AccountEndpoints
{
    public static RouteGroupBuilder MapAccountEndpoints(
        this RouteGroupBuilder group)
    {
        var account = group
            .MapGroup("/account")
            .WithTags("Account")
            .RequireAuthorization();

        account.MapGet(
                "/profile",
                GetProfileAsync)
            .WithName("GetAccountProfile")
            .Produces<AccountProfileResult>();

        account.MapPut(
                "/profile",
                UpdateProfileAsync)
            .WithName("UpdateAccountProfile")
            .Produces<AccountProfileResult>()
            .ProducesValidationProblem();

        account.MapPut(
                "/phone",
                UpdatePhoneNumberAsync)
            .WithName("UpdateAccountPhoneNumber")
            .Produces<AccountProfileResult>()
            .ProducesValidationProblem();

        account.MapPost(
                "/change-password",
                ChangePasswordAsync)
            .WithName("ChangeAccountPassword")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem();

        account.MapPost(
                "/profile-image",
                UploadProfileImageAsync)
            .WithName("UploadAccountProfileImage")
            .Produces<AccountProfileResult>()
            .ProducesValidationProblem()
            .DisableAntiforgery()
            .ExcludeFromDescription();

        account.MapDelete(
                "/profile-image",
                RemoveProfileImageAsync)
            .WithName("RemoveAccountProfileImage")
            .Produces<AccountProfileResult>();

        account.MapGet(
                "/two-factor",
                GetTwoFactorStatusAsync)
            .WithName("GetAccountTwoFactorStatus")
            .Produces<TwoFactorStatusResult>();

        account.MapGet(
                "/two-factor/setup",
                GetTwoFactorSetupAsync)
            .WithName("GetAccountTwoFactorSetup")
            .Produces<TwoFactorSetupResult>();

        account.MapPost(
                "/two-factor/enable",
                EnableTwoFactorAsync)
            .WithName("EnableAccountTwoFactor")
            .Produces<RecoveryCodesResponse>()
            .ProducesValidationProblem();

        account.MapPost(
                "/two-factor/disable",
                DisableTwoFactorAsync)
            .WithName("DisableAccountTwoFactor")
            .Produces(StatusCodes.Status204NoContent);

        account.MapPost(
                "/two-factor/recovery-codes",
                GenerateRecoveryCodesAsync)
            .WithName("GenerateAccountRecoveryCodes")
            .Produces<RecoveryCodesResponse>();

        account.MapDelete(
                "/",
                DeleteAccountAsync)
            .WithName("DeleteOwnAccount")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem();

        return group;
    }

    private static async Task<IResult> GetProfileAsync(
        ClaimsPrincipal principal,
        IMediator mediator,
        CancellationToken ct)
    {
        var userId =
            GetCurrentUserId(
                principal);

        var result =
            await mediator.Send(
                new GetAccountProfileQuery(
                    userId),
                ct);

        return TypedResults.Ok(
            result);
    }

    private static async Task<IResult> UpdateProfileAsync(
        [FromBody]
        UpdateAccountProfileRequest request,
        ClaimsPrincipal principal,
        IMediator mediator,
        CancellationToken ct)
    {
        var userId =
            GetCurrentUserId(
                principal);

        var result =
            await mediator.Send(
                new UpdateAccountProfileCommand(
                    userId,
                    request.FirstName,
                    request.LastName),
                ct);

        return TypedResults.Ok(
            result);
    }

    private static async Task<IResult> UpdatePhoneNumberAsync(
        [FromBody]
        UpdateAccountPhoneNumberRequest request,
        ClaimsPrincipal principal,
        IMediator mediator,
        CancellationToken ct)
    {
        var userId =
            GetCurrentUserId(
                principal);

        var result =
            await mediator.Send(
                new UpdateAccountPhoneNumberCommand(
                    userId,
                    request.PhoneNumber),
                ct);

        return TypedResults.Ok(
            result);
    }

    private static async Task<IResult> ChangePasswordAsync(
        [FromBody]
        ChangeAccountPasswordRequest request,
        ClaimsPrincipal principal,
        IMediator mediator,
        CancellationToken ct)
    {
        var userId =
            GetCurrentUserId(
                principal);

        await mediator.Send(
            new ChangeAccountPasswordCommand(
                userId,
                request.CurrentPassword,
                request.NewPassword),
            ct);

        return TypedResults.NoContent();
    }

    private static async Task<IResult> UploadProfileImageAsync(
        [FromForm]
        IFormFile file,
        ClaimsPrincipal principal,
        IMediator mediator,
        CancellationToken ct)
    {
        if (file is null ||
            file.Length == 0)
        {
            return TypedResults.BadRequest(
                "Profile image is required.");
        }

        var userId =
            GetCurrentUserId(
                principal);

        using var ms =
            new MemoryStream();

        await file.CopyToAsync(
            ms,
            ct);

        var result =
            await mediator.Send(
                new UpdateAccountProfileImageCommand(
                    userId,
                    ms.ToArray(),
                    file.ContentType),
                ct);

        return TypedResults.Ok(
            result);
    }

    private static async Task<IResult> RemoveProfileImageAsync(
        ClaimsPrincipal principal,
        IMediator mediator,
        CancellationToken ct)
    {
        var userId =
            GetCurrentUserId(
                principal);

        var result =
            await mediator.Send(
                new RemoveAccountProfileImageCommand(
                    userId),
                ct);

        return TypedResults.Ok(
            result);
    }

    private static async Task<IResult> GetTwoFactorStatusAsync(
        ClaimsPrincipal principal,
        IMediator mediator,
        CancellationToken ct)
    {
        var userId =
            GetCurrentUserId(
                principal);

        var result =
            await mediator.Send(
                new GetAccountTwoFactorStatusQuery(
                    userId),
                ct);

        return TypedResults.Ok(
            result);
    }

    private static async Task<IResult> GetTwoFactorSetupAsync(
        ClaimsPrincipal principal,
        IMediator mediator,
        CancellationToken ct)
    {
        var userId =
            GetCurrentUserId(
                principal);

        var result =
            await mediator.Send(
                new GetAccountTwoFactorSetupQuery(
                    userId),
                ct);

        return TypedResults.Ok(
            result);
    }

    private static async Task<IResult> EnableTwoFactorAsync(
        [FromBody]
        EnableTwoFactorRequest request,
        ClaimsPrincipal principal,
        IMediator mediator,
        CancellationToken ct)
    {
        var userId =
            GetCurrentUserId(
                principal);

        var recoveryCodes =
            await mediator.Send(
                new EnableAccountTwoFactorCommand(
                    userId,
                    request.VerificationCode),
                ct);

        return TypedResults.Ok(
            new RecoveryCodesResponse(
                recoveryCodes));
    }

    private static async Task<IResult> DisableTwoFactorAsync(
        ClaimsPrincipal principal,
        IMediator mediator,
        CancellationToken ct)
    {
        var userId =
            GetCurrentUserId(
                principal);

        await mediator.Send(
            new DisableAccountTwoFactorCommand(
                userId),
            ct);

        return TypedResults.NoContent();
    }

    private static async Task<IResult> GenerateRecoveryCodesAsync(
        ClaimsPrincipal principal,
        IMediator mediator,
        CancellationToken ct)
    {
        var userId =
            GetCurrentUserId(
                principal);

        var recoveryCodes =
            await mediator.Send(
                new GenerateAccountRecoveryCodesCommand(
                    userId),
                ct);

        return TypedResults.Ok(
            new RecoveryCodesResponse(
                recoveryCodes));
    }

    private static async Task<IResult> DeleteAccountAsync(
        [FromBody]
        DeleteAccountRequest request,
        ClaimsPrincipal principal,
        IMediator mediator,
        CancellationToken ct)
    {
        var userId =
            GetCurrentUserId(
                principal);

        await mediator.Send(
            new DeleteAccountCommand(
                userId,
                request.CurrentPassword),
            ct);

        return TypedResults.NoContent();
    }

    private static string GetCurrentUserId(
        ClaimsPrincipal principal)
    {
        var userId =
            principal.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(
                userId))
        {
            throw new UnauthorizedAccessException(
                "The authenticated user's identifier is missing.");
        }

        return userId;
    }
}

public sealed record UpdateAccountProfileRequest(
    string FirstName,
    string LastName);

public sealed record UpdateAccountPhoneNumberRequest(
    string? PhoneNumber);

public sealed record ChangeAccountPasswordRequest(
    string CurrentPassword,
    string NewPassword);

public sealed record EnableTwoFactorRequest(
    string VerificationCode);

public sealed record DeleteAccountRequest(
    string CurrentPassword);

public sealed record RecoveryCodesResponse(
    IReadOnlyCollection<string> RecoveryCodes);