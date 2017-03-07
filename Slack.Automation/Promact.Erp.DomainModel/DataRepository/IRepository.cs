using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.DataRepository
{
    public interface IRepository<T> : IDisposable
    {
        /// <summary>
        /// To get List of all from database 
        /// </summary>
        /// <returns></returns>
        IQueryable<T> GetAll();

        /// <summary>
        /// To search any data by its integer Id number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetById(int? id);

        /// <summary>
        /// Method to add data in database
        /// </summary>
        /// <param name="entity"></param>
        void Insert(T entity);


        /// <summary>
        /// Method to delete data from database from its integer Id
        /// </summary>
        /// <param name="id"></param>
        void Delete(int? id);

        /// <summary>
        /// Method to update data in existing database
        /// </summary>
        /// <param name="entity"></param>
        void Update(T entity);

        /// <summary>
        /// Saves the changes of the database using Async
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Method to search database using Linq Expression and get FirstOrDefault Value corresponding to expression
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        T FirstOrDefault(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Method to search database using Linq Expression and get All Value corresponding to expression
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IQueryable<T> Fetch(Func<T, bool> predicate);

        /// <summary>
        /// Method fetches the IQueryable based on expression.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> FetchAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Method to search database using Linq Expression and return true or false for any existance of data in database
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool Any(Func<T, bool> predicate);

        /// <summary>
        /// Method to search database using Linq Expression and return true or false for corresponding to expression
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool All(Func<T, bool> predicate);

        /// <summary>
        /// Method to search database using Linq Expression and get LastOrDefault Value corresponding to expression
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        T LastOrDefault(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Method fetches the first or default item from the datacontext based on the the supplied function.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Method fetches the first item from the datacontext based on the the supplied function.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<T> FirstAsync(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// Method used for delete multipule data from database
        /// </summary>
        /// <param name="predicate"></param>
        void RemoveRange(Expression<Func<T, bool>> predicate);
    }
}
