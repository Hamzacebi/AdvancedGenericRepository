using System;

#region Custom Usings
using System.Linq;
using System.Linq.Expressions;
#endregion Custom Usings

namespace DataAccess.Abstracts.Interfaces.RepositoryBase
{
    public interface IRepositorySelectable<T> where T : class
    {
        IQueryable<T> GetAllItems();
        IQueryable<T> GetAllItems(Expression<Func<T, bool>> predicate);

        T GetItem(object id);
        T GetItem(Expression<Func<T, bool>> predicate);
    }
}
