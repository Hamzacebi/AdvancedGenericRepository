using System;
using System.Collections.Generic;
using System.Text;

#region Custom Usings
using Common.Commons;

using Models.DatabaseModels.DatabaseContext;

using DataAccess.Abstracts.Interfaces.UnitOfWork;
using DataAccess.Abstracts.Interfaces.RepositoryEntities;

using DataAccess.Concretes.Classes.RepositoryEntities;

#endregion Custom Usings

namespace DataAccess.Concretes.Classes.UnitOfWork
{
    using Microsoft.EntityFrameworkCore;
    using RepositoryBase;
    using System.Transactions;

    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        #region Constants

        private bool disposedValue = false;

        private readonly object lockObjectForUser = new object();
        private readonly object lockObjectForOrder = new object();
        private readonly object lockObjectForProduct = new object();
        private readonly object lockObjectForCategory = new object();
        private readonly object lockObjectForDBContext = new object();

        private IRepositoryUser repositoryOfUser;
        private IRepositoryOrder repositoryOfOrder;
        private IRepositoryProduct repositoryOfProduct;
        private IRepositoryCategory repositoryOfCategory;


        private readonly DbContext dbContext;
        private readonly TransactionScope objectOfTransaction;

        #endregion Constants


        #region Constructors

        public UnitOfWork(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        #endregion Constructors


        #region Repository Properties

        IRepositoryUser IUnitOfWork.RepositoryOfUser
        {
            get
            {
                if (this.repositoryOfUser == null)
                {
                    lock (this.lockObjectForUser)
                    {
                        if (this.repositoryOfUser == null)
                        {
                            if (this.CheckDbContextTypeForOrderDBContext(typeOfDbContext: typeof(OrderDBContext)))
                            {
                                this.repositoryOfUser = new RepositoryUser(dbContext: this.dbContext);
                            }
                            else
                            {
                                throw new ArgumentNullException(message: ErrorConstants.argumentNullExceptionMessageForOrderDBContext,
                                                                innerException: null);
                            }
                        }
                    }
                }
                return this.repositoryOfUser;
            }
        }

        IRepositoryCategory IUnitOfWork.RepositoryOfCategory
        {
            get
            {
                if (this.repositoryOfCategory == null)
                {
                    lock (this.lockObjectForCategory)
                    {
                        if (this.repositoryOfCategory == null)
                        {
                            if (this.CheckDbContextTypeForOrderDBContext(typeOfDbContext: typeof(OrderDBContext)))
                            {
                                this.repositoryOfCategory = new RepositoryCategory(dbContext: this.dbContext);
                            }
                            else
                            {
                                throw new ArgumentNullException(message: ErrorConstants.argumentNullExceptionMessageForOrderDBContext,
                                                                innerException: null);
                            }
                        }
                    }
                }
                return this.repositoryOfCategory;
            }
        }

        IRepositoryProduct IUnitOfWork.RepositoryOfProduct
        {
            get
            {
                if (this.repositoryOfProduct == null)
                {
                    lock (this.lockObjectForProduct)
                    {
                        if (this.repositoryOfProduct == null)
                        {
                            if (this.CheckDbContextTypeForOrderDBContext(typeOfDbContext: typeof(OrderDBContext)))
                            {
                                this.repositoryOfProduct = new RepositoryProduct(dbContext: this.dbContext);
                            }
                            else
                            {
                                throw new ArgumentNullException(message: ErrorConstants.argumentNullExceptionMessageForOrderDBContext,
                                                                innerException: null);
                            }
                        }
                    }
                }
                return this.repositoryOfProduct;
            }
        }

        IRepositoryOrder IUnitOfWork.RepositoryOfOrder
        {
            get
            {
                if (this.repositoryOfOrder == null)
                {
                    lock (this.lockObjectForOrder)
                    {
                        if (this.repositoryOfOrder == null)
                        {
                            if (this.CheckDbContextTypeForOrderDBContext(typeOfDbContext: typeof(OrderDBContext)))
                            {
                                this.repositoryOfOrder = new RepositoryOrder(dbContext: this.dbContext);
                            }
                            else
                            {
                                throw new ArgumentNullException(message: ErrorConstants.argumentNullExceptionMessageForOrderDBContext,
                                                                innerException: null);
                            }
                        }
                    }
                }
                return this.repositoryOfOrder;
            }
        }

        #endregion Repository Properties


        #region Save Change Function

        int IUnitOfWork.SaveChanges()
        {
            return Tools.TryCatch<int>(function: () =>
            {
                return this.dbContext.SaveChanges();
            });
        }

        #endregion

        #region Transaction Functions

        TransactionScope IUnitOfWork.BeginTransaction()
        {
            return new TransactionScope();
        }

        #endregion

        #region Private Functions
        /// <summary>
        /// OrderDB icerisindeki tablolara ait repositoryler sadece OrderDBContext'i uzerinden uretilebilmesi icin yazilan fonksiyon
        /// </summary>
        /// <param name="typeOfDbContext">Kontrol edilmek istenilen herhangi bir DbContext class'i</param>
        /// <returns></returns>
        private bool CheckDbContextTypeForOrderDBContext(Type typeOfDbContext)
        {
            bool result = default(bool);
            if (typeOfDbContext != null)
            {
                result = typeOfDbContext == this.dbContext.GetType();
            }
            else
            {
                throw new ArgumentNullException(message: ErrorConstants.argumentNullExceptionMessageForDbContext,
                                                innerException: null);
            }
            return result;
        }
        #endregion

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.dbContext.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
