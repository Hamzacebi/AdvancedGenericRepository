using System;
using System.Collections.Generic;
using System.Text;

#region Custom Usings
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

using Common.Commons;
using DataAccess.Abstracts.Interfaces.RepositoryBase;

using Models.DatabaseModels.DatabaseEntities.EntityBase;

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
        IQueryable<T> IRepositorySelectable<T>.GetAllItems() => this.DbSet.AsQueryable();

        IQueryable<T> IRepositorySelectable<T>.GetAllItems(Expression<Func<T, bool>> predicate)
        {
            return this.DbSet.Where(predicate: predicate)
                             .AsQueryable();
        }

        T IRepositorySelectable<T>.GetItem(object id) =>
            Tools.TryCatch<T>(function: () =>
            {
                T getItemById = this.DbSet.Find(keyValues: id);
                return getItemById ?? null;
            });

        T IRepositorySelectable<T>.GetItem(Expression<Func<T, bool>> predicate)
        {
            return Tools.TryCatch<T>(function: () =>
            {
                return this.DbSet.SingleOrDefault(predicate: predicate);
            });
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
        T IRepositoryUpdatable<T>.UpdateItem(T updateItem) =>
            Tools.TryCatch<T>(function: () =>
            {
                return this.BasicUpdate(item: updateItem);
            });
        #endregion Update Functions

        #region Delete Functions
        T IRepositoryDeletable<T>.TemporaryDelete(T deleteEntity)
        {
            Func<T> function = () =>
            {
                return this.BasicUpdate(item: deleteEntity);
            };
            return function.TryCatch();
        }

        //bool IRepositoryDeletable<T>.PermanentDelete<T1>(object id) =>
        bool IRepositoryDeletable<T>.PermanentDelete(object id) =>
             Tools.TryCatch<bool>(function: () =>
             {
                 bool result = default(bool);

                 T getRecord = this.DbSet.Find(keyValues: id);
                 if (getRecord != null)
                 {
                     var deletedRecord = this.DbSet.Attach(entity: getRecord);
                     deletedRecord.State = EntityState.Deleted;
                     result = this.DbContext.SaveChanges() > 0 ? true : false;
                 }
                 return result;
             }, catchAndDo: (Exception exception) =>
             {
                 return false;
             });
        #endregion Delete Functions

        #region Private Functions

        /// <summary>
        /// Verilen T tipindeki itemi guncellenecek eleman olarak Database'ye Attach eden fonksiyon
        /// TemporaryDelete ve Update fonksiyonlarinda ayni govde kullanilacak oldugu icin tek sefer yazip N kere kullanmak amaciyla yazildi
        /// </summary>
        /// <param name="item">Guncellenmek istenilen class'a ait bilgiler</param>
        /// <returns></returns>
        T BasicUpdate(T item)
        {
            var updatedRecord = this.DbSet.Attach(entity: item);
            updatedRecord.State = EntityState.Modified;
            return updatedRecord.Entity;
        }

        #endregion
    }
}
