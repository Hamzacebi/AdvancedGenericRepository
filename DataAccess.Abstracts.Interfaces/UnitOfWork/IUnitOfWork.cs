using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Abstracts.Interfaces.UnitOfWork
{
    using RepositoryEntities;
    using System.Transactions;

    public interface IUnitOfWork
    {
        #region Access the Repositories

        /// <summary>
        /// Database islemleri icin yazilmis olan Base ve ozellikle User tablosu icin yazilmis olan fonksiyonlara erisim imkani saglayan property
        /// </summary>
        IRepositoryUser RepositoryOfUser { get; }

        /// <summary>
        /// Database islemleri icin yazilmis olan Base ve ozellikle Category tablosu icin yazilmis olan fonksiyonlara erisim imkani saglayan property
        /// </summary>
        IRepositoryCategory RepositoryOfCategory { get; }

        /// <summary>
        /// Database islemleri icin yazilmis olan Base ve ozellikle Product tablosu icin yazilmis olan fonksiyonlara erisim imkani saglayan property
        /// </summary>
        IRepositoryProduct RepositoryOfProduct { get; }

        /// <summary>
        /// Database islemleri icin yazilmis olan Base ve ozellikle Order tablosu icin yazilmis olan fonksiyonlara erisim imkani saglayan property
        /// </summary>
        IRepositoryOrder RepositoryOfOrder { get; }
        #endregion Access the Repositories

        #region Transaction Functions

        //TransactionScope BeginTransaction();

        //void CommitTransaction();

        //void RollBackTransaction();

        #endregion

        #region SaveChanges Function
        /// <summary>
        /// Database uzerinde yapilan Update - Delete ve Insert islemlerinin Database'e kayit edilmesini saglayan fonksiyon
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
        #endregion SaveChanges Function

    }
}
