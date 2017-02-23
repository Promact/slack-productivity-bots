using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.DataRepository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private DbContext db;
        private DbSet<T> dbSet;

        public Repository(DbContext context)
        {
            this.db = context;
            dbSet = db.Set<T>();
        }

        /// <summary>
        /// To get List of all from database 
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetAll()
        {
            return dbSet.AsQueryable();
        }

        /// <summary>
        /// To search any data by its integer Id number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetById(int? id)
        {
            return dbSet.Find(id);
        }

        /// <summary>
        /// Method to add data in database
        /// </summary>
        /// <param name="entity"></param>
        public void Insert(T entity)
        {
            dbSet.Add(entity);
        }

        /// <summary>
        /// Method to delete data from database from its integer Id
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int? id)
        {
            dbSet.Remove(dbSet.Find(id));
        }

        /// <summary>
        /// Method used for delete multipule data from database 
        /// </summary>
        /// <param name="predicate"></param>
        public void RemoveRange(Expression<Func<T, bool>> predicate)
        {
            dbSet.RemoveRange(dbSet.Where(predicate));
        }

        /// <summary>
        /// Method to update data in existing database
        /// </summary>
        /// <param name="entity"></param>
        public void Update(T entity)
        {
            db.Entry(entity).State = EntityState.Modified;
        }



        /// <summary>
        /// Saves the changes of the database using Async
        /// </summary>
        /// <returns></returns>
        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Method to search database using Linq Expression and get FirstOrDefault Value corresponding to expression
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return dbSet.FirstOrDefault(predicate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Method fetches the first item from the datacontext based on the the supplied function.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<T> FirstAsync(Expression<Func<T, bool>> predicate)
        {
            return await dbSet.FirstAsync<T>(predicate);
        }

        /// <summary>
        /// Method to search database using Linq Expression and get All Value corresponding to expression
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<T> Fetch(Func<T, bool> predicate)
        {
            try
            {
                return dbSet.Where<T>(predicate).AsQueryable();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Method fetches the IQueryable based on expression.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FetchAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {

                return await dbSet.Where(predicate).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Method to search database using Linq Expression and return true or false for any existance of data in database
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Any(Func<T, bool> predicate)
        {
            try
            {
                return dbSet.Any(predicate);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Method to search database using Linq Expression and return true or false for corresponding to expression
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool All(Func<T, bool> predicate)
        {
            try
            {
                return dbSet.All(predicate);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Method to search database using Linq Expression and get LastOrDefault Value corresponding to expression
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T LastOrDefault(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return dbSet.LastOrDefault(predicate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Method fetches the first or default item from the datacontext based on the the supplied function.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await dbSet.FirstOrDefaultAsync(predicate);

            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool disposed = false;

        /// <summary>
        /// Dispose Method
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await dbSet.FirstOrDefaultAsync(predicate);

            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// Method used for delete multipule data from database
        /// </summary>
        /// <param name="predicate"></param>
        public void RemoveRange(Expression<Func<T, bool>> predicate)
        {
            dbSet.RemoveRange(dbSet.Where(predicate));
        }
    }
}
