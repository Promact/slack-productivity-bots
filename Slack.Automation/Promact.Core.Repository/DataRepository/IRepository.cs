using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Promact.Core.Repository.DataRepository
{
    public interface IRepository<T> : IDisposable
    {
        IQueryable<T> GetAll();

        T GetById(int? id);
        T GetById(string id);
        void Insert(T entity);
        void Delete(int? id);
        void Update(T entity);
        void Save();
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        IQueryable<T> Fetch(Func<T, bool> predicate);
        bool Any(Func<T, bool> predicate);
        bool All(Func<T, bool> predicate);
        T LastOrDefault(Expression<Func<T, bool>> predicate);
    }
}
