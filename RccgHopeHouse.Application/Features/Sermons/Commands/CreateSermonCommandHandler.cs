using MediatR;
using RccgHopeHouse.Application.Features.Sermons.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Sermons.Commands
{
    public class CreateSermonCommandHandler : IRequestHandler<CreateSermonCommand, SermonDto>
    {
        private readonly ISermonRepository _repository;
        private readonly ICurrentUserService _currentUser;

        public CreateSermonCommandHandler(
            ISermonRepository repository,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<SermonDto> Handle(CreateSermonCommand cmd, CancellationToken ct)
        {
            // 1. Domain factory enforces business rules
            var sermon = Sermon.Create(
                cmd.Title,
                cmd.Speaker,
                cmd.ServiceDate,
                cmd.VideoUrl,
                cmd.Description
            );

            // 2. Persist via Core interface (Infrastructure implements)
            await _repository.AddAsync(sermon, ct);
            await _repository.SaveChangesAsync(ct);

            // 3. Map entity → response DTO
            return new SermonDto(
                Id: sermon.Id,
                Title: sermon.Title,
                Speaker: sermon.Speaker,
                ServiceDate: sermon.ServiceDate,
                VideoUrl: sermon.VideoUrl,
                Description: sermon.Description,
                IsPublished: sermon.IsPublished,
                CreatedAt: sermon.CreatedAt
            );
        }
    }
}
