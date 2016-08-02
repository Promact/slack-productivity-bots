using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.DataRepository
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

        public IEnumerable<T> List()
        {
            return dbSet.AsEnumerable(); ;
        }

        public virtual T GetById(int? id)
        {
            return dbSet.Find(id);
        }
        public virtual T GetById(string id)
        {
            return dbSet.Find(id);
        }

        public void Insert(T entity)
        {
            dbSet.Add(entity);
            db.SaveChanges();
        }

        public void Delete(int? id)
        {
            dbSet.Remove(dbSet.Find(id));
            db.SaveChanges();
        }
        public void Delete(string id)
        {
            T detail = dbSet.Find(id);
            dbSet.Remove(detail);
            db.SaveChanges();
        }

        public void Update(T entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void Save()
        {
            db.SaveChanges();
        }

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

        private bool disposed = false;

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


        public IQueryable<T> Fetch(Func<T, bool> predicate, string includeProperties)
        {
            try
            {
                var query = dbSet.Where<T>(predicate).AsQueryable();

                if (!String.IsNullOrWhiteSpace(includeProperties))
                {
                    includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList().
                        ForEach(x => query = query.Include(x));
                }

                return query;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
