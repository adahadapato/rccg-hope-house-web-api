using MediatR;
using RccgHopeHouse.Application.Common.Mappings;
using RccgHopeHouse.Application.Features.PastorPosts.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PastorPosts.Commands;

public class CreatePastorPostCommandHandler : IRequestHandler<CreatePastorPostCommand, PastorPostDto>
{
    private readonly IPastorPostRepository _repository;

    public CreatePastorPostCommandHandler(IPastorPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<PastorPostDto> Handle(CreatePastorPostCommand request, CancellationToken cancellationToken)
    {
        var post = PastorPost.Create(
            title: request.Title,
            content: request.Content,
            category: request.Category,
            themeOfTheYearId: request.ThemeOfTheYearId,
            authorName: request.AuthorName,
            excerpt: request.Excerpt,
            introHeading: request.IntroHeading,
            introText: request.IntroText,
            structuredContentJson: StructuredContentDto.Serialize(request.StructuredContent),
            closingText: request.ClosingText,
            coverImageData: request.CoverImageData,
            coverImageContentType: request.CoverImageContentType,
            bibleReference: request.BibleReference);

        await _repository.AddAsync(post, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        // NOTE: post.ThemeOfTheYear navigation is not loaded here — only the
        // FK (ThemeOfTheYearId) was set on the in-memory entity — so
        // ThemeTitle on the returned DTO will be null immediately after
        // creation. This is safe (no crash) but incomplete. If the response
        // needs the theme title right away, this handler should fetch the
        // theme separately, or the repository should re-query with
        // .Include(p => p.ThemeOfTheYear) after save.
        return post.ToPastorPostDto();
    }
}