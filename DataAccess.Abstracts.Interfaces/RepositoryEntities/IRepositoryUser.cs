using System;
using System.Collections.Generic;
using System.Text;

#region Custom Usings
using Models.DatabaseModels.DatabaseEntities;
#endregion Custom Usings

namespace DataAccess.Abstracts.Interfaces.RepositoryEntities
{
    using RepositoryBase;

    public interface IRepositoryUser : IRepositorySelectable<Users>, IRepositoryInsertable<Users>
    {
    }
}
