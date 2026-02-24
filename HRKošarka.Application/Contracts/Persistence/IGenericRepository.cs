using HRKošarka.Application.Models.Requests;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain.Common;
using System.Threading;

namespace HRKošarka.Application.Contracts.Persistence
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<IReadOnlyList<T>> GetAsync(CancellationToken cancellationToken = default);
        Task<PaginatedResponse<T>> GetPagedAsync(PaginationRequest request, CancellationToken cancellationToken = default); 
        IQueryable<T> GetQueryable(); 
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);
        Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default); 
    }

}
