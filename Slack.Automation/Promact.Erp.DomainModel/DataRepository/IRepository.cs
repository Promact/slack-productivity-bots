using System;
using System.Linq;
using System.Linq.Expressions;

namespace Promact.Erp.DomainModel.DataRepository
{
    public interface IRepository<T> : IDisposable
    {
        IQueryable<T> GetAll();
        T GetById(int? id);
        void Insert(T entity);
        void Delete(int? id);
        void Update(T entity);
        void Save();
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        IQueryable<T> Fetch(Func<T, bool> predicate);
    }
}
