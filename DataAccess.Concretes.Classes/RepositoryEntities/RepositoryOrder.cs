using System;
using System.Collections.Generic;
using System.Text;

#region Custom Usings
using Models.DatabaseModels.DatabaseEntities;

using DataAccess.Concretes.Classes.RepositoryBase;
using DataAccess.Abstracts.Interfaces.RepositoryEntities;
using Microsoft.EntityFrameworkCore;
#endregion Custom Usings

namespace DataAccess.Concretes.Classes.RepositoryEntities
{
    public class RepositoryOrder : RepositoryBase<Orders>, IRepositoryOrder
    {
        public RepositoryOrder(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
