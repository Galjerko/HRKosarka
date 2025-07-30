using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Requests;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain.Common;
using HRKošarka.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HRKošarka.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly HRDatabaseContext _context;

        public GenericRepository(HRDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<T>> GetAsync()
        {
            return await _context.Set<T>()
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PaginatedResponse<T>> GetPagedAsync(PaginationRequest request)
        {
            var query = _context.Set<T>().AsQueryable();

            if (!string.IsNullOrEmpty(request.SearchTerm) && request.SearchableProperties.Any())
            {
                query = ApplyGenericSearch(query, request.SearchTerm, request.SearchableProperties);
            }

            query = ApplyGenericSorting(query, request);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .AsNoTracking()
                .ToListAsync();

            return PaginatedResponse<T>.Success(
                items,
                request.Page,
                request.PageSize,
                totalCount,
                $"Retrieved {items.Count} items from page {request.Page}"
            );
        }

        private IQueryable<T> ApplyGenericSearch(IQueryable<T> query, string searchTerm, List<string> searchableProperties)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? searchExpression = null;

            foreach (var prop in searchableProperties)
            {
                var property = typeof(T).GetProperty(prop);
                if (property?.PropertyType == typeof(string))
                {
                    var propertyAccess = Expression.Property(parameter, property);
                    var nullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null));

                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
                    var propertyToLower = Expression.Call(propertyAccess, toLowerMethod);
                    var searchTermLower = Expression.Constant(searchTerm.ToLower());
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
                    var containsCall = Expression.Call(propertyToLower, containsMethod, searchTermLower);

                    var condition = Expression.AndAlso(nullCheck, containsCall);

                    searchExpression = searchExpression == null
                        ? condition
                        : Expression.OrElse(searchExpression, condition);
                }
            }

            if (searchExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(searchExpression, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

        private IQueryable<T> ApplyGenericSorting(IQueryable<T> query, PaginationRequest request)
        {
            string sortProperty = "Name";
            bool descending = true;

            if (!string.IsNullOrEmpty(request.SortBy))
            {
                if (request.SortableProperties.Any())
                {
                    if (request.SortableProperties.Contains(request.SortBy))
                    {
                        sortProperty = request.SortBy;
                        descending = request.SortDirection?.ToLower() == "desc";
                    }
                }
                else
                {
                    var property = typeof(T).GetProperty(request.SortBy);
                    if (property != null)
                    {
                        sortProperty = request.SortBy;
                        descending = request.SortDirection?.ToLower() == "desc";
                    }
                }
            }

            return ApplyOrderBy(query, sortProperty, descending);
        }

        private IQueryable<T> ApplyOrderBy(IQueryable<T> query, string propertyName, bool descending)
        {
            var property = typeof(T).GetProperty(propertyName);
            if (property == null) return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression propertyAccess = Expression.Property(parameter, property);

            if (property.PropertyType == typeof(string))
            {
                var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
                propertyAccess = Expression.Call(propertyAccess, toLowerMethod);
            }

            var lambda = Expression.Lambda(propertyAccess, parameter);

            var methodName = descending ? "OrderByDescending" : "OrderBy";
            var method = typeof(Queryable).GetMethods()
                .Where(m => m.Name == methodName && m.IsGenericMethodDefinition && m.GetParameters().Length == 2)
                .Single()
                .MakeGenericMethod(typeof(T), propertyAccess.Type);

            return (IQueryable<T>)method.Invoke(null, new object[] { query, lambda })!;
        }




        public IQueryable<T> GetQueryable()
        {
            return _context.Set<T>().AsQueryable();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _context.Set<T>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<T> CreateAsync(T entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
            if (entity != null)
            {
                entity.DateDeleted = DateTime.UtcNow;
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

    }
}
