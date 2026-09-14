using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Core.Interfaces
{
    public interface IContactUsRepository
    {
        Task<ContactUs?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<ContactUs>> GetAllAsync(int skip, int take, CancellationToken ct = default);
        Task<IReadOnlyList<ContactUs>> GetByReasonAsync(ContactReason reason, CancellationToken ct = default);
        Task<IReadOnlyList<ContactUs>> GetUnreadAsync(CancellationToken ct = default);
        Task<int> GetUnreadCountAsync(CancellationToken ct = default);
        Task AddAsync(ContactUs request, CancellationToken ct = default);
        Task UpdateAsync(ContactUs request, CancellationToken ct = default);
        /// <summary>
        /// Deletes a contact request permanently.
        /// </summary>
        /// <param name="contact">The entity to delete.</param>
        /// <param name="ct">Cancellation token.</param>
        Task DeleteAsync(ContactUs contact, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
