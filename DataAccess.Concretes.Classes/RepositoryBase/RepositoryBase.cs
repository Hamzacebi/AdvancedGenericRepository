using System;
using System.Collections.Generic;
using System.Text;

#region Custom Usings
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

using Common.Commons;
using DataAccess.Abstracts.Interfaces.RepositoryBase;

#endregion

namespace DataAccess.Concretes.Classes.RepositoryBase
{
    public abstract class RepositoryBase<T> : IRepositorySelectable<T>, IRepositoryInsertable<T>,
                                              IRepositoryUpdatable<T>, IRepositoryDeletable<T> where T : class
    {
        #region Constants

        protected DbContext DbContext { get; set; }
        protected DbSet<T> DbSet { get; set; }

        #endregion Constants

        #region Constructors

        public RepositoryBase(DbContext dbContext)
        {
            this.DbContext = dbContext ?? throw new ArgumentNullException(ErrorConstants.argumentNullExceptionMessageForDbContext);
            this.DbSet = DbContext.Set<T>();
        }
        #endregion Constructors

        #region Select Functions
        /// <summary>
        /// hamsi
        /// </summary>
        /// <returns></returns>
        IQueryable<T> IRepositorySelectable<T>.GetAllItems()
        {
            return this.DbSet.AsQueryable();
        }

        IQueryable<T> IRepositorySelectable<T>.GetAllItems(Expression<Func<T, bool>> predicate)
        {
            return this.DbSet.Where(predicate: predicate)
                             .AsQueryable();
        }

        T IRepositorySelectable<T>.GetItem(object id)
        {
            T getItemById = this.DbSet.Find(keyValues: id);
            return getItemById ?? null;
        }

        T IRepositorySelectable<T>.GetItem(Expression<Func<T, bool>> predicate)
        {
            return this.DbSet.SingleOrDefault(predicate: predicate);
        }
        #endregion Select Functions

        #region Insert Functions
        T IRepositoryInsertable<T>.InsertItem(T insertItem)
        {
            var addedRecord = this.DbSet.Attach(entity: insertItem);
            addedRecord.State = EntityState.Added;
            return addedRecord.Entity;
        }
        #endregion Insert Functions

        #region Update Functions
        T IRepositoryUpdatable<T>.UpdateItem(T updateItem)
        {
            var updatedRecord = this.DbSet.Attach(entity: updateItem);
            updatedRecord.State = EntityState.Modified;
            return updatedRecord.Entity;
        }
        #endregion Update Functions

        #region Delete Functions
        bool IRepositoryDeletable<T>.TemporaryDelete(object id)
        {

            //Tools.TryCatch<bool>(function: hamsi(id: id));
            return false;
        }

        bool IRepositoryDeletable<T>.PermanentDelete(object id)
        {

            return Tools.TryCatch<bool>(function: () => {

                bool result = default(bool);

                T getRecord = this.DbSet.Find(keyValues: id);
                if (getRecord != null)
                {
                    var deletedRecord = this.DbSet.Attach(entity: getRecord);
                    deletedRecord.State = EntityState.Deleted;
                    result = this.DbContext.SaveChanges() > 0 ? true : false;
                }
                return result;
            });
        }
        #endregion Delete Functions
    }
}
