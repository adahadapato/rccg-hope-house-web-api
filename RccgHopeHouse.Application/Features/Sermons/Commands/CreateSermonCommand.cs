using MediatR;
using RccgHopeHouse.Application.Features.Sermons.Dtos;
using System.ComponentModel.DataAnnotations;

namespace RccgHopeHouse.Application.Features.Sermons.Commands
{
    public record CreateSermonCommand(
     [Required, MaxLength(200)] string Title,
     [Required, MaxLength(100)] string Speaker,
     [Required] DateTime ServiceDate,
     [Required, Url] string VideoUrl,
     [MaxLength(2000)] string? Description) : IRequest<SermonDto>;
}
