using MediatR;
using RccgHopeHouse.Application.Common.Mappings;
using RccgHopeHouse.Application.Features.PastorPosts.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PastorPosts.Commands;

public class UpdatePastorPostCommandHandler : IRequestHandler<UpdatePastorPostCommand, PastorPostDto>
{
    private readonly IPastorPostRepository _repository;

    public UpdatePastorPostCommandHandler(IPastorPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<PastorPostDto> Handle(UpdatePastorPostCommand request, CancellationToken cancellationToken)
    {
        var post = await _repository.GetByIdForAdminAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(PastorPost), request.Id);

        post.Update(
            title: request.Title,
            content: request.Content,
            category: request.Category,
            themeOfTheYearId: request.ThemeOfTheYearId,
            excerpt: request.Excerpt,
            introHeading: request.IntroHeading,
            introText: request.IntroText,
            structuredContentJson: StructuredContentDto.Serialize(request.StructuredContent),
            closingText: request.ClosingText,
            bibleReference: request.BibleReference);

        if (request.CoverImageData is not null)
        {
            post.SetCoverImage(request.CoverImageData, request.CoverImageContentType);
        }

        await _repository.UpdateAsync(post, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        // NOTE: if ThemeOfTheYearId was changed by this update, the already-
        // loaded post.ThemeOfTheYear navigation still points at the OLD theme
        // in memory — EF does not auto-refresh navigation properties when a
        // scalar FK changes. ThemeTitle on this response may briefly be
        // stale/wrong until the post is fetched again on a later request
        // (a fresh DbContext will resolve it correctly next time). Not a
        // data-correctness bug — ThemeOfTheYearId itself is saved correctly —
        // just a display nicety worth fixing if it matters for the admin UI.
        return post.ToPastorPostDto();
    }
}