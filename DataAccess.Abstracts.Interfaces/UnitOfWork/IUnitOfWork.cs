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
        IRepositoryUser RepositoryOfUser { get; }

        IRepositoryCategory RepositoryOfCategory { get; }

        IRepositoryProduct RepositoryOfProduct { get; }

        IRepositoryOrder RepositoryOfOrder { get; }
        #endregion Access the Repositories

        #region Transaction Functions

        //TransactionScope BeginTransaction();

        //void CommitTransaction();

        //void RollBackTransaction();

        #endregion

        #region SaveChanges Function
        //int SaveChanges();
        #endregion SaveChanges Function

    }
}
