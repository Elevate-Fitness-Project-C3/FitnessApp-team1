﻿using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using TrainingService.Domain.Common;

namespace TrainingService.Domain.IRepositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll();
        IQueryable<T> GetAllIncludingDeleted();
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void SaveInclude(T entity, params string[] includedProperties);

        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
        Task<int> CountAsync(Expression<Func<T, bool>>? criteria = null);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);

        Task UpdateAsync(
         Expression<Func<T, bool>> predicate,
         Action<UpdateSettersBuilder<T>> setters,
         CancellationToken cancellationToken);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}