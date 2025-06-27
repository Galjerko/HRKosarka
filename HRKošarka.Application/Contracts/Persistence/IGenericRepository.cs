using HRKošarka.Application.Models.Requests;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain.Common;

namespace HRKošarka.Application.Contracts.Persistence
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<IReadOnlyList<T>> GetAsync();
        Task<PaginatedResponse<T>> GetPagedAsync(PaginationRequest request); 
        IQueryable<T> GetQueryable(); 
        Task<T?> GetByIdAsync(Guid id);
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(Guid id); 
    }

}
